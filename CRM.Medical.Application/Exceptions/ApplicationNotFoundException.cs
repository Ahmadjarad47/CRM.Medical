namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationNotFoundException(string message) : ApplicationExceptionBase(message)
{
    public override int StatusCode => 404;
}
