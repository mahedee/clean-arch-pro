namespace EduTrack.Domain.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public string ErrorCode { get; }

    protected Result(bool isSuccess, string error, string errorCode)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");
        
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error");

        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, string.Empty, string.Empty);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty, string.Empty);

    public static Result Failure(string error, string errorCode = "OPERATION_FAILED") => 
        new(false, error, errorCode);

    public static Result<T> Failure<T>(string error, string errorCode = "OPERATION_FAILED") => 
        new(default!, false, error, errorCode);

    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail
/// </summary>
/// <typeparam name="T">The type of the value returned on success</typeparam>
public class Result<T> : Result
{
    private readonly T _value;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException("Cannot access value of a failed result");
            
            return _value;
        }
    }

    protected internal Result(T value, bool isSuccess, string error, string errorCode)
        : base(isSuccess, error, errorCode)
    {
        _value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure<T>(error);
}

/// <summary>
/// Contains multiple validation errors
/// </summary>
public class ValidationResult : Result
{
    public IEnumerable<ValidationError> ValidationErrors { get; }

    private ValidationResult(IEnumerable<ValidationError> errors) 
        : base(false, "Validation failed", "VALIDATION_FAILED")
    {
        ValidationErrors = errors;
    }

    public static ValidationResult WithErrors(IEnumerable<ValidationError> errors) => new(errors);
}

/// <summary>
/// Represents a validation error
/// </summary>
public class ValidationError
{
    public string PropertyName { get; }
    public string ErrorMessage { get; }
    public object? AttemptedValue { get; }
    public string ErrorCode { get; }

    public ValidationError(string propertyName, string errorMessage, object? attemptedValue = null, string errorCode = "VALIDATION_ERROR")
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        AttemptedValue = attemptedValue;
        ErrorCode = errorCode;
    }
}