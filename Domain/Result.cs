namespace Domain;

public static class Result
{
    public static SuccessResult Success() => new();

    public static SuccessResult<T> Success<T>(T value) => new(value);

    public static ErrorResult Error() => new();

    public static ErrorResult<T> Error<T>() => new();

    public static ErrorResult<T, string> Error<T>(string message) => new(message);

    public static ValidationResult<T> Invalid<T>(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        => new(failures);
}

public interface IResult
{
    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}

public class SuccessResult : IResult
{
    public bool IsSuccess => true;

    public override string ToString() => "Success";
}

public class SuccessResult<T> : SuccessResult, IResult<T>
{
    public SuccessResult(T data) => Data = data;

    public T Data { get; }

    public override string ToString() => Data?.ToString() ?? $"Success<{typeof(T).Name}>";
}

public interface IErrorResult : IResult
{
    public IErrorResult<TCast> Cast<TCast>();
}

public class ErrorResult : IErrorResult
{
    public bool IsSuccess => false;

    public override string ToString() => "Error";

    public virtual IErrorResult<TCast> Cast<TCast>() => new ErrorResult<TCast>();
}

public interface IErrorResult<T> : IResult<T>, IErrorResult { }

public class ErrorResult<T> : ErrorResult, IErrorResult<T>
{
    public T Data => throw new Exception("Cannot access data when result is in error");
}

public class ErrorResult<T, E> : ErrorResult<T>, IErrorResult<T>
{
    public ErrorResult(E error) => Error = error;

    public E Error { get; }

    public override string ToString() => $"{Error}";

    public override ErrorResult<TCast, E> Cast<TCast>() => new(Error);
}

public class ValidationResult : ErrorResult
{
    public ValidationResult(params FluentValidation.Results.ValidationFailure[] failures)
        => Failures = failures;

    public ValidationResult(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        => Failures = failures;

    public IEnumerable<FluentValidation.Results.ValidationFailure> Failures { get; }
}

public class ValidationResult<T> : ValidationResult, IErrorResult<T>
{
    public ValidationResult(params FluentValidation.Results.ValidationFailure[] failures)
        : base(failures) { }

    public ValidationResult(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base(failures) { }

    public T Data => throw new Exception("Cannot access data when result is in error");

    public override ValidationResult<TCast> Cast<TCast>() => new(Failures);
}
