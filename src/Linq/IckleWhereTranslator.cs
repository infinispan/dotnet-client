using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Infinispan.Hotrod.Linq
{
    internal sealed class IckleWhereTranslator : ExpressionVisitor
    {
        private readonly IckleQueryModel _model;
        private readonly Type _entityType;
        private readonly ParameterExpression _parameter;
        private readonly StringBuilder _sb = new();

        public IckleWhereTranslator(IckleQueryModel model, Type entityType, ParameterExpression parameter)
        {
            _model = model;
            _entityType = entityType;
            _parameter = parameter;
        }

        public string Translate(Expression body)
        {
            Visit(body);
            return _sb.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso)
            {
                _sb.Append('(');
                Visit(node.Left);
                _sb.Append(" AND ");
                Visit(node.Right);
                _sb.Append(')');
                return node;
            }
            if (node.NodeType == ExpressionType.OrElse)
            {
                _sb.Append('(');
                Visit(node.Left);
                _sb.Append(" OR ");
                Visit(node.Right);
                _sb.Append(')');
                return node;
            }

            var (memberExpr, valueExpr) = IdentifySides(node);
            string protoField = ResolveProtoFieldName(memberExpr);
            object value = EvaluateValue(valueExpr);

            if (value == null)
            {
                _sb.Append($"e.{protoField}");
                _sb.Append(node.NodeType == ExpressionType.Equal ? " IS NULL" : " IS NOT NULL");
                return node;
            }

            string paramName = _model.AddParameter(value);
            _sb.Append($"e.{protoField} ");
            _sb.Append(node.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Binary operator '{node.NodeType}' is not supported in Ickle queries.")
            });
            _sb.Append($" :{paramName}");
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                _sb.Append("NOT (");
                Visit(node.Operand);
                _sb.Append(')');
                return node;
            }
            if (node.NodeType == ExpressionType.Convert)
            {
                Visit(node.Operand);
                return node;
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (IsEntityMember(node) && node.Type == typeof(bool))
            {
                string protoField = ResolveProtoFieldName(node);
                string paramName = _model.AddParameter(true);
                _sb.Append($"e.{protoField} = :{paramName}");
                return node;
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(string) && node.Object is MemberExpression memberExpr && IsEntityMember(memberExpr))
            {
                string protoField = ResolveProtoFieldName(memberExpr);
                string argValue = (string)EvaluateValue(node.Arguments[0]);

                string likePattern = node.Method.Name switch
                {
                    "Contains" => $"%{argValue}%",
                    "StartsWith" => $"{argValue}%",
                    "EndsWith" => $"%{argValue}",
                    _ => throw new NotSupportedException($"String method '{node.Method.Name}' is not supported in Ickle queries.")
                };

                string paramName = _model.AddParameter(likePattern);
                _sb.Append($"e.{protoField} LIKE :{paramName}");
                return node;
            }
            throw new NotSupportedException($"Method '{node.Method.DeclaringType?.Name}.{node.Method.Name}' is not supported in Ickle queries.");
        }

        private bool IsEntityMember(MemberExpression node)
        {
            return node.Expression == _parameter ||
                   (node.Expression is UnaryExpression ue && ue.NodeType == ExpressionType.Convert && ue.Operand == _parameter);
        }

        private string ResolveProtoFieldName(MemberExpression memberExpr)
        {
            return ProtobufFieldMapper.GetProtoFieldName(_entityType, memberExpr.Member.Name);
        }

        private (MemberExpression member, Expression value) IdentifySides(BinaryExpression node)
        {
            if (IsMemberOnParameter(node.Left))
                return (ExtractMember(node.Left), node.Right);
            if (IsMemberOnParameter(node.Right))
                return (ExtractMember(node.Right), node.Left);
            throw new NotSupportedException("Comparisons must involve a property access on the entity.");
        }

        private bool IsMemberOnParameter(Expression expr)
        {
            var unwrapped = StripConvert(expr);
            return unwrapped is MemberExpression me && IsEntityMember(me);
        }

        private MemberExpression ExtractMember(Expression expr)
        {
            return StripConvert(expr) as MemberExpression;
        }

        private static Expression StripConvert(Expression expr)
        {
            while (expr is UnaryExpression ue && ue.NodeType == ExpressionType.Convert)
                expr = ue.Operand;
            return expr;
        }

        internal static object EvaluateValue(Expression expr)
        {
            var unwrapped = StripConvert(expr);
            if (unwrapped is ConstantExpression ce)
                return ce.Value;

            if (unwrapped is MemberExpression me)
            {
                var container = me.Expression != null ? EvaluateValue(me.Expression) : null;
                return me.Member switch
                {
                    FieldInfo fi => fi.GetValue(container),
                    PropertyInfo pi => pi.GetValue(container),
                    _ => throw new NotSupportedException($"Member type '{me.Member.MemberType}' is not supported.")
                };
            }

            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expr, typeof(object)));
            return lambda.Compile().Invoke();
        }
    }
}
