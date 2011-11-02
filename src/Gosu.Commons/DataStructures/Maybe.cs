using System;

namespace Gosu.Commons.DataStructures
{
    public class Maybe<T>
    {
        private Maybe() {}

        public static Maybe<T> Nothing = new Maybe<T>();

        public static Maybe<T> Unit(T value)
        {
            return new Maybe<T> { Value = value };
        }

        public bool IsNothing
        {
            get { return this == Nothing; }
        }

        public bool HasValue
        {
            get { return !IsNothing; }
        }

        private T _value;
        public T Value
        {
            get
            {
                if (IsNothing)
                {
                    throw new InvalidOperationException("Tried to access the value of a Maybe instance that was Nothing");
                }

                return _value;
            }
            private set { _value = value; }
        }
    }
}