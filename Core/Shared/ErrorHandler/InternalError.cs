namespace Core.Shared;

public abstract record InternalError(ErrorReason Reason, string Message);
