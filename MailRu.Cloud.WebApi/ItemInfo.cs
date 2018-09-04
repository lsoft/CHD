using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Cloud.WebApi
{
    internal sealed class ItemInfo
    {
        public bool IsFolder
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets file name.
        /// </summary>
        /// <value>File name.</value>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets full file path with name in server.
        /// </summary>
        /// <value>Full file path.</value>
        public MailRuCloudPath FullPath
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets file hash value.
        /// </summary>
        /// <value>File hash.</value>
        public string Hash
        {
            get;
            internal set;
        }

        /// <summary>
        ///     File size.
        /// </summary>
        public long Size
        {
            get;
            internal set;
        }

        public ItemInfo(string name, MailRuCloudPath fullPath)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }
            IsFolder = true;
            Name = name;
            FullPath = fullPath;
        }

        public ItemInfo(string name, MailRuCloudPath fullPath, string hash, long size)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (fullPath == null)
            {
                throw new ArgumentNullException("fullPath");
            }
            if (hash == null)
            {
                throw new ArgumentNullException("hash");
            }
            IsFolder = false;
            Name = name;
            FullPath = fullPath;
            Hash = hash;
            Size = size;
        }
    }
}
