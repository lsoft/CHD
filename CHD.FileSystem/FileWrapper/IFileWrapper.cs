using System;
using System.Xml;
using CHD.FileSystem.Algebra;

namespace CHD.FileSystem.FileWrapper
{
    public interface IFileWrapper
    {
        string FileName
        {
            get;
        }

        string FolderPath
        {
            get;
        }

        string FilePath
        {
            get;
        }

        Suffix FilePathSuffix
        {
            get;
        }

        DateTime ModificationTime
        {
            get;
        }

        bool Exists
        {
            get;
        }

        IFileWrapper RefreshByFileSystem(
            );

        void Serialize(
            XmlNode target
            );
    }
}
