namespace Gosu.Commons.Comparers
{
    public class PropertyComparer
    {
        public bool Equals<T>(T left, object right)
        {
            if (right == null || !typeof(T).IsAssignableFrom(right.GetType()))
            {
                return false;
            }

            var properties = typeof(T).GetProperties();

            foreach (var propertyInfo in properties)
            {
                var leftValue = propertyInfo.GetValue(left, null);
                var rightValue = propertyInfo.GetValue(right, null);

                if (!object.Equals(leftValue, rightValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}