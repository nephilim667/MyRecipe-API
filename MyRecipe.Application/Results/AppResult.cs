using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MyRecipe.Application.Results
{
    public class AppResult
    {
        protected AppResult(bool isSuccess, ErrorResult? error)
        {
            if (isSuccess && error is not null || !isSuccess && error is null)
                throw new ArgumentException("Invalid error", nameof(error));

            IsSuccess = isSuccess;
            Error = error;
        }

        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailure => !IsSuccess;

        public ErrorResult? Error { get; }

        public static AppResult Success() => new(true, null);
        public static AppResult Failure(ErrorResult error) => new(false, error);

        public static bool TryFailure<TResponse>(ErrorResult error, [NotNullWhen(true)] out TResponse? result)
        {
            if (typeof(TResponse) == typeof(AppResult))
            {
                result = (TResponse)(object)Failure(error);
                return true;
            }

            if (!typeof(TResponse).IsGenericType || typeof(TResponse).GetGenericTypeDefinition() != typeof(AppResult<>))
            {
                result = default;
                return false;
            }

            MethodInfo? failureMethod = typeof(TResponse).GetMethod(
                nameof(Failure),
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(ErrorResult)],
                null);

            if (failureMethod is null)
            {
                result = default;
                return false;
            }

            result = (TResponse)failureMethod.Invoke(null, [error])!;
            return true;
        }
    }

    public sealed class AppResult<T> : AppResult
    {
        private AppResult(bool isSuccess, T? content, ErrorResult? error) : base(isSuccess, error)
        {
            Content = content;
        }

        public T? Content { get; }

        public static AppResult<T> Success(T result) => new(true, result, null);
        public static new AppResult<T> Failure(ErrorResult error) => new(false, default, error);
    }
}
