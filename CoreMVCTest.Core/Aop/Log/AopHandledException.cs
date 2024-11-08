using System;

namespace CoreMVCTest.Core.Aop.Log
{
    public class AopHandledException : ApplicationException
    {
        public string ErrorMessage { get; private set; }
        public Exception InnerHandledException { get; private set; }

        public AopHandledException()
        {
        }

        public AopHandledException(string msg) : base(msg)
        {
            ErrorMessage = msg;
        }

        public AopHandledException(string msg, Exception innerException) : base(msg)
        {
            InnerHandledException = innerException;
            ErrorMessage = msg;
        }

        public string GetError()
        {
            return ErrorMessage;
        }
    }
}
