using System;

namespace CHD.Common.Operation
{
    public static class OperationTypeEnumHelper
    {
        public static string ToHumanReadableString(
            this OperationTypeEnum operationType
            )
        {
            switch (operationType)
            {
                case OperationTypeEnum.NotSpecified:
                    return "Not Specified";
                case OperationTypeEnum.Create:
                    return "Create";
                case OperationTypeEnum.Recreate:
                    return "Recreate";
                case OperationTypeEnum.Delete:
                    return "Delete";
                default:
                    throw new ArgumentOutOfRangeException("operationType");
            }
        }
    }
}