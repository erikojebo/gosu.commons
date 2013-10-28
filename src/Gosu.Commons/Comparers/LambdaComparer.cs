using System;
using System.Collections.Generic;
using System.Linq;
using Gosu.Commons.Extensions;

namespace Gosu.Commons.Comparers
{
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object>[] _valuesToCompare;

        public LambdaComparer(params Func<T, object>[] valuesToCompare)
        {
            _valuesToCompare = valuesToCompare;
        }

        public bool Equals(T x, T y)
        {
            AssertGettersAreSpecified();

            foreach (var func in _valuesToCompare)
            {
                var leftValue = func(x);
                var rightValue = func(y);

                if (!Equals(leftValue, rightValue))
                    return false;
            }

            return true;
        }

        public int GetHashCode(T obj)
        {
            AssertGettersAreSpecified();

            return _valuesToCompare.First()(obj).GetHashCode();
        }

        private void AssertGettersAreSpecified()
        {
            if (_valuesToCompare.IsEmpty())
                throw new InvalidOperationException();
        }
    }
}