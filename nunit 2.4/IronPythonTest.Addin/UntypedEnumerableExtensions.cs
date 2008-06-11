using System.Collections;
using System.Collections.Generic;

namespace IronPythonTest.Addin
{
    public static class UntypedEnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable enumerable)
        {
            foreach (object item in enumerable) yield return (T) item;
        }

        public static List<T> ToList<T>(this IEnumerable enumerable)
        {
            return new List<T>(enumerable.ToEnumerable<T>());
        }
    }
}