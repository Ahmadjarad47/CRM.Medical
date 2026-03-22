namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationForbiddenException(string message, string errorCode = "forbidden")
    : ApplicationExceptionBase(message, errorCode, 403);
