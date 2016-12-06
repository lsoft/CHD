using System.IO;

namespace MailRu.Cloud.WebApi.Connection
{
    public interface IConnection
    {
        /// <summary>
        ///     Create folder on the server.
        /// </summary>
        /// <param name="name">New path name.</param>
        /// <param name="createIn">Destination path.</param>
        ServerPath CreateFolder(
            string name,
            ServerPath createIn
            );

        /// <summary>
        ///     Remove folder.
        /// </summary>
        /// <param name="folderPath">Full folder name.</param>
        ServerPath RemoveFolder(
            ServerPath folderPath
            );

        /// <summary>
        ///     Remove file.
        /// </summary>
        /// <param name="filePath">Full file name.</param>
        void RemoveFile(
            ServerPath filePath
            );

        /// <summary>
        ///     Upload file on the server asynchronously, if not use async await will be use synchronously operation.
        /// </summary>
        /// <param name="fileStream">file stream</param>
        /// <param name="fileName">File name.</param>
        /// <param name="destinationPath">Destination file path on the server.</param>
        void UploadFile(
            Stream fileStream,
            string fileName,
            ServerPath destinationPath
            );

        /// <summary>
        ///     Download file 
        /// </summary>
        /// <param name="destination">channel to write file's data</param>
        /// <param name="filePath">Path to the file in the cloud</param>
        /// <returns>File as byte array.</returns>
        void DownloadFile(
            Stream destination,
            ServerPath filePath
            );

        /// <summary>
        ///     Get list of files and folders from account.
        /// </summary>
        /// <param name="path">Path in the cloud to return the list of the items.</param>
        /// <returns>List of the items.</returns>
        Entry GetItems(
            ServerPath path
            );

    }
}