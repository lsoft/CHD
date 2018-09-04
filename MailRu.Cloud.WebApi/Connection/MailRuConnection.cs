using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using MailRu.Cloud.WebApi.Shard;

namespace MailRu.Cloud.WebApi.Connection
{
    internal sealed class MailRuConnection : IMailRuConnection
    {
        private const long MaxFileSize = 2L * 1024L * 1024L * 1024L;

        private readonly Requisites _requisites;

        public MailRuConnection(
            Requisites requisites
            )
        {
            if (requisites == null)
            {
                throw new ArgumentNullException("requisites");
            }
            _requisites = requisites;
        }

        /// <summary>
        ///     Remove folder.
        /// </summary>
        /// <param name="folderPath">Full folder name.</param>
        public MailRuCloudPath RemoveFolder(
            MailRuCloudPath folderPath
            )
        {
            var deletedFolderPath = Remove(
                folderPath
                );

            return
                deletedFolderPath;
        }

        /// <summary>
        ///     Remove file.
        /// </summary>
        /// <param name="filePath">Full file name.</param>
        public void RemoveFile(
            MailRuCloudPath filePath
            )
        {
            Remove(
                filePath
                );
        }


        /// <summary>
        ///     Create folder on the server.
        /// </summary>
        /// <param name="name">New path name.</param>
        /// <param name="createIn">Destination path.</param>
        public MailRuCloudPath CreateFolder(
            string name,
            MailRuCloudPath createIn
            )
        {
            var createdFolderPath = AddItem(
                new ItemInfo(
                    name,
                    createIn.Combine(name)
                    )
                );

            return
                createdFolderPath;
        }

        /// <summary>
        ///     Upload file on the server asynchronously, if not use async await will be use synchronously operation.
        /// </summary>
        /// <param name="fileStream">file stream</param>
        /// <param name="fileName">File name.</param>
        /// <param name="destinationPath">Destination file path on the server.</param>
        public void UploadFile(
            Stream fileStream,
            string fileName,
            MailRuCloudPath destinationPath
            )
        {
            var fi = new FileInfo(fileName);
            var extension = fi.Extension;

            UploadFile(
                fileStream,
                fileName,
                extension,
                0L,
                fileStream.Length,
                destinationPath
                );
        }


        /// <summary>
        ///     Download file 
        /// </summary>
        /// <param name="destination">channel to write file's data</param>
        /// <param name="filePath">Path to the file in the cloud</param>
        /// <returns>File as byte array.</returns>
        public void DownloadFile(
            Stream destination,
            MailRuCloudPath filePath
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            DoDownloadFile(
                destination,
                filePath
                );
        }

        /// <summary>
        ///     Get list of files and folders from account.
        /// </summary>
        /// <param name="path">Path in the cloud to return the list of the items.</param>
        /// <returns>List of the items.</returns>
        public Entry GetItems(
            MailRuCloudPath path
            )
        {
            return 
                DoGetItems(
                    path
                    );
        }

        public void Dispose()
        {
            //nothing to do
            //TODO: implement some code!
        }

        #region private code

        /// <summary>
        ///     Get list of files and folders from account.
        /// </summary>
        /// <param name="path">Path in the cloud to return the list of the items.</param>
        /// <returns>List of the items.</returns>
        private Entry DoGetItems(
            MailRuCloudPath path
            )
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            var uri = new Uri(string.Format("{0}/api/v2/folder?token={1}&home={2}", ConstSettings.CloudDomain, _requisites.AuthToken, path.GetUrlEncodedPath()));
            var request = (HttpWebRequest) WebRequest.Create(uri.OriginalString);
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = _requisites.Cookies;
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "application/json";
            request.UserAgent = ConstSettings.UserAgent;

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Cannot connect to Cloud Api");
                }

                var entry = (Entry) JsonParser.Parse(Helper.ReadResponseAsText(response), PObject.Entry);

