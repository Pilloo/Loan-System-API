namespace ErrorHandling;

/**
 * <summary>Enum that contains all the most typically used HTTP return codes.</summary>
 */
public enum ErrorCodes
{
    // 4xx Client Errors
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    Conflict = 409,
    Gone = 410,
    PreconditionFailed = 412,
    UnsupportedMediaType = 415,
    ValidationFailed = 422,
    TooManyRequests = 429,

    // 5xx Server Errors
    InternalError = 500,
    NotImplemented = 501,
    ServiceUnavailable = 503,
    GatewayTimeout = 504,

    // Custom Business Errors
    InsufficientBalance = 402,  // Payment Required
    ExpiredToken = 419         // Non-standard (commonly used)
}
