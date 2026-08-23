using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinispan.Hotrod.Linq;
using Org.Infinispan.Query.Remote.Client;
using SampleBankAccount;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    public class LinqTranslationTest
    {
        private static IQueryable<User> CreateQueryable()
        {
            var provider = new IckleQueryProvider(
                _ => Task.FromResult(new QueryResponse()),
                _ => new User());
            return new IckleQueryable<User>(provider);
        }

        private static IckleQueryModel TranslateQuery(IQueryable<User> query)
        {
            return IckleExpressionTranslator.Translate(query.Expression);
        }

        [Fact]
        public void TranslatesSimpleWhere()
        {
            var q = CreateQueryable().Where(u => u.Age > 30);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.age > :p0", model.ToIckle());
            Assert.Equal(30, model.Parameters["p0"]);
        }

        [Fact]
        public void TranslatesAndPredicate()
        {
            var q = CreateQueryable().Where(u => u.Name == "John" && u.Age > 25);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE (e.name = :p0 AND e.age > :p1)", model.ToIckle());
            Assert.Equal("John", model.Parameters["p0"]);
            Assert.Equal(25, model.Parameters["p1"]);
        }

        [Fact]
        public void TranslatesOrPredicate()
        {
            var q = CreateQueryable().Where(u => u.Name == "John" || u.Name == "Jane");
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE (e.name = :p0 OR e.name = :p1)", model.ToIckle());
        }

        [Fact]
        public void TranslatesCapturedVariable()
        {
            string name = "Alice";
            var q = CreateQueryable().Where(u => u.Name == name);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.name = :p0", model.ToIckle());
            Assert.Equal("Alice", model.Parameters["p0"]);
        }

        [Fact]
        public void TranslatesOrderBy()
        {
            var q = CreateQueryable().OrderBy(u => u.Name);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e ORDER BY e.name", model.ToIckle());
        }

        [Fact]
        public void TranslatesOrderByDescending()
        {
            var q = CreateQueryable().OrderByDescending(u => u.Age);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e ORDER BY e.age DESC", model.ToIckle());
        }

        [Fact]
        public void TranslatesOrderByThenBy()
        {
            var q = CreateQueryable()
                .OrderBy(u => u.Surname)
                .ThenByDescending(u => u.Age);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e ORDER BY e.surname, e.age DESC", model.ToIckle());
        }

        [Fact]
        public void TranslatesTake()
        {
            var q = CreateQueryable().Take(10);
            var model = TranslateQuery(q);
            Assert.Equal(10, model.Take);
        }

        [Fact]
        public void TranslatesSkip()
        {
            var q = CreateQueryable().Skip(5);
            var model = TranslateQuery(q);
            Assert.Equal(5L, model.Skip);
        }

        [Fact]
        public void TranslatesSkipTake()
        {
            var q = CreateQueryable().Skip(10).Take(5);
            var model = TranslateQuery(q);
            Assert.Equal(10L, model.Skip);
            Assert.Equal(5, model.Take);
        }

        [Fact]
        public void TranslatesComplexQuery()
        {
            var q = CreateQueryable()
                .Where(u => u.Age > 20)
                .OrderBy(u => u.Surname)
                .Skip(10)
                .Take(5);
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.age > :p0 ORDER BY e.surname", model.ToIckle());
            Assert.Equal(20, model.Parameters["p0"]);
            Assert.Equal(10L, model.Skip);
            Assert.Equal(5, model.Take);
        }

        [Fact]
        public void TranslatesChainedWhere()
        {
            var q = CreateQueryable()
                .Where(u => u.Age > 20)
                .Where(u => u.Name == "Bob");
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE (e.name = :p0) AND (e.age > :p1)", model.ToIckle());
        }

        [Fact]
        public void TranslatesStringContains()
        {
            var q = CreateQueryable().Where(u => u.Name.Contains("oh"));
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.name LIKE :p0", model.ToIckle());
            Assert.Equal("%oh%", model.Parameters["p0"]);
        }

        [Fact]
        public void TranslatesStringStartsWith()
        {
            var q = CreateQueryable().Where(u => u.Name.StartsWith("Jo"));
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.name LIKE :p0", model.ToIckle());
            Assert.Equal("Jo%", model.Parameters["p0"]);
        }

        [Fact]
        public void TranslatesStringEndsWith()
        {
            var q = CreateQueryable().Where(u => u.Name.EndsWith("hn"));
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE e.name LIKE :p0", model.ToIckle());
            Assert.Equal("%hn", model.Parameters["p0"]);
        }

        [Fact]
        public void TranslatesSelect()
        {
            var q = CreateQueryable().Select(u => new { u.Name, u.Surname });
            var model = IckleExpressionTranslator.Translate(q.Expression);
            Assert.Equal("SELECT e.name, e.surname FROM sample_bank_account.User e", model.ToIckle());
        }

        [Fact]
        public void TranslatesFirstTerminal()
        {
            var q = CreateQueryable().Where(u => u.Age > 30);
            var firstExpr = System.Linq.Expressions.Expression.Call(
                typeof(Queryable),
                nameof(Queryable.First),
                new[] { typeof(User) },
                q.Expression);
            var model = IckleExpressionTranslator.Translate(firstExpr);
            Assert.Equal(ResultOperator.First, model.ResultOperator);
        }

        [Fact]
        public void TranslatesCountTerminal()
        {
            var q = CreateQueryable().Where(u => u.Age > 30);
            var countExpr = System.Linq.Expressions.Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Count),
                new[] { typeof(User) },
                q.Expression);
            var model = IckleExpressionTranslator.Translate(countExpr);
            Assert.Equal(ResultOperator.Count, model.ResultOperator);
        }

        [Fact]
        public void TranslatesNotPredicate()
        {
            var q = CreateQueryable().Where(u => !(u.Age > 30));
            var model = TranslateQuery(q);
            Assert.Equal("FROM sample_bank_account.User e WHERE NOT (e.age > :p0)", model.ToIckle());
        }

        [Fact]
        public void MapsCSharpPropertyNamesToProtoFieldNames()
        {
            Assert.Equal("name", ProtobufFieldMapper.GetProtoFieldName(typeof(User), "Name"));
            Assert.Equal("surname", ProtobufFieldMapper.GetProtoFieldName(typeof(User), "Surname"));
            Assert.Equal("age", ProtobufFieldMapper.GetProtoFieldName(typeof(User), "Age"));
            Assert.Equal("id", ProtobufFieldMapper.GetProtoFieldName(typeof(User), "Id"));
        }

        [Fact]
        public void MapsProtoTypeName()
        {
            Assert.Equal("sample_bank_account.User", ProtobufFieldMapper.GetProtoTypeName(typeof(User)));
        }

        [Fact]
        public void ThrowsOnUnsupportedProperty()
        {
            Assert.Throws<NotSupportedException>(() =>
                ProtobufFieldMapper.GetProtoFieldName(typeof(User), "NonExistent"));
        }

        [Fact]
        public void ThrowsOnUnsupportedOperator()
        {
            var q = CreateQueryable();
            var expr = System.Linq.Expressions.Expression.Call(
                typeof(Queryable),
                nameof(Queryable.Distinct),
                new[] { typeof(User) },
                q.Expression);
            Assert.Throws<NotSupportedException>(() =>
                IckleExpressionTranslator.Translate(expr));
        }
    }
}
