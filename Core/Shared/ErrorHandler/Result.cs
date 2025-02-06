using Core.DTOs;

namespace Core.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public InternalError? Error { get; set; }
    public T Value { get; }

    private Result(T value, bool isSuccess, InternalError? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException("Success result can't have an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException("Failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;

        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(value, true, null);

    public static Result<T> Failure(InternalError error)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));
        return new Result<T>(default!, false, error);
    }
}