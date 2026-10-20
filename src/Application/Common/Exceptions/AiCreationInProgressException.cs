namespace Application.Common.Exceptions;

public sealed class AiCreationInProgressException() : Exception("This connection is still being created. Retry the same request shortly.");
