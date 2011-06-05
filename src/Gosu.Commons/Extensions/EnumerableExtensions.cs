using System.Collections.Generic;
using System.Linq;

namespace Gosu.Commons.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

        public static IEnumerable<T> AsList<T>(this T obj)
        {
            return new List<T> { obj };
        }

        public static void ResetTo<T>(this IList<T> original, IEnumerable<T> newItems)
        {
            original.Clear();

            foreach (var newItem in newItems)
            {
                original.Add(newItem);
            }
        }

    }
}