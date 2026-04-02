namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationForbiddenException(string message) : ApplicationExceptionBase(message)
{
    public override int StatusCode => 403;
}
