namespace Domain.Common;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail with an error message.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message when the operation failed. Null on success.
    /// </summary>
    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">Description of what went wrong.</param>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The result value.</param>
    public static Result<T> Success<T>(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="error">Description of what went wrong.</param>
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value on success.</typeparam>
public class Result<T>
    : Result
{
    /// <summary>
    /// The value returned on success. Default when the operation failed.
    /// </summary>
    public T? Value { get; }

    internal Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
}
