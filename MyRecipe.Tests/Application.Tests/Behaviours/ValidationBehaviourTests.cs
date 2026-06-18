using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using MyRecipe.Application.Behaviours;
using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;

namespace Application.Tests.Behaviours
{
    public class ValidationBehaviourTests
    {
        [Fact]
        public async Task Handle_Should_Return_Failure_Result_When_Request_Is_Invalid_And_Response_Is_AppResult()
        {
            // Arrange
            var behavior = new ValidationBehaviour<TestRequest, AppResult<TestResponse>>(
                [new TestRequestValidator()],
                NullLogger<ValidationBehaviour<TestRequest, AppResult<TestResponse>>>.Instance);

            var nextCalled = false;
            Task<AppResult<TestResponse>> Next(CancellationToken _)
            {
                nextCalled = true;
                return Task.FromResult(AppResult<TestResponse>.Success(new TestResponse("created")));
            }

            // Act
            var result = await behavior.Handle(new TestRequest(string.Empty), Next, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            result.Error!.Type.Should().Be(ErrorType.Invalid);
            result.Error.ValidationErrors.Should().ContainSingle(error => error.Identifier == nameof(TestRequest.Name));
            result.Content.Should().BeNull();
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_Should_Call_Next_When_Request_Is_Valid()
        {
            // Arrange
            var expected = AppResult<TestResponse>.Success(new TestResponse("created"));
            var behavior = new ValidationBehaviour<TestRequest, AppResult<TestResponse>>(
                [new TestRequestValidator()],
                NullLogger<ValidationBehaviour<TestRequest, AppResult<TestResponse>>>.Instance);

            Task<AppResult<TestResponse>> Next(CancellationToken _) => Task.FromResult(expected);

            // Act
            var result = await behavior.Handle(new TestRequest("recipe"), Next, CancellationToken.None);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid_And_Response_Is_Not_AppResult()
        {
            // Arrange
            var behavior = new ValidationBehaviour<PlainRequest, string>(
                [new PlainRequestValidator()],
                NullLogger<ValidationBehaviour<PlainRequest, string>>.Instance);

            Task<string> Next(CancellationToken _) => Task.FromResult("created");

            // Act
            Func<Task> act = async () => await behavior.Handle(new PlainRequest(string.Empty), Next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        private sealed record TestRequest(string Name) : IRequest<AppResult<TestResponse>>;

        private sealed record TestResponse(string Name);

        private sealed class TestRequestValidator : AbstractValidator<TestRequest>
        {
            public TestRequestValidator()
            {
                RuleFor(request => request.Name)
                    .NotEmpty()
                    .WithErrorCode("name-required");
            }
        }

        private sealed record PlainRequest(string Name) : IRequest<string>;

        private sealed class PlainRequestValidator : AbstractValidator<PlainRequest>
        {
            public PlainRequestValidator()
            {
                RuleFor(request => request.Name).NotEmpty();
            }
        }
    }
}
