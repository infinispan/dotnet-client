using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infinispan.Hotrod.Linq
{
    internal enum ResultOperator
    {
        List,
        First,
        FirstOrDefault,
        Single,
        SingleOrDefault,
        Count,
        LongCount
    }

    internal sealed class IckleQueryModel
    {
        public string FromTypeName { get; set; }
        public Type EntityType { get; set; }
        public string WhereClause { get; set; }
        public List<string> SelectFields { get; } = new();
        public LambdaExpression ProjectionExpression { get; set; }
        public List<(string ProtoFieldName, bool Descending)> OrderByClauses { get; } = new();
        public long? Skip { get; set; }
        public int? Take { get; set; }
        public ResultOperator ResultOperator { get; set; } = ResultOperator.List;
        public Dictionary<string, object> Parameters { get; } = new();
        private int _paramCounter;

        public string AddParameter(object value)
        {
            var name = $"p{_paramCounter++}";
            Parameters[name] = value;
            return name;
        }

        public string ToIckle()
        {
            var sb = new System.Text.StringBuilder();
            if (SelectFields.Count > 0)
            {
                sb.Append("SELECT ");
                for (int i = 0; i < SelectFields.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append("e.");
                    sb.Append(SelectFields[i]);
                }
                sb.Append(' ');
            }
            sb.Append("FROM ");
            sb.Append(FromTypeName);
            sb.Append(" e");
            if (WhereClause != null)
            {
                sb.Append(" WHERE ");
                sb.Append(WhereClause);
            }
            if (OrderByClauses.Count > 0)
            {
                sb.Append(" ORDER BY ");
                for (int i = 0; i < OrderByClauses.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append("e.");
                    sb.Append(OrderByClauses[i].ProtoFieldName);
                    if (OrderByClauses[i].Descending) sb.Append(" DESC");
                }
            }
            return sb.ToString();
        }
    }
}
