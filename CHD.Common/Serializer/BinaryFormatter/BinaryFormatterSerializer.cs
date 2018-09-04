using System;
using System.IO;

namespace CHD.Common.Serializer.BinaryFormatter
{
    public sealed class BinaryFormatterSerializer : ISerializer
    {
        public T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); //no not make this as field! no need to have state in this class because of multithreading!

                bf.Serialize(ms, obj);

                ms.Position = 0;

                T result = (T)bf.Deserialize(ms);

                return
                    result;
            }
        }

        public byte[] Serialize<T>(T savedObject)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); //no not make this as field! no need to have state in this class because of multithreading!

                bf.Serialize(ms, savedObject);

                ms.Position = 0;

                return
                    ms.ToArray();
            }
        }

        public void Serialize<T>(T savedObject, Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (!destination.CanWrite)
            {
                throw new InvalidOperationException("destination stream cannot write data");
            }

            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); //no not make this as field! no need to have state in this class because of multithreading!

            bf.Serialize(destination, savedObject);
        }

        public T Deserialize<T>(Stream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!source.CanRead)
            {
                throw new InvalidOperationException("source stream cannot read data");
            }

            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); //no not make this as field! no need to have state in this class because of multithreading!
            
            T result = (T)bf.Deserialize(source);

            result.Actualize();

            return
                result;
        }
    }
}
