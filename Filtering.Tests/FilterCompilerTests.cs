using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Guidelines.RestApi.Collections.Filtering
{
    public sealed class FilterCompilerTests
    {
        [Theory]
        [ClassData(typeof(CorrectInputTestData))]
        public void ShouldCompileWhenInputIsCorrect(string input, Item _, bool __)
        {
            // arrange
            // act
            var actual = FilterCompiler.Compile<Item>(input);

            // assert
            Assert.NotNull(actual);
        }

        [Theory]
        [ClassData(typeof(CorrectInputTestData))]
        public void ShouldTryCompileWhenInputIsCorrect(string input, Item _, bool __)
        {
            // arrange
            // act
            var actual = FilterCompiler.TryCompile<Item>(input, out var acutalFilter, out var actualError);
            
            // assert
            Assert.True(actual);
            Assert.NotNull(acutalFilter);
            Assert.Null(actualError);
        }

        [Theory]
        [ClassData(typeof(InvalidInputTestData))]
        public void TryCompileShouldReturnErrorWhenInputIsInvalid(string input, Item _, bool __)
        {
            // arrange
            // act
            var actual = FilterCompiler.TryCompile<Item>(input, out var acutalFilter, out var actualError);
            
            // assert
            Assert.False(actual);
            Assert.Null(acutalFilter);
            Assert.NotNull(actualError);
        }

        [Theory]
        [ClassData(typeof(CorrectInputTestData))]
        public void ShouldFilterWhenInputIsCorrectStatement(string input, Item item, bool expected)
        {
            // arrange
            var filterExpression = FilterParser.Parse<Item>(input);
            var filter = filterExpression.Compile();

            // act
            var actual = filter(item);

            // assert
            Assert.Equal(expected, actual);
        }

        private sealed class CorrectInputTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "Id eq 5", new Item { Id = 5 }, true };
                yield return new object[] { "not Id eq 5", new Item { Id = 5 }, false };
                yield return new object[] { "Value eq 5.0", new Item { Value = 5 }, true };
                yield return new object[] { "Value eq 5", new Item { Value = 5 }, true };
                yield return new object[] { "Value eq null", new Item { Value = 5 }, false };
                yield return new object[] { "Value eq null", new Item { }, true };
                yield return new object[] { "IsEnabled eq true", new Item { IsEnabled = true }, true };
                yield return new object[] { "not IsEnabled eq false", new Item { IsEnabled = true }, true };
                yield return new object[] { "Timestamp eq 2019-01-29", new Item { Timestamp = new DateTime(2019, 1, 29) }, true };
                yield return new object[] { "Timestamp gt 2019-01-29T01:00:00", new Item { Timestamp = new DateTime(2019, 1, 29, 1, 30, 40) }, true };
                yield return new object[] { "Title eq 'Some Title'", new Item { Title = "Some Title" }, true };
                yield return new object[] { "contains(Title, 'Title')", new Item { Title = "Some Title" }, true };
                yield return new object[] { "endswith(Title, 'Title')", new Item { Title = "Some Title" }, true };
                yield return new object[] { "startswith(Title, 'Some')", new Item { Title = "Some Title" }, true };
                yield return new object[] { "length(Title) eq 10", new Item { Title = "Some Title" }, true };
                yield return new object[] { "Title in ('Some', 'Title')", new Item { Title = "Title" }, true };
                yield return new object[] { "not indexof(Title, 'Tit') eq -1", new Item { Title = "Some Title" }, true };
                yield return new object[] { "not indexof(Title, 'Tit') eq -1 and Value gt 4", new Item { Title = "Some Title", Value = 5 }, true };
                yield return new object[] { "(length(Title) gt 5 and contains(Title, 'tle')) or Value in (3.0,5.0)", new Item { Title = "Some Title", Value = 3 }, true };
                yield return new object[] { "not indexof(Title, 'Tit') eq -1 or Value le 4", new Item { Title = "Some Title", Value = 5 }, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        public sealed class InvalidInputTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "Id eqo 5", new Item { Id = 5 }, true };
                yield return new object[] { "not Id eq 2019-01-29", new Item { Id = 5 }, false };
                yield return new object[] { "NotExistingProperty eq 5.0", new Item { Value = 5 }, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}