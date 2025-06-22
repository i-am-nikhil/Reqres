using System.Net;

namespace Reqres.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom exception for API-specific errors. Remains in the Application layer
    /// so the core logic can throw and handle specific, defined exceptions.
    /// </summary>
    public class ApiClientException : Exception
    {
        public HttpStatusCode? StatusCode { get; }

        public ApiClientException(string message) : base(message) { }
        public ApiClientException(string message, Exception innerException) : base(message, innerException) { }
        public ApiClientException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}