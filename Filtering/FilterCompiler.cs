using System;
using System.Diagnostics.CodeAnalysis;

namespace Guidelines.RestApi.Collections.Filtering
{
    /// <summary>
    /// Helper methods to assist with construction of well-formed expressions.
    /// </summary>
    public static class FilterCompiler
    {
        /// <summary>
        /// Creates a filtering function based on the provided expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements the filter will be used for.</typeparam>
        /// <param name="expression">An expression.</param>
        /// <returns>A function that evaluates the expression in the context of an item of the type <typeparamref name="T"/>.</returns>
        public static Func<T, bool> Compile<T>(string expression)
        {
            if (!TryCompile<T>(expression, out var predicate, out var error))
            {
                throw new ArgumentException(error);
            }

            return predicate;
        }

        /// <summary>
        /// Creates a filtering function based on the provided expression.
        /// </summary>
        /// <typeparam name="T">The type of the elements the filter will be used for.</typeparam>
        /// <param name="expression">An expression.</param>
        /// <param name="predicate">A function that evaluates the expression in the context of an item of the type <typeparamref name="T"/>.</returns>
        /// <param name="error">The reported error, if compilation was unsuccessful.</param>
        /// <returns>True if the function could be created; otherwise, false.</returns>
        public static bool TryCompile<T>(string expression, [MaybeNullWhen(false)] out Func<T, bool> predicate, [MaybeNullWhen(true)] out string error)
        {
            if (!FilterParser.TryParse<T>(expression, out var filterExpression, out error))
            {
                predicate = null;
                return false;
            }

            try
            {
                predicate = filterExpression.Compile();
                error = null;
                return true;
            }
            catch (Exception err)
            {
                predicate = null;
                error = err.Message;
                return false;
            }
        }
    }
}