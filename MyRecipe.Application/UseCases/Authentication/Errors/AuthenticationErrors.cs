using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;

namespace MyRecipe.Application.UseCases.Authentication.Errors
{
    public class AuthenticationErrors
    {
        public static readonly ErrorResult QueryInputRequired = new(
            "GetClaimReportByQueryInput required",
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
    }
}
