namespace Domain.Common;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail with an error.
/// </summary>
public class Result
{
    /// <summary>
    /// Message shown when the operation failed. Null on success.
    /// </summary>
    private const string ConcurrencyMessage = "The data was modified by another user. Please try again.";

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message when the operation failed. Null on success.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Category of the failure. Meaningful only when <see cref="IsSuccess"/> is false.
    /// </summary>
    public ResultErrorKind ErrorKind { get; }

    protected Result(bool isSuccess, string? error, ResultErrorKind errorKind)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorKind = errorKind;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null, ResultErrorKind.RuleViolation);

    /// <summary>
    /// Creates a failed result describing a violated business rule.
    /// </summary>
    /// <param name="error">Description of what went wrong.</param>
    public static Result Failure(string error) => new(false, error, ResultErrorKind.RuleViolation);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The result value.</param>
    public static Result<T> Success<T>(T value) => new(value, true, null, ResultErrorKind.RuleViolation);

    /// <summary>
    /// Creates a failed result describing a violated business rule.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="error">Description of what went wrong.</param>
    public static Result<T> Failure<T>(string error) => new(default, false, error, ResultErrorKind.RuleViolation);

    /// <summary>
    /// Creates a failed result carrying over the message and kind of an earlier failure.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="failure">The failure to propagate.</param>
    public static Result<T> Failure<T>(Result failure) => new(default, false, failure.Error, failure.ErrorKind);

    /// <summary>
    /// Creates a failed result describing something that does not exist.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="error">Description of what was not found.</param>
    public static Result<T> NotFound<T>(string error) => new(default, false, error, ResultErrorKind.NotFound);

    /// <summary>
    /// Creates a failed result describing a lost race with another user.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    public static Result<T> ConcurrencyConflict<T>() => new(default, false, ConcurrencyMessage, ResultErrorKind.ConcurrencyConflict);
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

    internal Result(T? value, bool isSuccess, string? error, ResultErrorKind errorKind)
        : base(isSuccess, error, errorKind)
    {
        Value = value;
    }
}
