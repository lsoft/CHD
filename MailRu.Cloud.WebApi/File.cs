namespace MailRu.Cloud.WebApi
{
    /// <summary>
    ///     Server file info.
    /// </summary>
    public sealed class File
    {
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
        ///     Gets public file link.
        /// </summary>
        /// <value>Public link.</value>
        public string PublicLink
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets or sets base file name.
        /// </summary>
        internal string PrimaryName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets base file size.
        /// </summary>
        /// <value>File size.</value>
        internal long PrimarySize
        {
            get;
            set;
        }
    }
}