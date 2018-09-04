using System.Collections.Generic;

namespace MailRu.Cloud.WebApi
{
    /// <summary>
    ///     List of items in cloud.
    /// </summary>
    public sealed class Entry
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Entry" /> class.
        /// </summary>
        /// <param name="foldersCount">Number of the folders.</param>
        /// <param name="filesCount">Number of the files.</param>
        /// <param name="folders">List of the folders.</param>
        /// <param name="files">List of the files.</param>
        /// <param name="path">The entry path on the server.</param>
        public Entry(int foldersCount, int filesCount, List<Folder> folders, List<File> files, string path)
        {
            NumberOfFolders = foldersCount;
            NumberOfFiles = filesCount;
            Folders = folders;
            Files = files;
            FullPath = path;
        }

        /// <summary>
        ///     Gets number of the folders.
        /// </summary>
        public int NumberOfFolders
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets number of the files.
        /// </summary>
        public int NumberOfFiles
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets list of the folders with their specification.
        /// </summary>
        public List<Folder> Folders
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets list of the files with their specification.
        /// </summary>
        public List<File> Files
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets full entry path on the server.
        /// </summary>
        public string FullPath
        {
            get;
            internal set;
        }
    }
}