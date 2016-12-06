using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHD.Common;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;

namespace CHD.Email.Operation
{
    internal class EmailSubjectComposer
    {
        public static string ComposeSubject(
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

            var subject = string.Join("*", lines);

            return
                subject;
        }

        public static bool IsSubjectCanBeParsed(
            string subject
            )
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var result = false;

            try
            {
                long order;
                Guid transactionGuid;
                GraveyardOperationTypeEnum operationType;
                Suffix filePathSuffix;
                Guid mailGuid;
                ParseSubject(
                    subject, 
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

        public static void ParseSubject(
            string subject, 
            out long order, 
            out Guid transactionGuid,
            out GraveyardOperationTypeEnum operationType,
            out Suffix filePathSuffix, 
            out Guid mailGuid
            )
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var lines = subject.Split('*');


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
