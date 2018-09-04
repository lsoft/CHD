using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using CHD.Common;
using CHD.Common.FileSystem.FFile;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.PathComparer;

namespace CHD.Tests.FileSystem.Schema
{
    public sealed class StructureFileSystemSchemaFactory
    {
        public static IFileSystemSchema CreateSchema(
            IPathComparerProvider pathComparerProvider,
            IFolder rootFolder,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (rootFolder == null)
            {
                throw new ArgumentNullException("rootFolder");
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
                rootFolder,
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
            IFolder currentFolder,
            XmlNode parentNode
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (currentFolder == null)
            {
                throw new ArgumentNullException("currentFolder");
            }
            if (parentNode == null)
            {
                throw new ArgumentNullException("parentNode");
            }

            var currentNode = ConvertToNode(xmlDocument, currentFolder);
            parentNode.AppendChild(currentNode);

            foreach (var cfi in currentFolder.Files.OrderBy(j => j.Name))
            {
                var fnode = ConvertToNode(xmlDocument, cfi);
                currentNode.AppendChild(fnode);
            }

            foreach (var cfo in currentFolder.Folders.OrderBy(j => j.Name))
            {
                ProcessFolder(
                    xmlDocument,
                    cfo,
                    currentNode
                    );
            }
        }

        private static XmlNode ConvertToNode(
            XmlDocument xmlDocument,
            IFile file
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var currentNode = xmlDocument.CreateNode(XmlNodeType.Element, "File", string.Empty);

            var nameAttr = xmlDocument.CreateAttribute("Name");
            nameAttr.AppendChild(xmlDocument.CreateTextNode(file.Name));
            currentNode.Attributes.Append(nameAttr);

            var sizeAttr = xmlDocument.CreateAttribute("Size");
            sizeAttr.AppendChild(xmlDocument.CreateTextNode(file.Size.ToString(CultureInfo.InvariantCulture)));
            currentNode.Attributes.Append(sizeAttr);

            return
                currentNode;
        }

        private static XmlNode ConvertToNode(
            XmlDocument xmlDocument,
            IFolder folder
            )
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            var currentNode = xmlDocument.CreateNode(XmlNodeType.Element, "Folder", string.Empty);

            var nameAttr = xmlDocument.CreateAttribute("Name");
            nameAttr.AppendChild(xmlDocument.CreateTextNode(folder.Name));
            currentNode.Attributes.Append(nameAttr);

            return
                currentNode;
        }

    }
}