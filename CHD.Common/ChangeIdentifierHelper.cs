using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CHD.Common.FileSystem.FFolder;
using CHD.Common.Others;

namespace CHD.Common
{
    public static class ChangeIdentifierHelper
    {
        public static Guid CalculateChangeIdentifier(
            this IFolder root
            )
        {
            if (root.ChildCount == 0)
            {
                return
                    Guid.Empty;
            }

            var tuples = new List<Tuple<string, Guid>>();

            DoProcessFolder(
                ref tuples,
                root
                );

            var arrays = tuples.ConvertAll(j => j.Item2.ToByteArray());

            var aggregated = arrays.Concatenate();

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(aggregated);

                var result = new Guid(hash);

                Debug.WriteLine("Tree: " + result);
                foreach (var tuple in tuples)
                {
                    Debug.WriteLine("    {0}: {1}", tuple.Item2, tuple.Item1);
                }

                return
                    result;
            }
        }

        public static Guid MakeForFile(
            IFolder currentFolder,
            string fileName,
            DateTime lastWriteTimeUtc
            )
        {
            if (currentFolder == null)
            {
                throw new ArgumentNullException("currentFolder");
            }
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var fullPath = Path.Combine(currentFolder.FullPath, fileName);

            using (var md5 = MD5.Create())
            {
                var array = Encoding.UTF8.GetBytes(fullPath);
                var time = BitConverter.GetBytes(lastWriteTimeUtc.Ticks);
                var combined = array.Concatenate<byte>(time);

                var hash = md5.ComputeHash(combined);

                var result = new Guid(hash);

                //Debug.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■ " + fullPath + " [time]: " + time.ArrgegateToString());
                //Debug.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■ " + fullPath + " [hash]: " + hash.ArrgegateToString());
                //Debug.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■ " + fullPath + ": " + lastWriteTimeUtc.ToString("HH:mm:ss.fff"));
                //Debug.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■ " + fullPath + " [" + lastWriteTimeUtc.Ticks + "]: " + result);

                return
                    result;
            }
        }

        internal static Guid MakeForFolder(
            IFolder currentFolder,
            string folderName
            )
        {
            if (currentFolder == null)
            {
                throw new ArgumentNullException("currentFolder");
            }
            if (folderName == null)
            {
                throw new ArgumentNullException("folderName");
            }

            var fullPath = Path.Combine(currentFolder.FullPath, folderName);

            using (var md5 = MD5.Create())
            {
                var array = Encoding.UTF8.GetBytes(fullPath);

                var hash = md5.ComputeHash(array);

                var result = new Guid(hash);

                Debug.WriteLine(fullPath + ": " + result);

                return
                    result;
            }
        }

        private static void DoProcessFolder(
            ref List<Tuple<string, Guid>> guids,
            IFolder folder
            )
        {
            foreach (var childFile in folder.Files.OrderBy(j => j.Name))
            {
                guids.Add(
                    new Tuple<string, Guid>(
                        childFile.FullPath,
                        childFile.ChangeIdentifier
                        )
                    );
            }

            foreach (var childFolder in folder.Folders.OrderBy(j => j.Name))
            {
                guids.Add(
                    new Tuple<string, Guid>(
                        childFolder.FullPath,
                        childFolder.ChangeIdentifier
                        )
                    );

                DoProcessFolder(ref guids, childFolder);
            }
        }

    }
}