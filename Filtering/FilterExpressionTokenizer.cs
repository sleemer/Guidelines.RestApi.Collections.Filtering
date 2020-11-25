using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Guidelines.RestApi.Collections.Filtering
{
    internal static class FilterExpressionTokenizer
    {
        private static class ComparisonOperator
        {
            public static TextParser<TextSpan> Eq { get; } = Span.EqualTo("eq");
            public static TextParser<TextSpan> Ne { get; } = Span.EqualTo("ne");
            public static TextParser<TextSpan> Gt { get; } = Span.EqualTo("gt");
            public static TextParser<TextSpan> Ge { get; } = Span.EqualTo("ge");
            public static TextParser<TextSpan> Lt { get; } = Span.EqualTo("lt");
            public static TextParser<TextSpan> Le { get; } = Span.EqualTo("le");
            public static TextParser<TextSpan> In { get; } = Span.EqualTo("in");
        }

        private static class LogicalOperator
        {
            public static TextParser<TextSpan> And { get; } = Span.EqualTo("and");
            public static TextParser<TextSpan> Or { get; } = Span.EqualTo("or");
            public static TextParser<TextSpan> Not { get; } = Span.EqualTo("not");
        }

        private static class StringFunction
        {
            public static TextParser<TextSpan> Contains { get; } = Span.EqualTo("contains");
            public static TextParser<TextSpan> StartsWith { get; } = Span.EqualTo("startswith");
            public static TextParser<TextSpan> EndsWith { get; } = Span.EqualTo("endswith");
            public static TextParser<TextSpan> IndexOf { get; } = Span.EqualTo("indexof");
            public static TextParser<TextSpan> Length { get; } = Span.EqualTo("length");
        }

        private static class Boolean
        {
            public static TextParser<TextSpan> True { get; } = Span.EqualTo("true");
            public static TextParser<TextSpan> False { get; } = Span.EqualTo("false");
        }

        private static class Iso8601
        {
            public static TextParser<TextSpan> DateTime { get; } = Span.Regex("\\d{4}-\\d\\d-\\d\\d(T\\d\\d:\\d\\d:\\d\\d)?(\\.\\d+)?(([+-]\\d\\d:\\d\\d)|Z)?");
        }

        private static class Keywords
        {
            public static TextParser<TextSpan> Null { get; } = Span.EqualTo("null");
        }

        public static Tokenizer<FilterExpressionToken> Instance { get; } =
            new TokenizerBuilder<FilterExpressionToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('('), FilterExpressionToken.LParen)
                .Match(Character.EqualTo(')'), FilterExpressionToken.RParen)
                .Match(Character.EqualTo(','), FilterExpressionToken.Comma)
                .Match(ComparisonOperator.Eq, FilterExpressionToken.Equal, requireDelimiters: true)
                .Match(ComparisonOperator.Ne, FilterExpressionToken.NotEqual, requireDelimiters: true)
                .Match(ComparisonOperator.Gt, FilterExpressionToken.GraterThan, requireDelimiters: true)
                .Match(ComparisonOperator.Ge, FilterExpressionToken.GraterThanOrEqual, requireDelimiters: true)
                .Match(ComparisonOperator.Lt, FilterExpressionToken.LessThan, requireDelimiters: true)
                .Match(ComparisonOperator.Le, FilterExpressionToken.LessThanOrEqual, requireDelimiters: true)
                .Match(ComparisonOperator.In, FilterExpressionToken.In, requireDelimiters: true)
                .Match(LogicalOperator.And, FilterExpressionToken.And, requireDelimiters: true)
                .Match(LogicalOperator.Or, FilterExpressionToken.Or, requireDelimiters: true)
                .Match(LogicalOperator.Not, FilterExpressionToken.Not, requireDelimiters: true)
                .Match(StringFunction.Contains, FilterExpressionToken.Contains, requireDelimiters: true)
                .Match(StringFunction.StartsWith, FilterExpressionToken.StartsWith, requireDelimiters: true)
                .Match(StringFunction.EndsWith, FilterExpressionToken.EndsWith, requireDelimiters: true)
                .Match(StringFunction.Length, FilterExpressionToken.Length, requireDelimiters: true)
                .Match(StringFunction.IndexOf, FilterExpressionToken.IndexOf, requireDelimiters: true)
                .Match(Iso8601.DateTime, FilterExpressionToken.DateTime, requireDelimiters: true)
                .Match(Numerics.Integer, FilterExpressionToken.Integer, requireDelimiters: true)
                .Match(Numerics.Decimal, FilterExpressionToken.Decimal, requireDelimiters: true)
                .Match(Boolean.True, FilterExpressionToken.True, requireDelimiters: true)
                .Match(Boolean.False, FilterExpressionToken.False, requireDelimiters: true)
                .Match(QuotedString.SqlStyle, FilterExpressionToken.String, requireDelimiters: true)
                .Match(Keywords.Null, FilterExpressionToken.Null, requireDelimiters: true)
                .Match(Identifier.CStyle, FilterExpressionToken.Identifier, requireDelimiters: true)
                .Build();
    }
}
