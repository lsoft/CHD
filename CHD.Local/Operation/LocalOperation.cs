using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CHD.FileSystem.Algebra;
using CHD.FileSystem.FileWrapper;
using CHD.Graveyard;
using CHD.Graveyard.Operation;

namespace CHD.Local.Operation
{
    [DebuggerDisplay("Order = {Order} {OperationType} {FilePathSuffix}")]
    public class LocalOperation : IOperation
    {
        private readonly string _localFilePath;

        public long Order
        {
            get;
            private set;
        }

        public Guid TransactionGuid
        {
            get;
            private set;
        }

        public GraveyardOperationTypeEnum OperationType
        {
            get;
            private set;
        }

        public Suffix FilePathSuffix
        {
            get;
            private set;
        }

        public LocalOperation(
            string filePath
            )
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("filePath: " + filePath);
            }

            _localFilePath = filePath;


            long order;
            Guid transactionGuid;
            GraveyardOperationTypeEnum operationType;
            Suffix filePathSuffix;
            LocalSubjectComposer.ParseSubject(
                filePath,
                out order,
                out transactionGuid,
                out operationType,
                out filePathSuffix
                );
            Order = order;
            TransactionGuid = transactionGuid;
            OperationType = operationType;
            FilePathSuffix = filePathSuffix;
        }

        public void WriteRemoteDataTo(
            Stream destination
            )
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            var lines = File.ReadAllLines(_localFilePath);
            var body = lines[1];

            var data = new byte[body.Length / 2];
            for (var cc = 0; cc < data.Length; cc++)
            {
                var index = cc * 2;
                var b = byte.Parse(body.Substring(index, 2), NumberStyles.HexNumber);
                data[cc] = b;
            }

            destination.Write(data, 0, data.Length);
        }

        public void Delete()
        {
            if (File.Exists(_localFilePath))
            {
                File.Delete(_localFilePath);
            }
        }

        internal static LocalOperation CreateOperation(
            string folderPath,
            long order,
            Guid transactionGuid,
            GraveyardOperationTypeEnum operationType,
            IFileWrapper fileWrapper,
            byte[] body = null
            )
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }
            if (fileWrapper == null)
            {
                throw new ArgumentNullException("fileWrapper");
            }

            var filePath = LocalSubjectComposer.ComposeSubject(
                order,
                transactionGuid,
                operationType,
                fileWrapper,
                folderPath
                );

            #region file body

            var flines = new string[2];

            flines[0] = string.Join(
                "\t",
                order,
                transactionGuid,
                operationType,
                fileWrapper.FilePathSuffix.FilePathSuffix
            );

            if (body != null)
            {
                var sb = new StringBuilder(body.Length*2);

                foreach (var b in body)
                {
                    sb.Append(b.ToString("X2"));
                }

                flines[1] = sb.ToString();
            }
            else
            {
                flines[1] = string.Empty;
            }

            #endregion

            File.WriteAllLines(
                filePath,
                flines
                );

            return 
                new LocalOperation(filePath);
        }

    }
}