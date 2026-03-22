namespace CRM.Medical.Application.Exceptions;

public abstract class ApplicationExceptionBase(string message, string errorCode, int statusCode)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public int StatusCode { get; } = statusCode;
}
