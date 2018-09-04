using System.IO;

namespace CHD.Common.Serializer
{
    public interface ISerializer
    {
        T DeepClone<T>(T obj);

        byte[] Serialize<T>(
            T savedObject
            );

        void Serialize<T>(
            T savedObject,
            Stream destination
            );

        T Deserialize<T>(
            Stream source
            );
    }
}