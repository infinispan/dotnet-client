using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infinispan.Hotrod.Linq
{
    public sealed class IckleQueryable<T> : IOrderedQueryable<T>
    {
        internal IckleQueryable(IckleQueryProvider provider)
        {
            Provider = provider;
            Expression = Expression.Constant(this);
        }

        internal IckleQueryable(IckleQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
            => Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
