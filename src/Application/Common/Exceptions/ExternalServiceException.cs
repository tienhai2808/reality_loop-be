namespace Application.Common.Exceptions;

public sealed class ExternalServiceException(string message, Exception? inner = null) : Exception(message, inner);
