using System.Xml;

namespace CHD.Tests.FileSystem.Schema
{
    public interface IFileSystemSchema
    {
        XmlDocument Schema
        {
            get;
        }

        bool SchemaEquals(IFileSystemSchema fsSchema);

        void Log(
            string header
            );
    }
}