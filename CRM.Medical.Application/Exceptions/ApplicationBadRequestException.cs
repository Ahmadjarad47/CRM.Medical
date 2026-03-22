namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationBadRequestException(string message, string errorCode = "bad_request")
    : ApplicationExceptionBase(message, errorCode, 400);
