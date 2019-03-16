using System;

namespace SimpleBlog.Infrastructures.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCodeException(int statusCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
