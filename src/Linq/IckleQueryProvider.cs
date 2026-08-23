using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Org.Infinispan.Query.Remote.Client;
using Org.Infinispan.Protostream;

namespace Infinispan.Hotrod.Linq
{
    internal sealed class IckleQueryProvider : IQueryProvider
    {
        private readonly Func<QueryRequest, Task<QueryResponse>> _queryExecutor;
        private readonly Func<byte[], object> _entityUnmarshaller;

        internal IckleQueryProvider(Func<QueryRequest, Task<QueryResponse>> queryExecutor,
            Func<byte[], object> entityUnmarshaller)
        {
            _queryExecutor = queryExecutor;
            _entityUnmarshaller = entityUnmarshaller;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetGenericArguments()[0];
            return (IQueryable)Activator.CreateInstance(
                typeof(IckleQueryable<>).MakeGenericType(elementType),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, new object[] { this, expression }, null);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new IckleQueryable<TElement>(this, expression);

        public object Execute(Expression expression)
            => ExecuteAsync<object>(expression, CancellationToken.None).GetAwaiter().GetResult();

        public TResult Execute<TResult>(Expression expression)
            => ExecuteAsync<TResult>(expression, CancellationToken.None).GetAwaiter().GetResult();

        internal async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken ct)
        {
            var model = IckleExpressionTranslator.Translate(expression);
            var queryRequest = CompileToQueryRequest(model);
            var response = await _queryExecutor(queryRequest);
            return MaterializeResult<TResult>(response, model);
        }

        private QueryRequest CompileToQueryRequest(IckleQueryModel model)
        {
            var qr = new QueryRequest();
            qr.QueryString = model.ToIckle();

            if (model.Skip.HasValue)
                qr.StartOffset = model.Skip.Value;

            int? maxResults = model.Take;
            switch (model.ResultOperator)
            {
                case ResultOperator.First:
                case ResultOperator.FirstOrDefault:
                    maxResults = maxResults.HasValue ? Math.Min(maxResults.Value, 1) : 1;
                    break;
                case ResultOperator.Single:
                case ResultOperator.SingleOrDefault:
                    maxResults = maxResults.HasValue ? Math.Min(maxResults.Value, 2) : 2;
                    break;
                case ResultOperator.Count:
                case ResultOperator.LongCount:
                    maxResults = 0;
                    break;
            }
            if (maxResults.HasValue)
                qr.MaxResults = maxResults.Value;

            foreach (var kv in model.Parameters)
            {
                var np = new QueryRequest.Types.NamedParameter();
                np.Name = kv.Key;
                np.Value = Cache<object, object>.WrapParameterValue(kv.Value);
                qr.NamedParameters.Add(np);
            }

            return qr;
        }

        private TResult MaterializeResult<TResult>(QueryResponse response, IckleQueryModel model)
        {
            switch (model.ResultOperator)
            {
                case ResultOperator.Count:
                    return (TResult)(object)(int)response.TotalResults;
                case ResultOperator.LongCount:
                    return (TResult)(object)response.TotalResults;
            }

            if (model.SelectFields.Count > 0 && model.ProjectionExpression != null)
                return MaterializeProjection<TResult>(response, model);

            return MaterializeEntities<TResult>(response, model);
        }

        private TResult MaterializeEntities<TResult>(QueryResponse response, IckleQueryModel model)
        {
            var entities = new List<object>();
            for (int i = 0; i < response.NumResults; i++)
            {
                var wm = response.Results[i];
                if (wm.WrappedBytes != null)
                    entities.Add(_entityUnmarshaller(wm.WrappedBytes.ToByteArray()));
            }

            return ApplyResultOperator<TResult>(entities, model.ResultOperator);
        }

