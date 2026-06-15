using Xunit;

using FluentValidation.TestHelper;

using MyRecipe.Application.UseCases.Authentication.Errors;
using MyRecipe.Application.UseCases.Authentication.Queries;
using MyRecipe.Application.UseCases.Authentication.Queries.Validators;

namespace Application.Tests.UseCases.Authentication
{
    public class AuthenticationQueryValidatorTests
    {
        private readonly AuthenticationQueryValidator _validator;

        public AuthenticationQueryValidatorTests()
        {
            _validator = new();
        }

        [Fact]
        public void Should_Have_An_Error_When_AuthenticationQueryInput_Is_Null()
        {
            // Arrange
            var query = new AuthenticationQuery(null!);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.authenticationInput)
                  .WithErrorMessage(AuthenticationErrors.QueryInputRequired.Description)
                  .WithErrorCode(AuthenticationErrors.QueryInputRequired.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_Have_An_Error_When_UsernameOrEmail_Is_Empty(string usernameOrEmail)
        {
            // Arrange
            var query = new AuthenticationQuery(new() { UsernameOrEmail = usernameOrEmail, Password = "ChefPassword123!" });

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.authenticationInput.UsernameOrEmail)
                  .WithErrorMessage(AuthenticationErrors.UserNameOrEmailRequired.Description)
                  .WithErrorCode(AuthenticationErrors.UserNameOrEmailRequired.Code);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_Have_An_Error_When_Password_Is_Empty(string password)
        {
            // Arrange
            var query = new AuthenticationQuery(new() { UsernameOrEmail = "sandbox_chef", Password = password });

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.authenticationInput.Password)
                  .WithErrorMessage(AuthenticationErrors.PasswordRequired.Description)
                  .WithErrorCode(AuthenticationErrors.PasswordRequired.Code);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        public void Should_Have_An_Error_When_Password_Is_Too_Short(string password)
        {
            // Arrange
            var query = new AuthenticationQuery(new() { UsernameOrEmail = "sandbox_chef", Password = password });

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.authenticationInput.Password)
                  .WithErrorMessage(AuthenticationErrors.PasswordTooShort.Description)
                  .WithErrorCode(AuthenticationErrors.PasswordTooShort.Code);

        }

        [Fact]
        public void Should_Pass_When_Query_Is_Valid()
        {
            // Arrange
            var query = new AuthenticationQuery(new() { UsernameOrEmail = "sandbox_chef", Password = "ChefPassword123!" });

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}
