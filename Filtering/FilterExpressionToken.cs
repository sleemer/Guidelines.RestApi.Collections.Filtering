using Superpower.Display;
using Superpower.Parsers;

namespace Guidelines.RestApi.Collections.Filtering
{
    internal enum FilterExpressionToken
    {
        None,

        Comma,

        Identifier,

        Integer,

        Decimal,

        String,

        DateTime,

        [Token(Category = "keyword", Example = "true")]
        True,

        [Token(Category = "keyword", Example = "false")]
        False,

        [Token(Category = "keyword", Example = "null")]
        Null,

        [Token(Category = "comparison operator", Example = "eq")]
        Equal,

        [Token(Category = "comparison operator", Example = "ne")]
        NotEqual,

        [Token(Category = "comparison operator", Example = "gt")]
        GraterThan,

        [Token(Category = "comparison operator", Example = "ge")]
        GraterThanOrEqual,

        [Token(Category = "comparison operator", Example = "lt")]
        LessThan,

        [Token(Category = "comparison operator", Example = "le")]
        LessThanOrEqual,

        [Token(Category = "comparison operator", Example = "in")]
        In,

        [Token(Category = "logical operator", Example = "and")]
        And,

        [Token(Category = "logical operator", Example = "or")]
        Or,

        [Token(Category = "logical operator", Example = "not")]
        Not,

        [Token(Category = "string function", Example = "contains")]
        Contains,

        [Token(Category = "string function", Example = "startswith")]
        StartsWith,

        [Token(Category = "string function", Example = "endswith")]
        EndsWith,

        [Token(Category = "string function", Example = "indexof")]
        IndexOf,

        [Token(Category = "string property", Example = "length")]
        Length,

        [Token(Example = "(")]
        LParen,

        [Token(Example = ")")]
        RParen
    }
}
