using System;
using System.IO;
using System.Net;
using System.Text;

namespace MailRu.Cloud.WebApi
{
    public static class Helper
    {
        /// <summary>
        ///     Read web response as text.
        /// </summary>
        /// <param name="resp">Web response.</param>
        /// <returns>Converted text.</returns>
        internal static string ReadResponseAsText(WebResponse resp)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    ReadResponseAsByte(resp, stream);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
                catch
                {
                    //// Cancellation token.
                    return "7035ba55-7d63-4349-9f73-c454529d4b2e";
                }
            }
        }

        /// <summary>
        ///     Write byte array in the stream.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <param name="outputStream">Stream to writing.</param>
        public static void WriteBytesInStream(byte[] bytes, Stream outputStream)
        {
            using (var stream = new MemoryStream(bytes))
            {
                WriteBytesInStream(stream, outputStream, bytes.LongLength);
            }
        }

        /// <summary>
        ///     Write file in the stream.
        /// </summary>
        /// <param name="fileStream">file stream</param>
        /// <param name="startPosition">Started read position in input stream. Input stream will create from file info.</param>
        /// <param name="size">File size.</param>
        /// <param name="outputStream">Stream to writing.</param>
        public static void WriteBytesInStream(
            Stream fileStream,
            long startPosition,
            long size,
            Stream outputStream
            )
        {
            fileStream.Seek(startPosition, SeekOrigin.Begin);
            WriteBytesInStream(fileStream, outputStream, size);
        }


        /// <summary>
        ///     Write one stream to another.
        /// </summary>
        /// <param name="fileStream">Source stream.</param>
        /// <param name="outputStream">Stream to writing.</param>
        /// <param name="length">Stream length.</param>
        public static void WriteBytesInStream(
            Stream fileStream,
            Stream outputStream,
            long length
            )
        {
            fileStream.CopyTo(outputStream);

            //int bufferLength = 8192;
            //long totalWritten = 0L;
            //if (length < bufferLength)
            //{
            //    fileStream.CopyTo(outputStream);
            //}
            //else
            //{
            //    while (length > totalWritten)
            //    {
            //        byte[] bytes = new byte[bufferLength];
            //        fileStream.Read(
            //            bytes,
            //            0,
            //            bufferLength
            //            );
            //        //byte[] bytes = fileStream.ReadBytes(bufferLength);
            //        outputStream.Write(bytes, 0, bufferLength);

            //        totalWritten += bufferLength;
            //        if (length - totalWritten < bufferLength)
            //        {
            //            bufferLength = (int)(length - totalWritten);
            //        }
            //    }
            //}
        }

        /// <summary>
        ///     Read web response as byte array.
        /// </summary>
        /// <param name="resp">Web response.</param>
        /// <param name="outputStream">Output stream to writing the response.</param>
        internal static void ReadResponseAsByte(
            WebResponse resp,
            Stream outputStream = null
            )
        {
            int bufSizeChunk = 30000;
            int totalBufSize = bufSizeChunk;
            var fileBytes = new byte[totalBufSize];

            int totalBytesRead = 0;

            using (var reader = new BinaryReader(resp.GetResponseStream()))
            {
                int bytesRead = 0;
                while ((bytesRead = reader.Read(fileBytes, totalBytesRead, totalBufSize - totalBytesRead)) > 0)
                {
                    if (outputStream != null)
                    {
                        outputStream.Write(fileBytes, totalBytesRead, bytesRead);
                    }

                    totalBytesRead += bytesRead;

                    if ((totalBufSize - totalBytesRead) == 0)
                    {
                        totalBufSize += bufSizeChunk;
                        Array.Resize(ref fileBytes, totalBufSize);
                    }
                }
            }
        }

    }
}
