using System;

namespace CHD.Common.Logger
{
    public interface IDisorderLogger
    {
        void LogMessage(string message);
        void LogFormattedMessage(string message, params object[] args);
        void LogException(Exception excp, string message = "");
    }
}