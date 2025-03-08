namespace Shared.ErrorHandling;

public record InvalidCredentials()
    : InternalError(ErrorReason.Unauthorized, "Invalid credentials.");

public record EmailNotVerified()
    : InternalError(ErrorReason.Unauthorized, "Email not confirmed. A new confirmation link has been sent.");

public record EmptyFields() : InternalError(ErrorReason.BadRequest, "One or more fields are empty.");

public record UserNotFound() : InternalError(ErrorReason.NotFound, "User not found.");

public record ExternalServiceUnavailable()
    : InternalError(ErrorReason.ServiceUnavailable, "External service is not available.");

public record InternalServerError() : InternalError(ErrorReason.InternalServerError, "Internal server error.");

public record EmailCanNotBeConfirmed() : InternalError(ErrorReason.InternalServerError,
    "Email can not be confirmed. Check if the email or token are valid.");

public record EmailOrUsernameAlreadyUsed() : InternalError(ErrorReason.Conflict, "Email or username already taken.");

public record PasswordDoesNotMeetSecurityCriteria()
    : InternalError(ErrorReason.Conflict, "Password does not meet security criteria.");

public record EmailAlreadyVerified() : InternalError(ErrorReason.Conflict, "Email already verified.");