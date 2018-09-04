using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CHD.Common.Others
{
    public static class Helper
    {
        public static bool Contains(this string t, bool ignoreCase, params char[] collection)
        {
            return
                !Helper.NotContains(t, ignoreCase, collection);
        }

        public static bool NotContains(this string t, bool ignoreCase, params char[] collection)
        {
            var tl = ignoreCase ? t.ToLower() : t;

            foreach (var c in collection)
            {
                var cl = ignoreCase ? char.ToLower(c) : c;

                if (tl.Contains(cl))
                {
                    return
                        false;
                }
            }

            return
                true;
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

        public static TSource Last<TSource>(
            this IReadOnlyList<TSource> source,
            int fromTailIndex
            )
        {
            return
                source.Skip(source.Count - Math.Abs(fromTailIndex) - 1).FirstOrDefault();
        }


        public static IEnumerable<TSource> TakeWithoutTail<TSource>(
            this IReadOnlyList<TSource> source,
            int tailCount
            )
        {
            return
                source.Take(source.Count - tailCount);
        }

        public static long CopyToConstraint(this Stream source, Stream destination, long bytes)
        {
            var writed = 0L;

            byte[] buffer = new byte[81920];
            int min = (int)Math.Min(buffer.Length, bytes);

            int read;
            while (bytes > 0L && (read = source.Read(buffer, 0, min)) > 0)
            {
                destination.Write(buffer, 0, read);
                bytes -= read;

                writed += read;
            }

            return
                writed;
        }

        public static string Beautify(this XmlDocument doc)
        {
            var sb = new StringBuilder();

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }

            return sb.ToString();
        }

        public static void GroupTransform<T>(
            this IEnumerable<T> values,
            Action<T, T> action
            )
        {
            T f = default(T);
            T s = default(T);

            var enumerator = values.GetEnumerator();
            using (enumerator)
            {
                if (!enumerator.MoveNext())
                {
                    return;
                }

                s = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    f = s;
                    s = enumerator.Current;

                    action(f, s);
                }
            }
        }

        public static bool NotIn<T>(
            this T t,
            params T[] collection
            )
        {
            return
                !collection.Contains(t);
        }

        public static bool In<T>(this T t, params T[] collection)
        {
            return
                collection.Contains(t);
        }

        public static T[] CloneArray<T>(this T[] a)
        {
            if (a == null)
            {
                return null;
            }

            var result = new T[a.Length];

            for (var cc = 0; cc < a.Length; cc++)
            {
                result[cc] = a[cc];
            }

            return result;
        }

        public static void ForEach<T>(
            this IEnumerable<T> values,
            Action<T> action
            )
        {
            foreach (var v in values)
            {
                action(v);
            }
        }


        public static string Join<T>(
            this IEnumerable<T> values,
            string separator,
            Func<T, string> converter
            )
        {
            var sb = new StringBuilder();

            bool first = true;
            foreach (var v in values)
            {
                if (first)
                {
                    sb.Append(separator);

                    first = false;
                }

                var s = converter(v);

                sb.Append(s);
            }

            return 
                sb.ToString();
        }

        public static T[] Concatenate<T>(this IReadOnlyList<T[]> b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            var bTotalLength = b.Sum(j => j.Length);

            var r = new T[bTotalLength];

            var index = 0;
            foreach (var bi in b)
            {
                Array.Copy(bi, 0, r, index, bi.Length);

                index += bi.Length;
            }

            return
                r;
        }

        public static T[] Concatenate<T>(this T[] a, T[] b)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }

            var r = new T[a.Length + b.Length];
            Array.Copy(a, 0, r, 0, a.Length);
            Array.Copy(b, 0, r, a.Length, b.Length);

            return
                r;
        }

        public static string ArrgegateToString(
            this IEnumerable<byte> array,
            string separator = ""
            )
        {
            var result = string.Join(
                separator,
                array.Select(j => j.ToString("X2")).ToArray()
                );

            return result;
        }

        public static bool ArrayEquals<T>(
            this IEnumerable<T> buffer0,
            IEnumerable<T> buffer1,
            IComparer<T> comparator = null
            )
            where T : IComparable
        {
            if (buffer0 == null && buffer1 == null)
            {
                return
                    true;
            }
            if (buffer0 != null && buffer1 == null)
            {
                return
                    false;
            }
            if (buffer0 == null && buffer1 != null)
            {
                return
                    false;
            }

            if (comparator == null)
            {
                comparator = Comparer<T>.Default;
            }


            var enumerator0 = buffer0.GetEnumerator();
            if (!enumerator0.MoveNext())
            {
                //коллекция пуста
                return false;
            }

            using (enumerator0)
            {
                var enumerator1 = buffer1.GetEnumerator();
                if (!enumerator1.MoveNext())
                {
                    //коллекция пуста
                    return false;
                }

                using (enumerator1)
                {
                    while (true)
                    {
                        var b0 = enumerator0.Current;
                        var b1 = enumerator1.Current;

                        if (comparator.Compare(b0, b1) != 0)
                        //if (b0.CompareTo(b1) != 0)
                        {
                            //не совпадают
                            return false;
                        }

                        var c0 = enumerator0.MoveNext();
                        var c1 = enumerator1.MoveNext();

                        if (!c0 && !c1)
                        {
                            //обе коллекции закончились
                            //это успех
                            return true;
                        }

                        if (!c0 || !c1)
                        {
                            //какая то коллекция закончилась
                            //то есть у них разный размер
                            //это фейл

                            return false;
                        }

                        //ни одна из коллекций не закончилась
                        //продолжаем сравнение
                    }
                }
            }
        }

        public static bool ArrayEquals<T>(
            IEnumerable<T> buffer0,
            IEnumerable<T> buffer1,
            long elementCheckCount,
            IComparer<T> comparator = null
            )
            where T : IComparable
        {
            if (buffer0 == null && buffer1 == null)
            {
                return
                    true;
            }
            if (buffer0 != null && buffer1 == null)
            {
                return
                    false;
            }
            if (buffer0 == null && buffer1 != null)
            {
                return
                    false;
            }


            if (comparator == null)
            {
                comparator = Comparer<T>.Default;
            }


            if (elementCheckCount == 0)
            {
                //надо проверить 0 пар; проверили, все ок :)
                return true;
            }

            var enumerator0 = buffer0.GetEnumerator();
            if (!enumerator0.MoveNext())
            {
                //коллекция пуста
                return false;
            }

            using (enumerator0)
            {
                var enumerator1 = buffer1.GetEnumerator();
                if (!enumerator1.MoveNext())
                {
                    //коллекция пуста
                    return false;
                }

                using (enumerator1)
                {
                    var cnt = 0;

                    while (true)
                    {
                        var b0 = enumerator0.Current;
                        var b1 = enumerator1.Current;

                        if(comparator.Compare(b0, b0) != 0)
                        //if (b0.CompareTo(b1) != 0)
                        {
                            //не совпадают
                            return false;
                        }

                        cnt++;

                        if (cnt == elementCheckCount)
                        {
                            //проверили нужное количество элементов
                            return true;
                        }

                        var c0 = enumerator0.MoveNext();
                        var c1 = enumerator1.MoveNext();

                        if (!c0 || !c1)
                        {
                            //какая то коллекция закончилась
                            //т.е. в любом случае завершаем процедуру сравнения
                            //надо только определить что возвращать
                            if (cnt < elementCheckCount)
                            {
                                //одна из коллекций оказалась меньшего размера, чем надо проверять
                                return false;
                            }

                            return true;
                        }

                        //ни одна из коллекций не закончилась
                        //счетчика не достигли
                        //продолжаем сравнение
                    }
                }
            }
        }
    }
}
