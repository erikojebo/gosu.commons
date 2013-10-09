using System;
using System.Linq.Expressions;

namespace Gosu.Commons.Reflection
{
    public class PropertyName
    {
        public static string For<T>(Expression<Func<T, object>> getterExpression)
        {
            return GetPropertyNameFromExpression(getterExpression.Body);
        }

        public static string For<T, U>(Expression<Func<T, U>> getterExpression)
        {
            return GetPropertyNameFromExpression(getterExpression.Body);
        }

        public static string For<T>(Expression<Func<T>> getterExpression)
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