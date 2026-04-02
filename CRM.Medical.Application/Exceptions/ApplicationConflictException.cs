namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationConflictException(string message) : ApplicationExceptionBase(message)
{
    public override int StatusCode => 409;
}
