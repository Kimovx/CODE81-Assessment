namespace CODE81_Assessment.Application.Exceptions
{
    public class HttpException(string message, int statusCode) : Exception(message)
    {
        public string MessageEn { get; } = message;
        public int StatusCode { get; } = statusCode;
    }

    public class InvalidCredentialsException : HttpException
    {
        public InvalidCredentialsException()
            : base("Invalid credentials.", 401)
        { }
    }

    public class UserHasNoRolesException : HttpException
    {
        public UserHasNoRolesException()
            : base("User has no roles assigned.", 403)
        { }
    }

    public class RefreshTokenExpiredException : HttpException
    {
        public RefreshTokenExpiredException()
            : base("Refresh token expired.", 401)
        { }
    }

    public class InvalidRefreshTokenException : HttpException
    {
        public InvalidRefreshTokenException()
            : base("Invalid refresh token.", 401)
        { }
    }

    public class RefreshTokenReuseDetectedException : HttpException
    {
        public RefreshTokenReuseDetectedException()
            : base("Refresh token reuse detected. Please login again.", 401)
        { }
    }

    public class UserCreationFailedException : HttpException
    {
        public UserCreationFailedException()
            : base("User creation failed.", StatusCodes.Status400BadRequest)
        { }
    }

    public class UserRoleAssignmentFailedException : HttpException
    {
        public UserRoleAssignmentFailedException()
            : base("Failed to assign role to user.", StatusCodes.Status400BadRequest)
        { }
    }

    public class UserUpdateFailedException : HttpException
    {
        public UserUpdateFailedException()
            : base("User update failed.", StatusCodes.Status400BadRequest)
        { }
    }

    public class UserDeletionFailedException : HttpException
    {
        public UserDeletionFailedException()
            : base("User deletion failed.", StatusCodes.Status400BadRequest)
        { }
    }

    public class UnknownInternalServerError : HttpException
    {
        public UnknownInternalServerError()
            : base("An unknown internal server error occurred.", StatusCodes.Status500InternalServerError)
        { }
    }

    public class LogCreationFailedException : HttpException
    {
        public LogCreationFailedException()
            : base("Failed to create log entry.", StatusCodes.Status500InternalServerError)
        { }
    }

    public sealed class EntityNotFoundException : HttpException
    {
        public EntityNotFoundException(string message = "Not found.")
            : base(message, 404)
        { }
    }

    public sealed class ConflictException : HttpException
    {
        public ConflictException(string message = "Conflict")
            : base(message, 409)
        { }
    }

    public sealed class BadRequestException : HttpException
    {
        public BadRequestException(string message = "Bad request")
            : base(message, 400)
        { }
    }
}
