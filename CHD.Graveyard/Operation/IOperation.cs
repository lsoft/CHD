using System;
using System.IO;
using CHD.FileSystem.Algebra;

namespace CHD.Graveyard.Operation
{
    public interface IOperation
    {
        long Order
        {
            get;
        }

        Guid TransactionGuid
        {
            get;
        }

        GraveyardOperationTypeEnum OperationType
        {
            get;
        }

        Suffix FilePathSuffix
        {
            get;
        }

        void WriteRemoteDataTo(
            Stream destination
            );

        void Delete();
    }
}