using System;
using System.Globalization;
using CHD.Common;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;

namespace CHD.MailRuCloud.Operation
{
    internal class MailRuFileNameComposer
    {
        internal const string Suffix = ".bin";

        public static string ComposeFileName(
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            Guid mailGuid,
            IFileWrapper fileWrapper
            )
        {
            var lines = new string[5];

            lines[0] = order.ToString(CultureInfo.InvariantCulture);
            lines[1] = transactionGuid.ToString();
            lines[2] = operationType.ToString();
            lines[3] = Base64Helper.EncodeToString(fileWrapper.FilePathSuffix.FilePathSuffix);
            lines[4] = mailGuid.ToString();

            var joined = string.Join(" ", lines);

            var fileName = 
                joined
                //Base64Helper.EncodeToString(joined) 
                + Suffix;

            return
                fileName;
        }

        public static bool IsFileNameCanBeParsed(
            string fileName64
            )
        {
            if (fileName64 == null)
            {
                throw new ArgumentNullException("fileName64");
            }

            var result = false;

            try
            {
                long order;
                Guid transactionGuid;
                GraveyardOperationTypeEnum operationType;
                Suffix filePathSuffix;
                Guid mailGuid;
                ParseFileName(
                    fileName64, 
                    out order, 
                    out transactionGuid,
                    out operationType, 
                    out filePathSuffix, 
                    out mailGuid
                    );

                result = true;
            }
            catch
            {
                //nothing should be performed here
            }

            return
                result;
        }

        public static void ParseFileName(
            string fileName64, 
            out long order, 
            out Guid transactionGuid,
            out GraveyardOperationTypeEnum operationType,
            out Suffix filePathSuffix, 
            out Guid mailGuid
            )
        {
            if (fileName64 == null)
            {
                throw new ArgumentNullException("fileName64");
            }

            var decoded = 
                //Base64Helper.DecodeFromString(fileName64.Substring(0, fileName64.Length - Suffix.Length));
                fileName64.Substring(0, fileName64.Length - Suffix.Length);
            var lines = decoded.Split(' ');

            order = long.Parse(lines[0]);
            transactionGuid = Guid.Parse(lines[1]);
            if (!Enum.TryParse(lines[2], out operationType))
            {
                throw new ArgumentException("operationType");
            }
            filePathSuffix = new Suffix(Base64Helper.DecodeFromString(lines[3]));
            mailGuid = Guid.Parse(lines[4]);

        }
    }
}