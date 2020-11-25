using System;
using System.Linq.Expressions;

namespace Guidelines.RestApi.Collections.Filtering
{
    internal static class ComparisonExpression
    {
        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
        {
            if (left.IsNullOrNullable() && !right.IsNullOrNullable())
            {
                right = Expression.Convert(right, left.Type);
            }
            else if (!left.IsNullOrNullable() && right.IsNullOrNullable())
            {
                left = Expression.Convert(left, right.Type);
            }

            return Expression.MakeBinary(binaryType, left, right);
        }

        private static bool IsNullOrNullable(this Expression expression) 
            => (expression is ConstantExpression constantExpression && constantExpression.Value == null)
            || (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }
}