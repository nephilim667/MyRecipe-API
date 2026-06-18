using FluentAssertions;

using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;

namespace Application.Tests.Results
{
    public class AppResultTests
    {
        [Fact]
        public void Success_Should_Create_Success_Result_Without_Error()
        {
            // Act
            var result = AppResult.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Error.Should().BeNull();
        }

        [Fact]
        public void Failure_Should_Create_Failure_Result_With_Error()
        {
            // Arrange
            var error = new ErrorResult("Something failed", ErrorType.Error);

            // Act
            var result = AppResult.Failure(error);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(error);
        }

        [Fact]
        public void GenericSuccess_Should_Create_Success_Result_With_Content()
        {
            // Arrange
            var content = new TestContent("mocked-token");

            // Act
            var result = AppResult<TestContent>.Success(content);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Content.Should().Be(content);
            result.Error.Should().BeNull();
        }

        [Fact]
        public void GenericFailure_Should_Create_Failure_Result_Without_Content()
        {
            // Arrange
            var error = new ErrorResult("Something failed", ErrorType.Error);

            // Act
            var result = AppResult<TestContent>.Failure(error);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Content.Should().BeNull();
            result.Error.Should().Be(error);
        }

        [Fact]
        public void TryFailure_Should_Create_Base_Failure_Result()
        {
            // Arrange
            var error = new ErrorResult("Something failed", ErrorType.Error);

            // Act
            var created = AppResult.TryFailure<AppResult>(error, out var result);

            // Assert
            created.Should().BeTrue();
            result.Should().NotBeNull();
            result!.IsFailure.Should().BeTrue();
            result.Error.Should().Be(error);
        }

        [Fact]
        public void TryFailure_Should_Create_Generic_Failure_Result()
        {
            // Arrange
            var error = new ErrorResult("Something failed", ErrorType.Error);

            // Act
            var created = AppResult.TryFailure<AppResult<TestContent>>(error, out var result);

            // Assert
            created.Should().BeTrue();
            result.Should().NotBeNull();
            result!.IsFailure.Should().BeTrue();
            result.Error.Should().Be(error);
            result.Content.Should().BeNull();
        }

        [Fact]
        public void TryFailure_Should_Return_False_For_Non_Result_Type()
        {
            // Arrange
            var error = new ErrorResult("Something failed", ErrorType.Error);

            // Act
            var created = AppResult.TryFailure<string>(error, out var result);

            // Assert
            created.Should().BeFalse();
            result.Should().BeNull();
        }

        private sealed record TestContent(string Token);
    }
}
