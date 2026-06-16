using System.Diagnostics.CodeAnalysis;

namespace MyRecipe.Application.Results
{
    public class AppResult
    {
        public AppResult(bool isSuccess, ErrorResult? error = null, object content = null)
        {
            if (isSuccess && error is not null || !isSuccess && error is null)
                throw new ArgumentException("Invalid error", nameof(error));

            IsSuccess = isSuccess;
            Error = error;
            Content = content;
        }

        public object? Content { get; }

        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailure => !IsSuccess;

        public ErrorResult? Error { get; }

        public static AppResult Success() => new(true, null);
        public static AppResult Failure(ErrorResult error) => new(false, error);

    }

    public class AppResult<T>(bool isSuccess, ErrorResult? error = null, T? content = default) : AppResult(isSuccess, error)
    {
        public new T? Content { get; } = content;

        [MemberNotNullWhen(false, nameof(Error))]
        public new bool IsSuccess => base.IsSuccess;

        [MemberNotNullWhen(true, nameof(Error))]
        public new bool IsFailure => base.IsFailure;

        public ErrorResult? Error => base.Error;

        public static AppResult<T> Success(T result) => new(true, null, result);
        public static new AppResult<T> Failure(ErrorResult error) => new(false, error, default);
    }
}
