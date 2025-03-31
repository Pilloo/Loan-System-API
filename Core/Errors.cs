using ErrorHandling;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core;

/// <summary>
/// Represents an error thrown when authentication fails due to invalid credentials
/// or unverified email address.
/// </summary>
/// <remarks>
/// <para>
/// Uses a generic message to prevent credential enumeration attacks. The same message
/// is returned for both invalid credentials and unverified emails.
/// </para>
/// <para>
/// Corresponds to HTTP 401 Unauthorized status code.
/// </para>
/// </remarks>
public record InvalidCredentialsOrEmailNotVerified() : InternalError
(
    Code: ErrorCodes.Unauthorized,
    Title: "Invalid credentials",
    Detail:
    "Invalid credentials or email not verified. Please check your details or request a new verification link."
);

/// <summary>
/// Represents a validation error containing detailed field-specific error messages.
/// </summary>
/// <param name="validationErrors">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
/// <remarks>
/// <para>
/// Transforms <see cref="ModelStateDictionary"/> errors into RFC 7807 compliant format.
/// The <see cref="InternalError.Extensions"/> property contains detailed validation messages:
/// </para>
/// <para>
/// Empty or null error messages are automatically filtered out.
/// </para>
/// <para>
/// Corresponds to HTTP 400 Bad Request status code.
/// </para>
/// </remarks>
public record ValidationFailed(ModelStateDictionary validationErrors) : InternalError
(
    Code: ErrorCodes.BadRequest,
    Title: "Invalid request",
    Detail: "One or more validation errors occurred.",
    Extensions: new Dictionary<string, object>(
        validationErrors
            .Where(kvp => kvp.Value!.Errors.Any(e => !string.IsNullOrEmpty(e.ErrorMessage)))
            .Select(kvp => KeyValuePair.Create(
                kvp.Key,
                kvp.Value!.Errors.Count == 1
                    ? (object)kvp.Value.Errors[0].ErrorMessage!
                    : kvp.Value.Errors.Select(e => e.ErrorMessage!).ToList()
            ))
    ));

/// <summary>
/// Represents an error thrown when a requested user cannot be found.
/// </summary>
/// <remarks>
/// Corresponds to HTTP 404 Not Found status code.
/// </remarks>
public record UserNotFound() : InternalError
(
    Code: ErrorCodes.NotFound,
    Title: "User not found",
    Detail: "The user requested could not be found."
);

/// <summary>
/// Represents an error thrown when an external service (email provider, API, etc.) is unavailable.
/// </summary>
/// <remarks>
/// Corresponds to HTTP 503 Service Unavailable status code.
/// </remarks>
public record ExternalServiceUnavailable() : InternalError
(
    Code: ErrorCodes.ServiceUnavailable,
    Title: "External service is not available",
    Detail: "The external service requested is not available."
);

/// <summary>
/// Represents an error thrown when an unexpected system failure occurs.
/// </summary>
/// <remarks>
/// <para>
/// Should be used for unexpected server-side errors only.
/// </para>
/// <para>
/// Corresponds to HTTP 500 Internal Server Error status code.
/// </para>
/// </remarks>
public record InternalServerError() : InternalError
(
    Code: ErrorCodes.InternalError,
    Title: "Internal server error",
    Detail: "One or more internal components have failed."
);

/// <summary>
/// Represents an error thrown when registration fails due to duplicate email or username.
/// </summary>
/// <remarks>
/// <para>
/// Uses a generic message to prevent account enumeration attacks. Does not disclose
/// whether the email or username was already taken.
/// </para>
/// <para>
/// Corresponds to HTTP 400 Bad Request status code.
/// </para>
/// </remarks>
public record EmailOrUsernameAlreadyUsed() : InternalError
(
    Code: ErrorCodes.BadRequest,
    Title: "Registration failed",
    Detail: "An account with these details already exists or the information is invalid. " +
            "Please try again or use the 'Forgot Password' option if needed."
);

/// <summary>
/// Represents an error thrown when email verification fails.
/// </summary>
/// <remarks>
/// <para>
/// Uses a generic message to prevent enumeration attacks. Covers multiple failure scenarios:
/// <list type="bullet">
/// <item>Email not registered</item>
/// <item>Invalid verification token</item>
/// <item>Email already verified</item>
/// </list>
/// </para>
/// <para>
/// Corresponds to HTTP 409 Conflict status code.
/// </para>
/// </remarks>
public record InvalidVerificationLink() : InternalError
(
    Code: ErrorCodes.Conflict,
    Title: "Invalid verification link",
    Detail: "This verification link is invalid or has already been used. Please request a new one if needed."
);

/// <summary>
/// Represents an error thrown when password validation fails against security requirements.
/// </summary>
/// <param name="operationResult">The <see cref="IdentityResult"/> containing password validation errors.</param>
/// <remarks>
/// <para>
/// This error aggregates multiple password validation failures into a structured RFC 7807 response.
/// The <see cref="InternalError.Extensions"/> property contains detailed validation messages under the
/// <c>password_validation_errors</c> key.
/// </para>
/// <para>
/// Empty or null error descriptions are automatically filtered out.
/// </para>
/// <para>
/// Corresponds to HTTP 422 Unprocessable Entity status code.
/// </para>
/// </remarks>
public record PasswordDoesNotMeetSecurityCriteria(IdentityResult operationResult) : InternalError
(
    Code: ErrorCodes.ValidationFailed,
    Title: "Password does not meet security criteria",
    Detail:
    "The password you have entered does not meet the basic security criteria. Please create a new password " +
    "following the established security criteria.",
    Extensions: new Dictionary<string, object>
    {
        ["password_validation_errors"] =
            operationResult.Errors.Where(error => !String.IsNullOrEmpty(error.Description))
                .Select(error => error.Description).ToList()
    }
);

public record FileNotFound() : InternalError(
    Code: ErrorCodes.NotFound,
    Title: "File not found",
    Detail: "The file you requested is invalid. Please try again."
);