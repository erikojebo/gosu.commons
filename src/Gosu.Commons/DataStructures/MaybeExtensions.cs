using System;

namespace Gosu.Commons.DataStructures
{
    public static class MaybeExtensions
    {
        public static Func<T, Maybe<U>> Lift<T, U>(this Func<T, U> f)
        {
            return x => Maybe<U>.Unit(f(x));
        }

        public static Maybe<U> Bind<T, U>(this Maybe<T> x, Func<T, Maybe<U>> f)
        {
            return x.IsNothing ? Maybe<U>.Nothing : f(x.Value);
        }

        public static Maybe<U> SelectMany<T, U>(this Maybe<T> x, Func<T, Maybe<U>> f)
        {
            return Bind(x, f);
        }

        public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> x, Func<T, Maybe<U>> k, Func<T, U, V> s)
        {
            return s(x.Value, k(x.Value).Value).ToMaybe();
        }

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return Maybe<T>.Unit(value);
        }
    }
}