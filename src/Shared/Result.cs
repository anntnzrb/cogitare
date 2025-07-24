namespace Cogitare.Shared;

internal abstract record Result<T>
{
    public bool IsSuccess => this is Success<T>;
    public bool IsFailure => this is Failure<T>;

    public static implicit operator Result<T>(T value) => new Success<T>(value);
}

internal sealed record Success<T>(T Value) : Result<T>;

internal sealed record Failure<T>(string Error) : Result<T>;

internal static class Result
{
    public static Result<T> Success<T>(T value) => new Success<T>(value);
    public static Result<T> Failure<T>(string error) => new Failure<T>(error);
}