using Microsoft.AspNetCore.Identity;

namespace Core.Shared;

public record UsernameOrPasswordIncorrect()
    : InternalError(ErrorReason.Unauthorized, "Username or password are incorrect");

public record EmailNotVerified() : InternalError(ErrorReason.Unauthorized, "Email not verified");

public record EmailOrPasswordFieldsEmpty() : InternalError(ErrorReason.BadRequest, "Username or pasword are empty");

public record EmptyFields() : InternalError(ErrorReason.BadRequest, "One or more fields are empty");

public record UsernameNotFound() : InternalError(ErrorReason.NotFound, "Username not found");

public record ExternalServiceUnavailable()
    : InternalError(ErrorReason.ServiceUnavailable, "External service is not available");

public record InternalServerError() : InternalError(ErrorReason.InternalServerError, "Internal server error");

public record EmailCanNotBeConfirmed() : InternalError(ErrorReason.InternalServerError, "Email can not be confirmed");

public record EmailOrUsernameAlreadyUsed() : InternalError(ErrorReason.Conflict, "Email or username already used");

public record PassswordDoesNotMeetSecurityCriteria()
    : InternalError(ErrorReason.Conflict, "Passsword does not meet security criteria");