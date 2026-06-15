using FluentAssertions;
using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.Interfaces;
using MyRecipe.Application.Outputs.Queries.Authentication;
using MyRecipe.Application.UseCases.Authentication.Errors;
using MyRecipe.Application.UseCases.Authentication.Queries;
using NSubstitute;
using Xunit;

namespace Application.Tests.UseCases.Authentication
{
    public class AuthenticationQueryHandlerTests
    {
        private readonly IAuthenticationService _authService;
        private readonly AuthenticationQueryHandler _handler;

        public AuthenticationQueryHandlerTests()
        {
            // 1. Create the mock cleanly without .Object
            _authService = Substitute.For<IAuthenticationService>();

            // System Under Test (SUT)
            _handler = new AuthenticationQueryHandler(_authService);
        }

        [Fact]
        public async Task Should_Return_Token_Response_When_Credentials_Are_Valid()
        {
            // Arrange
            var input = new AuthenticationQueryInput() { UsernameOrEmail = "sandbox_chef", Password = "ChefPassword123!" };
            var query = new AuthenticationQuery(input);
            var mockResponse = new AuthenticationResponse() { Token = "mocked-jwt-token", Username = "sandbox_chef", Email = "sandbox@myrecipe.com" };

            // NSubstitute style: No .Setup(), it automatically handles the Task wrapping!
            _authService.AuthenticateAsync(input.UsernameOrEmail, input.Password)
                        .Returns(mockResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("mocked-jwt-token");
            result.Username.Should().Be("sandbox_chef");

            // Asserting behavior: Check that the service was called exactly once
            await _authService.Received(1).AuthenticateAsync(input.UsernameOrEmail, input.Password);
        }

        [Fact]
        public async Task Should_Throw_UnauthorizedAccessException_When_Credentials_Are_Invalid()
        {
            // Arrange
            var input = new AuthenticationQueryInput() { UsernameOrEmail = "imposter_chef", Password = "wrong-password" };
            var query = new AuthenticationQuery(input);

            // Mocking a failed authentication return
            _authService.AuthenticateAsync(input.UsernameOrEmail, input.Password)
                        .Returns((AuthenticationResponse?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage(AuthenticationErrors.InvalidCredentials.Description);
        }
    }
}
