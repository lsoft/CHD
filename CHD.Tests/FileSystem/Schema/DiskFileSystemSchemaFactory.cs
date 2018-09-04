using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using CHD.Common;
using CHD.Common.PathComparer;
using CHD.Tests.FileSystem.Surgeon;

namespace CHD.Tests.FileSystem.Schema
{
    public sealed class DiskFileSystemSchemaFactory
    {
        public static IFileSystemSchema CreateSchema(
            IPathComparerProvider pathComparerProvider,
            string rootFolderPath,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (rootFolderPath == null)
            {
                throw new ArgumentNullException("rootFolderPath");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            var schema = new XmlDocument();
            var snode = schema.CreateNode(XmlNodeType.Element, "Structure", string.Empty);
            schema.AppendChild(snode);

            ProcessFolder(
                schema,
                rootFolderPath,
                snode
                );

            var result = new FileSystemSchema(
                pathComparerProvider,
                schema,
                logger
                );

            return result;
        }

        private static void ProcessFolder(
            XmlDocument xmlDocument,
            string folderPath,
            XmlNode parentNode
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            var currentNode = ConvertFolderToNode(xmlDocument, folderPath);
            parentNode.AppendChild(currentNode);

            foreach (var cfi in folderPath.Files().OrderBy(j => j))
            {
                var fnode = ConvertFileToNode(xmlDocument, cfi);
                currentNode.AppendChild(fnode);
            }

            foreach (var cfo in folderPath.Folders().OrderBy(j => j))
            {
                ProcessFolder(
                    xmlDocument,
                    cfo,
                    currentNode
                    );
            }
        }

        private static XmlNode ConvertFileToNode(
            XmlDocument xmlDocument,
            string filePath
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var currentNode = xmlDocument.CreateNode(XmlNodeType.Element, "File", string.Empty);

            var nameAttr = xmlDocument.CreateAttribute("Name");
            nameAttr.AppendChild(xmlDocument.CreateTextNode(filePath.FileInfo().Name));
            currentNode.Attributes.Append(nameAttr);

            var sizeAttr = xmlDocument.CreateAttribute("Size");
            sizeAttr.AppendChild(xmlDocument.CreateTextNode(filePath.FileInfo().Length.ToString(CultureInfo.InvariantCulture)));
            currentNode.Attributes.Append(sizeAttr);

            return
                currentNode;
        }

        private static XmlNode ConvertFolderToNode(
            XmlDocument xmlDocument,
            string folderPath
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            var currentNode = xmlDocument.CreateNode(XmlNodeType.Element, "Folder", string.Empty);

            var nameAttr = xmlDocument.CreateAttribute("Name");
            nameAttr.AppendChild(xmlDocument.CreateTextNode(folderPath.FolderInfo().Name));
            currentNode.Attributes.Append(nameAttr);

            return
                currentNode;
        }

    }
}