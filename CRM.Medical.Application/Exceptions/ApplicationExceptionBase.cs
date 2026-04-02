namespace CRM.Medical.Application.Exceptions;

public abstract class ApplicationExceptionBase(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}
