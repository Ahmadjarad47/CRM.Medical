namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationUnauthorizedException(string message) : ApplicationExceptionBase(message)
{
    public override int StatusCode => 401;
}
