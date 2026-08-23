using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infinispan.Hotrod.Linq
{
    public static class AsyncQueryableExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            return provider.ExecuteAsync<List<T>>(source.Expression, ct);
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.First));
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source,
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.First), predicate);
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.FirstOrDefault));
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source,
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.FirstOrDefault), predicate);
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.Single));
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.SingleOrDefault));
            return provider.ExecuteAsync<T>(expression, ct);
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.Count));
            return provider.ExecuteAsync<int>(expression, ct);
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source,
            Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.Count), predicate);
            return provider.ExecuteAsync<int>(expression, ct);
        }

        public static Task<long> LongCountAsync<T>(this IQueryable<T> source, CancellationToken ct = default)
        {
            var provider = GetIckleProvider(source);
            var expression = BuildTerminalExpression<T>(source, nameof(Queryable.LongCount));
            return provider.ExecuteAsync<long>(expression, ct);
        }

        private static IckleQueryProvider GetIckleProvider<T>(IQueryable<T> source)
        {
            if (source.Provider is IckleQueryProvider p) return p;
            throw new InvalidOperationException("Async operations are only supported on Ickle queryables.");
        }

        private static Expression BuildTerminalExpression<T>(IQueryable<T> source, string methodName,
            Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                var withPredicate = FindQueryableMethod(methodName, 2, typeof(T));
                return Expression.Call(null, withPredicate, source.Expression, Expression.Quote(predicate));
            }
            var method = FindQueryableMethod(methodName, 1, typeof(T));
            return Expression.Call(null, method, source.Expression);
        }

        private static MethodInfo FindQueryableMethod(string name, int paramCount, Type elementType)
        {
            var method = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == name && m.GetParameters().Length == paramCount && m.IsGenericMethod);
            return method.MakeGenericMethod(elementType);
        }
    }
}
