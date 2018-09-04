using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CHD.Common.Others
{
    public sealed class CHDException : Exception
    {
        public CHDExceptionTypeEnum ExceptionType
        {
            get;
            private set;
        }

        public CHDException(string message)
            : base(message)
        {
            ExceptionType = CHDExceptionTypeEnum.NotSpecified;
        }

        public CHDException(string message, CHDExceptionTypeEnum exceptionType)
            : base(message)
        {
            ExceptionType = exceptionType;
        }

        public CHDException(string message, Exception innerException, CHDExceptionTypeEnum exceptionType) : base(message, innerException)
        {
            ExceptionType = exceptionType;
        }

        protected CHDException(SerializationInfo info, StreamingContext context, CHDExceptionTypeEnum exceptionType) : base(info, context)
        {
            ExceptionType = exceptionType;
        }

        public CHDException(CHDExceptionTypeEnum exceptionType)
        {
            ExceptionType = exceptionType;
        }
    }
}
