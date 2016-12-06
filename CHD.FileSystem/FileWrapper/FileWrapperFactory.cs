using System;
using System.Globalization;
using System.Xml;
using CHD.FileSystem.Algebra;

namespace CHD.FileSystem.FileWrapper
{
    public class FileWrapperFactory : IFileWrapperFactory
    {
        public IFileWrapper CreateFile(
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

            return 
                new RealFile(
                    filePath,
                    folderPath
                    );
        }

        public IFileWrapper CreateFile(
            XmlNode source
            )
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var filename = source.SelectSingleNode("filename").InnerText;
            var folderpath = source.SelectSingleNode("folderpath").InnerText;
            var filepath = source.SelectSingleNode("filepath").InnerText;
            var filepathsuffix = source.SelectSingleNode("filepathsuffix").InnerText;
            var modificationtime = new DateTime(long.Parse(source.SelectSingleNode("modificationtime").InnerText, CultureInfo.InvariantCulture));
            var exists = bool.Parse(source.SelectSingleNode("exists").InnerText);

            return 
                new RealFile(
                    filename,
                    folderpath,
                    filepath,
                    new Suffix(filepathsuffix),
                    modificationtime,
                    exists
                    );
        }
    }
}