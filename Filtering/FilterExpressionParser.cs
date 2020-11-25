using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Guidelines.RestApi.Collections.Filtering
{
    internal static class FilterExpressionParser
    {
        private static class ComparisonOperator
        {
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> Equal =
                Token.EqualTo(FilterExpressionToken.Equal).Value(ExpressionType.Equal);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> NotEqual =
                Token.EqualTo(FilterExpressionToken.NotEqual).Value(ExpressionType.NotEqual);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> GreaterThan =
                Token.EqualTo(FilterExpressionToken.GraterThan).Value(ExpressionType.GreaterThan);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> GreaterThanOrEqual =
                Token.EqualTo(FilterExpressionToken.GraterThanOrEqual).Value(ExpressionType.GreaterThanOrEqual);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> LessThan =
                Token.EqualTo(FilterExpressionToken.LessThan).Value(ExpressionType.LessThan);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> LessThanOrEqual =
                Token.EqualTo(FilterExpressionToken.LessThanOrEqual).Value(ExpressionType.LessThanOrEqual);

            public static TokenListParser<FilterExpressionToken, Expression> In(Expression obj) =>
                from prop in Property(obj)
                from contains in Token.EqualTo(FilterExpressionToken.In)
                from list in Constant
                select (Expression)Expression.Call(
                    list, 
                    list.Type.GetMethod("Contains") ?? throw new InvalidOperationException($"Method Contains not found for type {list.Type}."), 
                    prop);
        }

        private static class LogicalOperator
        {
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> And =
                Token.EqualTo(FilterExpressionToken.And).Value(ExpressionType.And);
            public static readonly TokenListParser<FilterExpressionToken, ExpressionType> Or =
                Token.EqualTo(FilterExpressionToken.Or).Value(ExpressionType.Or);

            public static TokenListParser<FilterExpressionToken, Expression> Not(TokenListParser<FilterExpressionToken, Expression> operandParser) =>
                from negate in Token.EqualTo(FilterExpressionToken.Not)
                from operand in operandParser
                select (Expression)Expression.Not(operand);
        }

        private static class List
        {
            public static TokenListParser<FilterExpressionToken, ConstantExpression> Strings =
                Token.EqualTo(FilterExpressionToken.String)
                    .Apply(QuotedString.SqlStyle)
                    .AtLeastOnceDelimitedBy(Token.EqualTo(FilterExpressionToken.Comma))
                    .Select(strings => Expression.Constant(new List<string>(strings), typeof(List<string>)));

            public static TokenListParser<FilterExpressionToken, ConstantExpression> Integers =
                Token.EqualTo(FilterExpressionToken.Integer)
                    .Apply(Numerics.IntegerInt32)
                    .AtLeastOnceDelimitedBy(Token.EqualTo(FilterExpressionToken.Comma))
                    .Select(nums => Expression.Constant(new List<int>(nums), typeof(List<int>)));

            public static TokenListParser<FilterExpressionToken, ConstantExpression> Decimals =
                Token.EqualTo(FilterExpressionToken.Decimal)
                    .Apply(Numerics.DecimalDecimal)
                    .AtLeastOnceDelimitedBy(Token.EqualTo(FilterExpressionToken.Comma))
                    .Select(nums => Expression.Constant(new List<decimal>(nums), typeof(List<decimal>)));

            public static TokenListParser<FilterExpressionToken, ConstantExpression> DateTimes =
                Token.EqualTo(FilterExpressionToken.DateTime)
                    .Select(span => DateTime.Parse(span.ToStringValue()))
                    .AtLeastOnceDelimitedBy(Token.EqualTo(FilterExpressionToken.Comma))
                    .Select(dateTimes => Expression.Constant(new List<DateTime>(dateTimes), typeof(List<DateTime>)));
        }

        private static class Strings
        {
            private static readonly IDictionary<FilterExpressionToken, string> s_stringFunctions = new Dictionary<FilterExpressionToken, string>
            {
                [FilterExpressionToken.StartsWith] = "StartsWith",
                [FilterExpressionToken.EndsWith] = "EndsWith",
                [FilterExpressionToken.Contains] = "Contains",
                [FilterExpressionToken.IndexOf] = "IndexOf",
            };

            private static TokenListParser<FilterExpressionToken, Expression> StringFunction(Expression obj, FilterExpressionToken func) =>
                from methodName in Token.EqualTo(func).Value(s_stringFunctions[func])
                from lparen in Token.EqualTo(FilterExpressionToken.LParen)
                from prop in Property(obj)
                from constant in Token.EqualTo(FilterExpressionToken.Comma).IgnoreThen(Constant)
                from rparen in Token.EqualTo(FilterExpressionToken.RParen)
                select (Expression)Expression.Call(prop, typeof(string).GetMethod(methodName, new[] { typeof(string) })!, constant);

            public static TokenListParser<FilterExpressionToken, Expression> StartsWith(Expression obj)
                => StringFunction(obj, FilterExpressionToken.StartsWith);

            public static TokenListParser<FilterExpressionToken, Expression> EndsWith(Expression obj)
                => StringFunction(obj, FilterExpressionToken.EndsWith);

            public static TokenListParser<FilterExpressionToken, Expression> Contains(Expression obj)
                => StringFunction(obj, FilterExpressionToken.Contains);
            
            public static TokenListParser<FilterExpressionToken, Expression> IndexOf(Expression obj)
                => StringFunction(obj, FilterExpressionToken.IndexOf);

            public static TokenListParser<FilterExpressionToken, Expression> Length(Expression obj) =>
                from length in Token.EqualTo(FilterExpressionToken.Length)
                from lparen in Token.EqualTo(FilterExpressionToken.LParen)
                from prop in Property(obj)
                from rparen in Token.EqualTo(FilterExpressionToken.RParen)
                select (Expression)Expression.Property(prop, "Length");
        }

        private static TokenListParser<FilterExpressionToken, ConstantExpression> Constant =
            (from lparen in Token.EqualTo(FilterExpressionToken.LParen)
             from elements in List.Integers.Or(List.Strings).Or(List.Decimals).Or(List.DateTimes)
             from rparen in Token.EqualTo(FilterExpressionToken.RParen)
             select elements)
            .Or(Token.EqualTo(FilterExpressionToken.String)
                .Apply(QuotedString.SqlStyle)
                .Select(str => Expression.Constant(str, typeof(string))))
            .Or(Token.EqualTo(FilterExpressionToken.Integer)
                .Apply(Numerics.IntegerInt32)
                .Select(num => Expression.Constant(num, typeof(int))))
            .Or(Token.EqualTo(FilterExpressionToken.Decimal)
                .Apply(Numerics.DecimalDecimal)
                .Select(num => Expression.Constant(num, typeof(decimal))))
            .Or(Token.EqualTo(FilterExpressionToken.DateTime)
                .Select(dateTime => Expression.Constant(DateTime.Parse(dateTime.ToStringValue()), typeof(DateTime))))
            .Or(Token.EqualTo(FilterExpressionToken.True).Value(Expression.Constant(true, typeof(bool))))
            .Or(Token.EqualTo(FilterExpressionToken.False).Value(Expression.Constant(false, typeof(bool))))
            .Or(Token.EqualTo(FilterExpressionToken.Null).Value(Expression.Constant(null)));

        private static TokenListParser<FilterExpressionToken, Expression> Factor(Expression obj) =>
            (from lparen in Token.EqualTo(FilterExpressionToken.LParen)
             from expr in Parse.Ref(() => Expr(obj))
             from rparen in Token.EqualTo(FilterExpressionToken.RParen)
             select expr)
            .Or(Constant.Select(constant => (Expression)constant))
            .Or(Operand(obj));

        private static TokenListParser<FilterExpressionToken, Expression> Comparison(Expression obj) =>
            ComparisonOperator.In(obj)
                .Try()
                .Or(Parse.Chain(
                    ComparisonOperator.Equal
                        .Or(ComparisonOperator.NotEqual)
                        .Or(ComparisonOperator.GreaterThan)
                        .Or(ComparisonOperator.GreaterThanOrEqual)
                        .Or(ComparisonOperator.LessThan)
                        .Or(ComparisonOperator.LessThanOrEqual),
                    Factor(obj),
                    ComparisonExpression.MakeBinary));

        private static TokenListParser<FilterExpressionToken, Expression> Operation(Expression obj) =>
            Comparison(obj)
                .Or(Strings.StartsWith(obj))
                .Or(Strings.EndsWith(obj))
                .Or(Strings.Contains(obj));

        private static TokenListParser<FilterExpressionToken, Expression> Property(Expression obj) =>
            Token.EqualTo(FilterExpressionToken.Identifier)
                .Select(propName => (Expression)Expression.Property(obj, propName.ToStringValue()));

        private static TokenListParser<FilterExpressionToken, Expression> Operand(Expression obj) =>
            Property(obj)
                .Or(Strings.Length(obj))
                .Or(Strings.IndexOf(obj));

        private static TokenListParser<FilterExpressionToken, Expression> Conjunction(Expression obj) => Parse.Chain(
            LogicalOperator.And,
            Operation(obj).Or(LogicalOperator.Not(Operation(obj))),
            Expression.MakeBinary);

        private static TokenListParser<FilterExpressionToken, Expression> Disjunction(Expression obj) => Parse.Chain(
            LogicalOperator.Or,
            Conjunction(obj),
            Expression.MakeBinary);

        private static TokenListParser<FilterExpressionToken, Expression> Expr(Expression obj) => Disjunction(obj);

        private static TokenListParser<FilterExpressionToken, Expression<Func<T, bool>>> Lambda<T>()
        {
            var obj = Expression.Parameter(typeof(T), "item");
            return Expr(obj).AtEnd().Select(body => Expression.Lambda<Func<T, bool>>(body, false, obj));
        }

        public static TokenListParserResult<FilterExpressionToken, Expression<Func<T, bool>>> TryParse<T>(TokenList<FilterExpressionToken> tokens)
            => Lambda<T>().TryParse(tokens);
    }
}
