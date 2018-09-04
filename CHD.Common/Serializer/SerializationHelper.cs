using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace CHD.Common.Serializer
{
    public static class SerializationHelper
    {
        public static void Actualize<T>(
            this T t
            )
        {
            var upgraders = ScanForUpgradersInVersionOrder<T>();

            foreach (var actualizer in upgraders)
            {
                actualizer.Upgrade(t);
            }
        }

        public static int GetVersion<T>(
            )
        {
            var upgraders = ScanForUpgradersInVersionOrder<T>();

            var lod = upgraders.LastOrDefault();

            if (lod == null)
            {
                return
                    0;
            }

            return
                lod.Version;
        }

        private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        private static IReadOnlyList<IDeserializeUpgrader<T>> ScanForUpgradersInVersionOrder<T>(
            )
        {
            var t = typeof(T);

            Func<Type, object> factory = tt =>
            {
                var nesteds = t.GetNestedTypes(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                var upgraders = nesteds
                    .Where(j => j.GetInterface(typeof(IDeserializeUpgrader<>).FullName) != null)
                    .Select(k => (IDeserializeUpgrader<T>)Activator.CreateInstance(k))
                    .OrderBy(l => l.Version)
                    .ToList()
                    ;

                for (var i = 0; i < upgraders.Count; i++)
                {
                    if (upgraders[i].Version != (i + 1))
                    {
                        throw new SerializationException("Versions does not make a chain");
                    }
                }

                return
                    upgraders;
            };

            var result = (IReadOnlyList<IDeserializeUpgrader<T>>)_cache.GetOrAdd(
                t,
                factory
                );

            return
                result;
        }
    }

    /*
    EXAMPLE

    [Serializable]
    internal sealed class Subject
    {
        public SerializationVersionProvider<Subject> SerializationVersion = new SerializationVersionProvider<Subject>();

        public string Field0;
        public string Field1;
        public string Field2;

        public Subject()
        {
            Field0 = "000";
            Field1 = "111";
            Field2 = "222";
        }

        private sealed class DeserializeUpgrader2 : IDeserializeUpgrader<Subject>
        {
            public int Version
            {
                get
                {
                    return
                        2;
                }
            }

            public void Upgrade(Subject subject)
            {
                if (subject == null)
                {
                    return;
                }

                subject.Field2 = "default2";
            }
        }

        private sealed class DeserializeUpgrader1 : IDeserializeUpgrader<Subject>
        {
            public int Version
            {
                get
                {
                    return
                        1;
                }
            }

            public void Upgrade(Subject subject)
            {
                if (subject == null)
                {
                    return;
                }

                subject.Field1 = "default1";
            }
        }
    }


    */
}