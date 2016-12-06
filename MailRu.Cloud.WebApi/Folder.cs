namespace MailRu.Cloud.WebApi
{
    /// <summary>
    ///     Server folder info.
    /// </summary>
    public class Folder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Folder" /> class.
        /// </summary>
        public Folder()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Folder" /> class.
        /// </summary>
        /// <param name="name">Folder name.</param>
        public Folder(string name)
            : this(0, 0, name, 0L, ServerPath.Empty)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Folder" /> class.
        /// </summary>
        /// <param name="foldersCount">Number of folders.</param>
        /// <param name="filesCount">Number of files.</param>
        /// <param name="name">Folder name.</param>
        /// <param name="size">Folder size.</param>
        /// <param name="fullPath">Full folder path.</param>
        /// <param name="publicLink">Public folder link.</param>
        public Folder(int foldersCount, int filesCount, string name, long size, ServerPath fullPath, string publicLink = null)
        {
            NumberOfFolders = foldersCount;
            NumberOfFiles = filesCount;
            Name = name;
            Size = size;
            FullPath = fullPath;
            PublicLink = publicLink;
        }

        /// <summary>
        ///     Gets number of folders in folder.
        /// </summary>
        /// <value>Number of folders.</value>
        public int NumberOfFolders
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets number of files in folder.
        /// </summary>
        /// <value>Number of files.</value>
        public int NumberOfFiles
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets folder name.
        /// </summary>
        /// <value>Folder name.</value>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets folder size.
        /// </summary>
        /// <value>Folder size.</value>
        public long Size
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets full folder path on the server.
        /// </summary>
        /// <value>Full folder path.</value>
        public ServerPath FullPath
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets public folder link.
        /// </summary>
        /// <value>Public link.</value>
        public string PublicLink
        {
            get;
            internal set;
        }
    }
}