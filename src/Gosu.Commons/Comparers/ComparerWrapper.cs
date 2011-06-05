using System.Collections;
using System.Collections.Generic;

namespace Gosu.Commons.Comparers
{
    public class ComparerWrapper<T> : IEqualityComparer
    {
        private readonly IEqualityComparer<T> _comparer;

        public ComparerWrapper(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public new bool Equals(object x, object y)
        {
            return _comparer.Equals((T)x, (T)y);
        }

        public int GetHashCode(object obj)
        {
            return _comparer.GetHashCode((T)obj);
        }
    }
}