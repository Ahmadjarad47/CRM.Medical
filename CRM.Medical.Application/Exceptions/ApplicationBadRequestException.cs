namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationBadRequestException(string message) : ApplicationExceptionBase(message)
{
    public override int StatusCode => 400;
}
