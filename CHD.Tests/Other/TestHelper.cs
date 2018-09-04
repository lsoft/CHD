using System;
using System.Globalization;
using System.IO;

namespace CHD.Tests.Other
{
    internal static class TestHelper
    {
        public static int AbsRandom
        {
            get
            {
                return
                    Math.Abs(Random);
            }
        }

        public static int Random
        {
            get
            {
                var g = Guid.NewGuid();
                var preg = g.ToString().Substring(0, 8);
                var seed = int.Parse(preg, NumberStyles.HexNumber);

                var now = DateTime.Now;
                var xormask = now.DayOfYear * 24 * 60 * 60 + (int)now.TimeOfDay.TotalSeconds;

                seed ^= xormask;

                return
                    seed;
            }
        }

        public static byte[] ReadBodyFrom(
            string filePath,
            long startPosition,
            int tailSize
            )
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Position = startPosition;

                var tailBuffer = new byte[tailSize];

                fileStream.Read(
                    tailBuffer,
                    0,
                    tailSize
                    );

                return
                    tailBuffer;
            }
        }

        public static string GetFolderPathThatDoesNotExists()
        {
            string folderPath;
            do
            {
                folderPath =
                    Path.Combine(
                        Path.GetTempPath(),
                        Guid.NewGuid().ToString()
                        );
            }
            while (System.IO.Directory.Exists(folderPath));

            return
                folderPath;
        }

        public static string GetFilePathThatDoesNotExists()
        {
            string filePath;
            do
            {
                filePath =
                    Path.Combine(
                        Path.GetTempPath(),
                        Path.GetRandomFileName()
                        );
            }
            while (System.IO.File.Exists(filePath));
            return filePath;
        }

        public static void SafeDeleteFolder(string folderPath)
        {
            try
            {
                System.IO.Directory.Delete(folderPath);
            }
            catch
            {
                //nothing to do
            }
        }

        public static void SafeDeleteFile(string filePath)
        {
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch
            {
                //nothing to do
            }
        }

        public static byte[] GenerateBufferWithRandomData(int seed, long bufferSize)
        {
            var result = new byte[bufferSize];

            new Random(seed).NextBytes(result);

            return
                result;
        }

        public static byte[] GenerateBufferWithRandomData(long bufferSize)
        {
            var result = new byte[bufferSize];

            new Random(AbsRandom).NextBytes(result);

            return
                result;
        }
    }
}