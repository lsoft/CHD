using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common
{
    public static class Helper
    {
        public static T[] Append<T>(this T[] a, T[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            var result = new T[a.Length + b.Length];

            Array.Copy(a, 0, result, 0, a.Length);
            Array.Copy(b, 0, result, a.Length, b.Length);

            return
                result;
        }

        public static T[] SubArray<T>(this T[] a, int startIndex, int length)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            var result = new T[length];

            Array.Copy(a, startIndex, result, 0, length);

            return
                result;
        }

        public static T[] SubArray<T>(this T[] a, int startIndex)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            var result = new T[a.Length - startIndex];

            Array.Copy(a, startIndex, result, 0, result.Length);
            
            return
                result;
        }

        public static bool Contains(this string t, bool ignoreCase, params string[] collection)
        {
            return
                !Helper.NotContains(t, ignoreCase, collection);
        }

        public static bool NotContains(this string t, bool ignoreCase, params string[] collection)
        {
            var tl = ignoreCase ? t.ToLower() : t;

            foreach (var c in collection)
            {
                var cl = ignoreCase ? c.ToLower() : c;

                if (tl.Contains(cl))
                {
                    return
                        false;
                }
            }

            return
                true;
        }


        public static bool NotIn<T>(this T t, params T[] collection) 
        {
            return
                !collection.Contains(t);
        }

        public static bool In<T>(this T t, params T[] collection)
        {
            return
                collection.Contains(t);
        }

        public static T ExtractFirst<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return
                    default(T);
            }

            var result = list[0];
            
            list.RemoveAt(0);

            return
                result;
        }

    }
}
