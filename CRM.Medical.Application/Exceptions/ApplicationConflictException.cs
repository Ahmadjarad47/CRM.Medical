namespace CRM.Medical.Application.Exceptions;

public sealed class ApplicationConflictException(string message, string errorCode = "conflict")
    : ApplicationExceptionBase(message, errorCode, 409);
