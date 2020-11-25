using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Guidelines.RestApi.Collections.Filtering
{
    public class FilterParserTests
    {
        [Theory]
        [InlineData("Id eq 42", "(item.Id == 42)")]
        [InlineData("Id ne 42", "(item.Id != 42)")]
        [InlineData("Id gt 42", "(item.Id > 42)")]
        [InlineData("Id ge 42", "(item.Id >= 42)")]
        [InlineData("Id lt 42", "(item.Id < 42)")]
        [InlineData("Id le 42", "(item.Id <= 42)")]
        [InlineData("Value eq 42", "(item.Value == Convert(42, Nullable`1))")]
        [InlineData("Id in (42, 100)", "value(System.Collections.Generic.List`1[System.Int32]).Contains(item.Id)")]
        [InlineData("length(Title) eq 5", "(item.Title.Length == 5)")]
        [InlineData("indexof(Title, 'le') eq 5", "(item.Title.IndexOf(\"le\") == 5)")]
        [InlineData("startswith(Title, 'le')", "item.Title.StartsWith(\"le\")")]
        [InlineData("endswith(Title, 'le')", "item.Title.EndsWith(\"le\")")]
        [InlineData("contains(Title, 'le')", "item.Title.Contains(\"le\")")]
        [InlineData("not Value eq null", "Not((item.Value == null))")]
        [InlineData("Id le 42 and Id ge 30", "((item.Id <= 42) And (item.Id >= 30))")]
        [InlineData("Id le 42 and Id ge 30 and Value eq null", "(((item.Id <= 42) And (item.Id >= 30)) And (item.Value == null))")]
        [InlineData("(Id le 42 and Id ge 30) or Value eq null", "(((item.Id <= 42) And (item.Id >= 30)) Or (item.Value == null))")]
        [InlineData("Title eq 'Some Title'", "(item.Title == \"Some Title\")")]
        [InlineData("Timestamp gt 2020-10-20", "(item.Timestamp > 20/10/2020 00:00:00)")]
        [InlineData("Timestamp ge 2020-10-20T15:03:00", "(item.Timestamp >= 20/10/2020 15:03:00)")]
        public void ValidSyntaxIsAccepted(string input, string? expected = null)
        {
            // arrange
            // act
            var actual = FilterParser.Parse<Item>(input).Body.ToString();

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Id gte 42", "Syntax error (line 1, column 4): unexpected identifier `gte`.")]
        [InlineData("Id gt 42)", "Syntax error (line 1, column 9): unexpected `)`.")]
        [InlineData("Value gt 42.0f", "Syntax error (line 1, column 10): unexpected `4`.")]
        public void InvalidSyntaxIsRejected(string input, string expectedMessage)
        {
            // arrange
            // act
            var actual = Assert.Throws<ArgumentException>(() => FilterParser.Parse<Item>(input));

            // assert
            Assert.Equal(expectedMessage, actual.Message);
        }
    }
}
