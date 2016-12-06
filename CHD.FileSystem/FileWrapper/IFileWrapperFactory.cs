using System.Xml;

namespace CHD.FileSystem.FileWrapper
{
    public interface IFileWrapperFactory
    {
        IFileWrapper CreateFile(
            string filePath,
            string folderPath
            );

        IFileWrapper CreateFile(
            XmlNode source
            );
    }
}