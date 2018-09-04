using System;
using System.Collections.Generic;
using System.IO;
using CHD.Common.PathComparer;

namespace CHD.Tests.FileSystem
{
    internal static class DiskOperationHelper
    {
        public static FileInfo FileInfo(this string filePath)
        {
            var fi = new FileInfo(filePath);

            return fi;
        }

        public static DirectoryInfo FolderInfo(this string folderPath)
        {
            var di = new DirectoryInfo(folderPath);

            return di;
        }

        public static IEnumerable<string> Folders(this string folderPath)
        {
            var result = Directory.GetDirectories(folderPath);

            return result;
        }

        public static IEnumerable<string> Files(this string folderPath)
        {
            var result = Directory.GetFiles(folderPath);

            return result;
        }

        public static string Parent(this string folderPath)
        {
            var fdi = new DirectoryInfo(folderPath);

            return fdi.Parent.FullName;
        }

        public static bool Parent(this string folderPath, string rootFolderPath, IPathComparerProvider pathComparerProvider)
        {
            var fdi = new DirectoryInfo(folderPath);
            var rfdi = new DirectoryInfo(rootFolderPath);

            if (string.Compare(fdi.FullName, rfdi.FullName, pathComparerProvider.Comparison) == 0)
            {
                return
                    false;
            }

            return
                true;
        }

        public static string CreateFolder(this string parentFolderPath, string childFolderName)
        {
            var childFolderPath = Path.Combine(parentFolderPath, childFolderName);

            if (!Directory.Exists(childFolderPath))
            {
                Directory.CreateDirectory(childFolderPath);
            }

            return
                childFolderPath;
        }

        public static string DeleteFolder(this string parentFolderPath, string folderName)
        {
            var folderPath = Path.Combine(parentFolderPath, folderName);

            Directory.Delete(
                folderPath,
                true
                );

            return
                folderPath;
        }

        public static string DeleteFile(this string parentFolderPath, string fileName)
        {
            var filePath = Path.Combine(parentFolderPath, fileName);

            File.Delete(
                filePath
                );

            return
                filePath;
        }

        public static void CreateFileWithBody(this string parentFolderPath, string fileName, byte[] body)
        {
            var filePath = Path.Combine(parentFolderPath, fileName);

            File.WriteAllBytes(
                filePath,
                body
                );
        }

        public static string ReplaceFileBody(this string parentFolderPath, string fileName, Func<long, byte[]> bodyProvider)
        {
            var filePath = Path.Combine(parentFolderPath, fileName);

            var length = new FileInfo(filePath).Length;

            var body = bodyProvider(length);

            File.WriteAllBytes(
                filePath,
                body
                );

            return
                filePath;
        }
        
        public static string Enter(this string parentFolderPath, string childFolderName)
        {
            var childFolderPath = Path.Combine(parentFolderPath, childFolderName);

            if (Directory.Exists(childFolderPath))
            {
                return
                    childFolderPath;
            }

            return null;
        }
    }
}