namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationNotFoundException(string message, string errorCode = "resource_not_found")
    : ApplicationExceptionBase(message, errorCode, 404);
