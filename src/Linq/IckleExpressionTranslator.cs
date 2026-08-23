using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infinispan.Hotrod.Linq
{
    internal static class IckleExpressionTranslator
    {
        public static IckleQueryModel Translate(Expression expression)
        {
            var model = new IckleQueryModel();
            Expression current = expression;

            while (current is MethodCallExpression mce && mce.Method.DeclaringType == typeof(Queryable))
            {
                switch (mce.Method.Name)
                {
                    case "Where":
                        HandleWhere(model, mce);
                        break;
                    case "OrderBy":
                        HandleOrderBy(model, mce, descending: false);
                        break;
                    case "OrderByDescending":
                        HandleOrderBy(model, mce, descending: true);
                        break;
                    case "ThenBy":
                        HandleOrderBy(model, mce, descending: false);
                        break;
                    case "ThenByDescending":
                        HandleOrderBy(model, mce, descending: true);
                        break;
                    case "Take":
                        HandleTake(model, mce);
                        break;
                    case "Skip":
                        HandleSkip(model, mce);
                        break;
                    case "Select":
                        HandleSelect(model, mce);
                        break;
                    case "First":
                        HandleTerminal(model, mce, ResultOperator.First);
                        break;
                    case "FirstOrDefault":
                        HandleTerminal(model, mce, ResultOperator.FirstOrDefault);
                        break;
                    case "Single":
                        HandleTerminal(model, mce, ResultOperator.Single);
                        break;
                    case "SingleOrDefault":
                        HandleTerminal(model, mce, ResultOperator.SingleOrDefault);
                        break;
                    case "Count":
                        HandleTerminal(model, mce, ResultOperator.Count);
                        break;
                    case "LongCount":
                        HandleTerminal(model, mce, ResultOperator.LongCount);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"The LINQ operator '{mce.Method.Name}' is not supported by the Ickle query provider.");
                }
                current = mce.Arguments[0];
            }

            if (current is ConstantExpression ce && ce.Value is IQueryable queryable)
            {
                model.EntityType = queryable.ElementType;
                model.FromTypeName = ProtobufFieldMapper.GetProtoTypeName(queryable.ElementType);
            }
            else
            {
                throw new NotSupportedException("Unexpected expression source.");
            }

            return model;
        }

        private static void HandleWhere(IckleQueryModel model, MethodCallExpression mce)
        {
            var lambda = StripQuotes(mce.Arguments[1]) as LambdaExpression;
            var entityType = lambda.Parameters[0].Type;
            var translator = new IckleWhereTranslator(model, entityType, lambda.Parameters[0]);
            var clause = translator.Translate(lambda.Body);

            model.WhereClause = model.WhereClause == null
                ? clause
                : $"({model.WhereClause}) AND ({clause})";
        }

        private static void HandleOrderBy(IckleQueryModel model, MethodCallExpression mce, bool descending)
        {
            var lambda = StripQuotes(mce.Arguments[1]) as LambdaExpression;
            var memberExpr = StripConvert(lambda.Body) as MemberExpression;
            if (memberExpr == null)
                throw new NotSupportedException("OrderBy must reference a property on the entity.");
            var entityType = lambda.Parameters[0].Type;
            var protoField = ProtobufFieldMapper.GetProtoFieldName(entityType, memberExpr.Member.Name);
            model.OrderByClauses.Insert(0, (protoField, descending));
        }

        private static void HandleTake(IckleQueryModel model, MethodCallExpression mce)
        {
            model.Take = (int)IckleWhereTranslator.EvaluateValue(mce.Arguments[1]);
        }

        private static void HandleSkip(IckleQueryModel model, MethodCallExpression mce)
        {
            model.Skip = Convert.ToInt64(IckleWhereTranslator.EvaluateValue(mce.Arguments[1]));
        }

        private static void HandleSelect(IckleQueryModel model, MethodCallExpression mce)
        {
            var lambda = StripQuotes(mce.Arguments[1]) as LambdaExpression;
            model.ProjectionExpression = lambda;
            var entityType = lambda.Parameters[0].Type;

            var members = new List<MemberExpression>();
            CollectMemberAccesses(lambda.Body, lambda.Parameters[0], members);

            foreach (var me in members)
            {
                var protoField = ProtobufFieldMapper.GetProtoFieldName(entityType, me.Member.Name);
                if (!model.SelectFields.Contains(protoField))
                    model.SelectFields.Add(protoField);
            }
        }

        private static void HandleTerminal(IckleQueryModel model, MethodCallExpression mce, ResultOperator op)
        {
            model.ResultOperator = op;
            if (mce.Arguments.Count > 1)
            {
                var lambda = StripQuotes(mce.Arguments[1]) as LambdaExpression;
                if (lambda != null)
                {
                    var entityType = lambda.Parameters[0].Type;
                    var translator = new IckleWhereTranslator(model, entityType, lambda.Parameters[0]);
                    var clause = translator.Translate(lambda.Body);
                    model.WhereClause = model.WhereClause == null
                        ? clause
                        : $"({model.WhereClause}) AND ({clause})";
                }
            }
        }

        private static void CollectMemberAccesses(Expression expr, ParameterExpression param, List<MemberExpression> members)
        {
            switch (expr)
            {
                case MemberExpression me when me.Expression == param:
                    members.Add(me);
                    break;
                case NewExpression ne:
                    foreach (var arg in ne.Arguments)
                        CollectMemberAccesses(arg, param, members);
                    break;
                case MemberInitExpression mie:
                    foreach (var binding in mie.Bindings)
                        if (binding is MemberAssignment ma)
                            CollectMemberAccesses(ma.Expression, param, members);
                    break;
                case UnaryExpression ue when ue.NodeType == ExpressionType.Convert:
                    CollectMemberAccesses(ue.Operand, param, members);
                    break;
            }
        }

        private static Expression StripQuotes(Expression expr)
        {
            while (expr.NodeType == ExpressionType.Quote)
                expr = ((UnaryExpression)expr).Operand;
            return expr;
        }

        private static Expression StripConvert(Expression expr)
        {
            while (expr is UnaryExpression ue && ue.NodeType == ExpressionType.Convert)
                expr = ue.Operand;
            return expr;
        }
    }
}
