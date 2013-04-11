using System;
using System.Linq.Expressions;

namespace Gosu.Commons.Reflection
{
    public class ExpressionParser
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> getterExpression)
        {
            return GetPropertyNameFromExpression(getterExpression.Body);
        }

        public static string GetPropertyName<T,U>(Expression<Func<T,U>> getterExpression)
        {
            return GetPropertyNameFromExpression(getterExpression.Body);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> getterExpression)
        {
            return GetPropertyNameFromExpression(getterExpression.Body);
        }

        private static string GetPropertyNameFromExpression(Expression getterExpression)
        {
            if (getterExpression is UnaryExpression)
                return GetPropertyNameFromExpression(((UnaryExpression)getterExpression).Operand);

            var expression = getterExpression as MemberExpression;

            if (expression == null)
                throw new InvalidOperationException("Can not get member name from the given expression");

            return expression.Member.Name;
        }
    }
}