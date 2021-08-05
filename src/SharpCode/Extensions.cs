using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCode
{
    internal static class Extensions
    {
        public static bool AtLeast<T>(this IEnumerable<T> source, int count) =>
            source.Take(count).Count() == count;

        public static TResult[] SelectToArray<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) =>
            source.Select(selector).ToArray();
    }
}
