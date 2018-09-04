using CHD.Common.Serializer;

namespace CHD.Common.Saver
{
    public interface IBinarySaver<TAddress>
        where TAddress : IAddress
    {
        ISerializer Serializer
        {
            get;
        }

        bool IsTargetExists(
            TAddress address
            );

        void Save<T>(
            TAddress address,
            T savedObject
            );

        T Read<T>(
            TAddress address
            );
    }
}