                return
                    entry;
            }
        }

        /// <summary>
        ///     Download file 
        /// </summary>
        /// <param name="destination">channel to write file's data</param>
        /// <param name="filePath">Path to the file in the cloud</param>
        /// <returns>File as byte array.</returns>
        private void DoDownloadFile(
            Stream destination,
            MailRuCloudPath filePath
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            CookieContainer unusedCookie = null;
            var shard = GetShardInfo(
                ShardType.Get,
                false,
                ref unusedCookie
                );

            var serverFilePath = filePath.GetPath();

            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", shard.Url, serverFilePath.TrimStart('/')));
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = _requisites.Cookies;
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = ConstSettings.DefaultAcceptType;
            request.UserAgent = ConstSettings.UserAgent;
            request.AllowReadStreamBuffering = false;

            var response = request.GetResponse();

            Helper.ReadResponseAsByte(
                response,
                destination
                );
        }

        /// <summary>
        ///     Upload file on the server
        /// </summary>
        /// <param name="fileStream">file stream</param>
        /// <param name="fileName">File name.</param>
        /// <param name="extension">File extension.</param>
        /// <param name="startPosition">Start stream position to writing. Stream will create from file info.</param>
        /// <param name="size">Bytes count to write from source stream in another.</param>
        /// <param name="destinationPath">Destination file path on the server.</param>
        private void UploadFile(
            Stream fileStream,
            string fileName,
            string extension,
            long startPosition,
            long size,
            MailRuCloudPath destinationPath
            )
        {
            destinationPath = destinationPath.SafelyAppend("/");

            if (size > MaxFileSize)
            {
                throw new OverflowException("Not supported file size.", new Exception(string.Format("The maximum file size is {0} byte. Currently file size is {1} byte.", MaxFileSize, size)));
            }

            CookieContainer unusedCookie = null;
            var shard = GetShardInfo(
                ShardType.Upload,
                false,
                ref unusedCookie
                );

            var boundary = Guid.NewGuid();

            //// Boundary request building.
            var boundaryBuilder = new StringBuilder();
            boundaryBuilder.AppendFormat("------{0}\r\n", boundary);
            boundaryBuilder.AppendFormat("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\n", fileName);
            boundaryBuilder.AppendFormat("Content-Type: {0}\r\n\r\n", ConstSettings.GetContentType(extension));

            var endBoundaryBuilder = new StringBuilder();
            endBoundaryBuilder.AppendFormat("\r\n------{0}--\r\n", boundary);

            byte[] endBoundaryRequest = Encoding.UTF8.GetBytes(endBoundaryBuilder.ToString());
            byte[] boundaryRequest = Encoding.UTF8.GetBytes(boundaryBuilder.ToString());

            var url = new Uri(string.Format("{0}?cloud_domain=2&{1}", shard.Url, _requisites.Login));
            var request = (HttpWebRequest)WebRequest.Create(url.OriginalString);
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = _requisites.Cookies;
            request.Method = "POST";
            request.ContentLength = size + boundaryRequest.LongLength + endBoundaryRequest.LongLength;
            request.Referer = string.Format("{0}/home{1}", ConstSettings.CloudDomain, destinationPath.GetUrlEncodedPath());
            request.Headers.Add("Origin", ConstSettings.CloudDomain);
            request.Host = url.Host;
            request.ContentType = string.Format("multipart/form-data; boundary=----{0}", boundary);
            request.Accept = "*/*";
            request.UserAgent = ConstSettings.UserAgent;
            request.AllowWriteStreamBuffering = false;

            using (var s = request.GetRequestStream())
            {
                Helper.WriteBytesInStream(boundaryRequest, s);
                Helper.WriteBytesInStream(fileStream, startPosition, size, s);
                Helper.WriteBytesInStream(endBoundaryRequest, s);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string[] resp = Helper.ReadResponseAsText(response).Split(new[] { ';' });
                        string hashResult = resp[0];
                        long sizeResult = long.Parse(resp[1].Replace("\r\n", string.Empty));

                        AddItem(
                            new ItemInfo(
                                fileName,
                                destinationPath.Combine(fileName),
                                hashResult,
                                sizeResult
                                )
                        );
                    }
                }
            }
        }

        /// <summary>
        ///     Get shard info that to do post get request. Can be use for anonymous user.
        /// </summary>
        /// <param name="shardType">Shard type as numeric type.</param>
        /// <param name="useAnonymousUser">To get anonymous user.</param>
        /// <param name="cookie">Generated cookie.</param>
        /// <returns>Shard info.</returns>
        private ShardInfo GetShardInfo(
            ShardType shardType,
            bool useAnonymousUser,
            ref CookieContainer cookie
            )
        {
            var uri = new Uri(string.Format("{0}/api/v2/dispatcher?{2}={1}", ConstSettings.CloudDomain, !useAnonymousUser ? _requisites.AuthToken : 2.ToString(), !useAnonymousUser ? "token" : "api"));
            var request = (HttpWebRequest)WebRequest.Create(uri.OriginalString);
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = !useAnonymousUser ? _requisites.Cookies : new CookieContainer();
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "application/json";
            request.UserAgent = ConstSettings.UserAgent;

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Cannot connect to Cloud Api");
                }
                
                cookie = request.CookieContainer;

                return
                    (ShardInfo) JsonParser.Parse(Helper.ReadResponseAsText(response), PObject.Shard, shardType.GetEnumDescription());
            }
        }

        /// <summary>
        ///     Remove file or folder.
        /// </summary>
        /// <param name="path">Full file or folder name.</param>
        private MailRuCloudPath Remove(
            MailRuCloudPath path
            )
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            byte[] removeRequest = Encoding.UTF8.GetBytes(string.Format("home={0}&api={1}&token={2}&email={3}&x-email={3}", path.GetPath(), 2, _requisites.AuthToken, _requisites.Login));

            var url = new Uri(string.Format("{0}/api/v2/file/remove", ConstSettings.CloudDomain));
            var request = (HttpWebRequest)WebRequest.Create(url.OriginalString);
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = _requisites.Cookies;
            request.Method = "POST";
            request.ContentLength = removeRequest.LongLength;
            request.Referer = string.Format("{0}/home{1}", ConstSettings.CloudDomain, path.CutLastElement());
            request.Headers.Add("Origin", ConstSettings.CloudDomain);
            request.Host = url.Host;
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "*/*";
            request.UserAgent = ConstSettings.UserAgent;

            using (Stream s = request.GetRequestStream())
            {
                s.Write(removeRequest, 0, removeRequest.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidOperationException("Cannot connect to Cloud Api");
                    }

                    var folderPath = (string)JsonParser.Parse(Helper.ReadResponseAsText(response), PObject.BodyAsString);

                    var result = new MailRuCloudPath(folderPath);

                    return
                        result;
                }
            }
        }


        /// <summary>
        ///     Create file record in the cloud.
        /// </summary>
        /// <param name="itemInfo">Item (file or folder) info.</param>
        private MailRuCloudPath AddItem(
            ItemInfo itemInfo
            )
        {
            bool hasFile = !itemInfo.IsFolder;//itemInfo.Hash != null && itemInfo.Size != 0;
            string filePart = hasFile ? string.Format("&hash={0}&size={1}", itemInfo.Hash, itemInfo.Size) : string.Empty;
            byte[] addFileRequest = Encoding.UTF8.GetBytes(string.Format("home={0}&conflict=rename&api={1}&token={2}", itemInfo.FullPath, 2, _requisites.AuthToken) + filePart);

            var url = new Uri(string.Format("{0}/api/v2/{1}/add", ConstSettings.CloudDomain, hasFile ? "file" : "folder"));
            var request = (HttpWebRequest)WebRequest.Create(url.OriginalString);
            request.Proxy = _requisites.Proxy;
            request.CookieContainer = _requisites.Cookies;
            request.Method = "POST";
            request.ContentLength = addFileRequest.LongLength;
            request.Referer = string.Format("{0}/home{1}", ConstSettings.CloudDomain, itemInfo.FullPath.GetElementsWithoutSuffix(itemInfo.Name).GetUrlEncodedPath());
            request.Headers.Add("Origin", ConstSettings.CloudDomain);
            request.Host = url.Host;
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "*/*";
            request.UserAgent = ConstSettings.UserAgent;

            using (var s = request.GetRequestStream())
            {
                s.Write(addFileRequest, 0, addFileRequest.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidOperationException("Cannot connect to Cloud Api");
                    }

                    var folderPath = (string)JsonParser.Parse(Helper.ReadResponseAsText(response), PObject.BodyAsString);

                    var result = new MailRuCloudPath(folderPath);

                    return
                        result;
                }
            }
        }

        #endregion
    }
}