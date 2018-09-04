using System;

namespace CHD.Common.OnlineStatus.Diff.Apply.Report
{
    public static class OperationReportStatusEnumHelper
    {
        public static string ToHumanReadableString(
            this OperationReportStatusEnum operationType
            )
        {
            switch (operationType)
            {
                case OperationReportStatusEnum.NotStarted:
                    return "Not Started";
                case OperationReportStatusEnum.InProgress:
                    return "In Progress";
                case OperationReportStatusEnum.SuccessfullyCompleted:
                    return "Completed";
                case OperationReportStatusEnum.CompletedWithErrors:
                    return "Failed";
                default:
                    throw new ArgumentOutOfRangeException("operationType");
            }
        }
    }
}