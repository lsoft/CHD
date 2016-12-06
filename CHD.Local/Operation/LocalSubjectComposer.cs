using System;
using System.Globalization;
using System.IO;
using CHD.Common;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;

namespace CHD.Local.Operation
{
    internal class LocalSubjectComposer
    {
        public static void ParseSubject(
            string filePath, 
            out long order, 
            out Guid transactionGuid,
            out GraveyardOperationTypeEnum operationType,
            out Suffix filePathSuffix
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var fi = new FileInfo(filePath);
            var fn = fi.Name;

            var joined = Base64Helper.DecodeFromString(fn);

            var lines = joined.Split('*');

            order = long.Parse(lines[0]);
            transactionGuid = Guid.Parse(lines[1]);
            if (!Enum.TryParse(lines[2], out operationType))
            {
                throw new ArgumentException("operationType");
            }
            filePathSuffix = new Suffix(lines[3]);
        }

        public static string ComposeSubject(
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            IFileWrapper fileWrapper,
            string folderPath
            )
        {
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }

            var lines = new string[4];

            lines[0] = order.ToString(CultureInfo.InvariantCulture);
            lines[1] = transactionGuid.ToString();
            lines[2] = operationType.ToString();
            lines[3] = fileWrapper.FilePathSuffix.FilePathSuffix;

            var joined = string.Join("*", lines);
            var fileName = Base64Helper.EncodeToString(joined);
            var filePath = Path.Combine(folderPath, fileName);

            return
                filePath;
        }
    }
}
