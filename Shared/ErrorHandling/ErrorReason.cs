namespace Shared.ErrorHandling;

public enum ErrorReason
{
    Unauthorized,
    NotFound,
    BadRequest,
    Conflict,
    ServiceUnavailable,
    InternalServerError,
}