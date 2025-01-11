namespace UrlShortener.Common.Results;

public class Result<T> 
{
    private T? _value;

    private Result(bool isSuccess, Error error)
    {
        if(isSuccess && error != Error.None
            || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        if(error == Error.None)
        {
            throw new ArgumentException("Invalid error");
        }
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public T? Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("There is no value for failure");
            }

            return _value!;
        }

        private init => _value = value;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error); 
}
