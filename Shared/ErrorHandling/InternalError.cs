namespace Shared.ErrorHandling;

public abstract record InternalError(ErrorReason Reason, string Message);
