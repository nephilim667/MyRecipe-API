using FluentValidation;
using MyRecipe.Application.UseCases.Authentication.Errors;

namespace MyRecipe.Application.UseCases.Authentication.Queries.Validators
{
    public class AuthenticationQueryValidator : AbstractValidator<AuthenticationQuery>
    {
        public AuthenticationQueryValidator() 
        {
            RuleFor(qry => qry.authenticationInput)
                .NotNull()
                .WithMessage(AuthenticationErrors.QueryInputRequired.Description)
                .WithErrorCode(AuthenticationErrors.QueryInputRequired.Code);

            When(qry => qry.authenticationInput is not null, () =>
            {
                RuleFor(qry => qry.authenticationInput.UsernameOrEmail)
                    .NotEmpty()
                    .WithMessage(AuthenticationErrors.UserNameOrEmailRequired.Description)
                    .WithErrorCode(AuthenticationErrors.UserNameOrEmailRequired.Code);

                RuleFor(qry => qry.authenticationInput.Password)
                    .NotEmpty()
                    .WithMessage(AuthenticationErrors.PasswordRequired.Description)
                    .WithErrorCode(AuthenticationErrors.PasswordRequired.Code);
            });
        }
    }
}
