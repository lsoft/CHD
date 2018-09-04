using System;
using System.Xml;
using CHD.Common;
using CHD.Common.Others;
using CHD.Common.PathComparer;

namespace CHD.Tests.FileSystem.Schema
{
    public sealed class FileSystemSchema : IFileSystemSchema
    {
        private readonly IPathComparerProvider _pathComparerProvider;
        private readonly IDisorderLogger _logger;

        public XmlDocument Schema
        {
            get;
            private set;
        }

        public FileSystemSchema(
            IPathComparerProvider pathComparerProvider,
            XmlDocument schema,
            IDisorderLogger logger
            )
        {
            if (pathComparerProvider == null)
            {
                throw new ArgumentNullException("pathComparerProvider");
            }
            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _pathComparerProvider = pathComparerProvider;
            Schema = schema;
            _logger = logger;
        }

        public bool SchemaEquals(IFileSystemSchema fsSchema)
        {
            if (fsSchema == null)
            {
                throw new ArgumentNullException("fsSchema");
            }

            var bthis = this.Schema.Beautify();
            var bother = fsSchema.Schema.Beautify();

            var result = string.Compare(bthis, bother, _pathComparerProvider.Comparison);

            return
                result == 0;
        }

        public void Log(
            string header
            )
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }


            _logger.LogMessage(header);
            _logger.LogMessage(Schema.Beautify());
            _logger.LogMessage(string.Empty);
        }
    }
}
