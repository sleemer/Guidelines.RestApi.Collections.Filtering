using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Guidelines.RestApi.Collections.Filtering
{
    public class FilterParserTests
    {
        [Theory]
        [InlineData("Id eq 42")]
        [InlineData("Id ne 42")]
        [InlineData("Id gt 42")]
        [InlineData("Id ge 42")]
        [InlineData("Id lt 42")]
        [InlineData("Id le 42")]
        [InlineData("Value eq 42")]
        [InlineData("Id in (42, 100)")]
        [InlineData("length(Title) eq 5")]
        [InlineData("indexof(Title, 'le') eq 5")]
        [InlineData("startswith(Title, 'le')")]
        [InlineData("endswith(Title, 'le')")]
        [InlineData("contains(Title, 'le')")]
        [InlineData("not Value eq null")]
        [InlineData("Id le 42 and Id ge 30")]
        [InlineData("Id le 42 and Id ge 30 and Value eq null")]
        [InlineData("(Id le 42 and Id ge 30) or Value eq null")]
        [InlineData("Title eq 'Some Title'")]
        [InlineData("Timestamp gt 2020-10-20")]
        [InlineData("Timestamp ge 2020-10-20T15:03:00")]
        public void ValidSyntaxIsAccepted(string input)
        {
            // arrange
            // act
            var actual = FilterParser.Parse<Item>(input);

            // assert
            Assert.NotNull(actual);
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
