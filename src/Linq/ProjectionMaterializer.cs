using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infinispan.Hotrod.Linq
{
    internal static class ProjectionMaterializer
    {
        public static Func<object[], TResult> Compile<TResult>(
            LambdaExpression selectLambda,
            List<string> projectedProtoFields,
            Type entityType)
        {
            var valuesParam = Expression.Parameter(typeof(object[]), "values");
            var rewriter = new ProjectionRewriter(
                selectLambda.Parameters[0], valuesParam, projectedProtoFields, entityType);
            var rewritten = rewriter.Visit(selectLambda.Body);
            return Expression.Lambda<Func<object[], TResult>>(rewritten, valuesParam).Compile();
        }

        private sealed class ProjectionRewriter : ExpressionVisitor
        {
            private readonly ParameterExpression _entityParam;
            private readonly ParameterExpression _valuesParam;
            private readonly List<string> _fields;
            private readonly Type _entityType;

            public ProjectionRewriter(ParameterExpression entityParam, ParameterExpression valuesParam,
                List<string> fields, Type entityType)
            {
                _entityParam = entityParam;
                _valuesParam = valuesParam;
                _fields = fields;
                _entityType = entityType;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == _entityParam && node.Member is PropertyInfo)
                {
                    string protoName = ProtobufFieldMapper.GetProtoFieldName(_entityType, node.Member.Name);
                    int index = _fields.IndexOf(protoName);
                    if (index < 0)
                        throw new InvalidOperationException(
                            $"Projected field '{node.Member.Name}' not found in SELECT fields.");
                    var arrayAccess = Expression.ArrayIndex(_valuesParam, Expression.Constant(index));
                    return Expression.Convert(arrayAccess, node.Type);
                }
                return base.VisitMember(node);
            }
        }
    }
}
