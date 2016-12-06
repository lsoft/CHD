using System;
using System.Globalization;
using System.IO;
using System.Xml;
using CHD.FileSystem.Algebra;

namespace CHD.FileSystem.FileWrapper
{
    public class RealFile : IFileWrapper
    {
        public string FileName
        {
            get;
            private set;
        }

        public string FolderPath
        {
            get;
            private set;
        }

        public string FilePath
        {
            get;
            private set;
        }

        public Suffix FilePathSuffix
        {
            get;
            private set;
        }

        public DateTime ModificationTime
        {
            get;
            private set;
        }

        public bool Exists
        {
            get;
            private set;
        }

        public RealFile(
            string filePath,
            string folderPath
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            var fi = new FileInfo(filePath);
            
            FileName = fi.Name;
            FilePath = fi.FullName;
            FolderPath = fi.Directory != null ? fi.Directory.FullName : string.Empty;
            ModificationTime = fi.LastWriteTime;
            Exists = fi.Exists;
            FilePathSuffix = PathAlgebra.GetSuffix(FilePath, FolderPath);
        }

        public RealFile(
            string fileName, 
            string folderPath, 
            string filePath,
            Suffix filePathSuffix, 
            DateTime modificationTime, 
            bool exists
            )
        {
            FileName = fileName;
            FolderPath = folderPath;
            FilePath = filePath;
            FilePathSuffix = filePathSuffix;
            ModificationTime = modificationTime;
            Exists = exists;
        }

        public IFileWrapper RefreshByFileSystem(
            )
        {
            var result = new RealFile(
                FilePath,
                FolderPath
                );

            return
                result;
        }

        public void Serialize(
            XmlNode target
            )
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var filenameNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "filename",
                null
                );
            filenameNode.InnerText = FileName;
            target.AppendChild(filenameNode);

            var filepathNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "filepath",
                null
                );
            filepathNode.InnerText = FilePath;
            target.AppendChild(filepathNode);

            var folderPathNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "folderpath",
                null
                );
            folderPathNode.InnerText = FolderPath;
            target.AppendChild(folderPathNode);

            var filePathSuffix = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "filepathsuffix",
                null
                );
            filePathSuffix.InnerText = FilePathSuffix.FilePathSuffix;
            target.AppendChild(filePathSuffix);

            var modificationTimeNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "modificationtime",
                null
                );
            modificationTimeNode.InnerText = ModificationTime.Ticks.ToString(CultureInfo.InvariantCulture);
            target.AppendChild(modificationTimeNode);

            var existsNode = target.OwnerDocument.CreateNode(
                XmlNodeType.Element,
                "exists",
                null
                );
            existsNode.InnerText = Exists.ToString();
            target.AppendChild(existsNode);
        }
    }
}