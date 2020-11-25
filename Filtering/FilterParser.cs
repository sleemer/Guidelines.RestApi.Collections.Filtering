using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Guidelines.RestApi.Collections.Filtering
{
    /// <summary>
    /// Helper methods to assist with parsing of well-formed expressions.
    /// </summary>
    public static class FilterParser
    {
        /// <summary>
        /// Parses a filtering function based on the provided expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements the filter will be used for.</typeparam>
        /// <param name="expression">An expression.</param>
        /// <returns>A function that evaluates the expression in the context of an item of the type <typeparamref name="T"/>.</returns>
        public static Expression<Func<T, bool>> Parse<T>(string filterExpression)
            => TryParse<T>(filterExpression, out var root, out var error) ? root : throw new ArgumentException(error);

        /// <summary>
        /// Parses a filtering function based on the provided expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements the filter will be used for.</typeparam>
        /// <param name="expression">An expression.</param>
        /// <param name="predicate">A function that evaluates the expression in the context of an item of the type <typeparamref name="T"/>.</returns>
        /// <param name="error">The reported error, if compilation was unsuccessful.</param>
        /// <returns>True if the function could be created; otherwise, false.</returns>
        public static bool TryParse<T>(
            string filterExpression,
            [MaybeNullWhen(false)] out Expression<Func<T, bool>> root,
            [MaybeNullWhen(true)] out string error)
        {
            if (filterExpression == null)
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }

            var tokens = FilterExpressionTokenizer.Instance.TryTokenize(filterExpression);
            if (!tokens.HasValue)
            {
                error = tokens.ToString();
                root = null;
                return false;
            }

            try
            {
                var result = FilterExpressionParser.TryParse<T>(tokens.Value);
                if (!result.HasValue)
                {
                    error = result.ToString();
                    root = null;
                    return false;
                }

                error = null;
                root = result.Value;
                return true;
            }
            catch (Exception err)
            {
                error = err.Message;
                root = null;
                return false;
            }
        }
    }
}
