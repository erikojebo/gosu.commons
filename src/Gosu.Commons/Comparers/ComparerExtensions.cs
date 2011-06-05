using System.Collections;
using System.Collections.Generic;

namespace Gosu.Commons.Comparers
{
    public static class ComparerExtensions
    {
        public static IEqualityComparer AsEqualityComparer<T>(this IEqualityComparer<T> comparer)
        {
            return new ComparerWrapper<T>(comparer);
        }
    }
}