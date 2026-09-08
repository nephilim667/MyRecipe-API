using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;

namespace MyRecipe.Application.UseCases.Authentication.Errors
{
    public static class AuthenticationErrors
    {
        public static readonly ErrorResult InvalidCredentials = new(
            "Authentication credentials invalids",
            ErrorType.Invalid,
            "Invalid credentials",
            "authentication-credentials-invalids");

        public static readonly ErrorResult QueryInputRequired = new(
            "AuthenticationQueryInput required",
            ErrorType.Error,
            "Authentication query input cannot be null.",
            "authentication-queryinput-required");

        public static readonly ErrorResult UserNameOrEmailRequired = new(
            "Username or Email required",
            ErrorType.Error,
            "Username or Email is required.",
            "authentication-username-required");

        public static readonly ErrorResult PasswordRequired = new(
            "Password required",
            ErrorType.Error,
            "Password is required.",
            "authentication-password-required");

        public static readonly ErrorResult PasswordTooShort = new(
            "Password length too short",
            ErrorType.Error,
            "Password length must greather than 3.",
            "authentication-password-too-short");
    }
}
