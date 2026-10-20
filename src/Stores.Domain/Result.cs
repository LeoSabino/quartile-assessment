namespace Stores.Domain;

public sealed class Result<T>
{
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    public bool IsSuccess { get; }
    public T Value { get; }

    public static Result<T> Success(T value) => new(value);
}
