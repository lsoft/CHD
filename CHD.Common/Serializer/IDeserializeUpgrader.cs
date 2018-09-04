namespace CHD.Common.Serializer
{
    public interface IDeserializeUpgrader<in T>
    {
        int Version
        {
            get;
        }

        void Upgrade(T t);
    }
}