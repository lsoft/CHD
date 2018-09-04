using System;
using System.Globalization;
using CHD.Common.FileSystem.FFile;
using CHD.Common.Others;
using CHD.Common.Saver;

namespace CHD.Common.ServiceCode
{
    public sealed class SubjectComposer
    {
        private const char Divider = ';';

        public static string ComposeSubject(
            int structureCurrentVersion,
            long order,
            Guid transactionGuid,
            MessageTypeEnum messageType,
            Guid letterGuid,
            ICopyableFile file
            )
        {
            var lines = new string[7];

            lines[0] = structureCurrentVersion.ToString(CultureInfo.InvariantCulture);
            lines[1] = order.ToString(CultureInfo.InvariantCulture);
            lines[2] = transactionGuid.ToString();
            lines[3] = messageType.ToString();
            lines[4] = Base64Helper.EncodeToString(file.FullPath);
            lines[5] = letterGuid.ToString();
            lines[6] = file.Size.ToString();

            var subject = string.Join(Divider.ToString(), lines);

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
                int structureCurrentVersion;
                long order;
                Guid transactionGuid;
                MessageTypeEnum messageType;
                string fullPath;
                Guid letterGuid;
                long size;
                ParseSubject(
                    subject, 
                    out structureCurrentVersion,
                    out order, 
                    out transactionGuid,
                    out messageType,
                    out fullPath, 
                    out letterGuid,
                    out size
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
            out int structureCurrentVersion,
            out long order, 
            out Guid transactionGuid,
            out MessageTypeEnum messageType,
            out string fullPath, 
            out Guid letterGuid,
            out long size
            )
        {
            if (subject == null)
            {
                throw new ArgumentNullException("subject");
            }

            var lines = subject.Split(Divider);


            structureCurrentVersion = int.Parse(lines[0]);
            order = long.Parse(lines[1]);
            transactionGuid = Guid.Parse(lines[2]);
            if (!Enum.TryParse(lines[3], out messageType))
            {
                throw new ArgumentException("messageType");
            }
            fullPath = Base64Helper.DecodeFromString(lines[4]);
            letterGuid = Guid.Parse(lines[5]);
            size = long.Parse(lines[6]);

        }
    }
}