        private TResult MaterializeProjection<TResult>(QueryResponse response, IckleQueryModel model)
        {
            var rows = UnwrapProjectionRows(response);

            var resultType = typeof(TResult);
            if (resultType.IsGenericType &&
                (resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                 resultType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var elementType = resultType.GetGenericArguments()[0];
                var method = typeof(ProjectionMaterializer)
                    .GetMethod(nameof(ProjectionMaterializer.Compile))
                    .MakeGenericMethod(elementType);
                var materializer = method.Invoke(null,
                    new object[] { model.ProjectionExpression, model.SelectFields, model.EntityType });

                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)Activator.CreateInstance(listType);
                foreach (var row in rows)
                    list.Add(materializer.GetType().GetMethod("Invoke").Invoke(materializer, new object[] { row }));
                return (TResult)list;
            }

            var scalarMaterializer = CompileProjectionMaterializer<TResult>(model);
            var items = new List<object>();
            foreach (var row in rows)
                items.Add(scalarMaterializer(row));
            return ApplyResultOperator<TResult>(items, model.ResultOperator);
        }

        private Func<object[], T> CompileProjectionMaterializer<T>(IckleQueryModel model)
        {
            return ProjectionMaterializer.Compile<T>(model.ProjectionExpression, model.SelectFields, model.EntityType);
        }

        private static List<object[]> UnwrapProjectionRows(QueryResponse response)
        {
            var rows = new List<object[]>();
            if (response.ProjectionSize == 0) return rows;
            for (int i = 0; i < response.NumResults; i++)
            {
                var row = new object[response.ProjectionSize];
                for (int j = 0; j < response.ProjectionSize; j++)
                {
                    WrappedMessage wm = response.Results[i * response.ProjectionSize + j];
                    row[j] = UnwrapScalar(wm);
                }
                rows.Add(row);
            }
            return rows;
        }

        private static object UnwrapScalar(WrappedMessage wm)
        {
            return wm.ScalarOrMessageCase switch
            {
                WrappedMessage.ScalarOrMessageOneofCase.WrappedDouble => wm.WrappedDouble,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedFloat => wm.WrappedFloat,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedInt64 => wm.WrappedInt64,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt64 => wm.WrappedUInt64,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedInt32 => wm.WrappedInt32,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed64 => wm.WrappedFixed64,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed32 => wm.WrappedFixed32,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedBool => wm.WrappedBool,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedString => wm.WrappedString,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedBytes => wm.WrappedBytes,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt32 => wm.WrappedUInt32,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed32 => wm.WrappedSFixed32,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed64 => wm.WrappedSFixed64,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt32 => wm.WrappedSInt32,
                WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt64 => wm.WrappedSInt64,
                _ => null
            };
        }

        private static TResult ApplyResultOperator<TResult>(List<object> items, ResultOperator op)
        {
            switch (op)
            {
                case ResultOperator.First:
                    if (items.Count == 0)
                        throw new InvalidOperationException("Sequence contains no elements.");
                    return (TResult)items[0];
                case ResultOperator.FirstOrDefault:
                    return items.Count > 0 ? (TResult)items[0] : default;
                case ResultOperator.Single:
                    if (items.Count == 0)
                        throw new InvalidOperationException("Sequence contains no elements.");
                    if (items.Count > 1)
                        throw new InvalidOperationException("Sequence contains more than one element.");
                    return (TResult)items[0];
                case ResultOperator.SingleOrDefault:
                    if (items.Count > 1)
                        throw new InvalidOperationException("Sequence contains more than one element.");
                    return items.Count > 0 ? (TResult)items[0] : default;
                case ResultOperator.List:
                default:
                    if (typeof(TResult).IsAssignableFrom(typeof(List<>).MakeGenericType(items.GetType().GetGenericArguments().FirstOrDefault() ?? typeof(object))))
                        return (TResult)(object)items;
                    var listType = typeof(TResult);
                    if (listType.IsGenericType)
                    {
                        var elementType = listType.GetGenericArguments()[0];
                        var typedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        foreach (var item in items)
                            typedList.Add(item);
                        return (TResult)typedList;
                    }
                    return (TResult)(object)items;
            }
        }
    }
}
