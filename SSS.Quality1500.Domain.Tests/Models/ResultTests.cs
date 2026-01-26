namespace SSS.Quality1500.Domain.Tests.Models;

using FluentAssertions;
using SSS.Quality1500.Domain.Models;
using Xunit;

public sealed class ResultTests
{
    [Fact]
    public void OkShouldCreateSuccessResult()
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Ok(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void FailShouldCreateFailureResult()
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Fail("error");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void MatchWithSuccessResultShouldCallSuccessFunction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Ok(42);

        // Act
        string output = result.Match(
            onSuccess: value => $"Success: {value}",
            onFailure: error => $"Error: {error}"
        );

        // Assert
        output.Should().Be("Success: 42");
    }

    [Fact]
    public void MatchWithFailureResultShouldCallFailureFunction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Fail("error");

        // Act
        string output = result.Match(
            onSuccess: value => $"Success: {value}",
            onFailure: error => $"Error: {error}"
        );

        // Assert
        output.Should().Be("Error: error");
    }

    [Fact]
    public void OnSuccessWithSuccessResultShouldExecuteAction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Ok(42);
        int capturedValue = 0;

        // Act
        result.OnSuccess(value => capturedValue = value);

        // Assert
        capturedValue.Should().Be(42);
    }

    [Fact]
    public void OnSuccessWithFailureResultShouldNotExecuteAction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Fail("error");
        int capturedValue = 0;

        // Act
        result.OnSuccess(value => capturedValue = value);

        // Assert
        capturedValue.Should().Be(0);
    }

    [Fact]
    public void OnFailureWithFailureResultShouldExecuteAction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Fail("error");
        string capturedError = string.Empty;

        // Act
        result.OnFailure(error => capturedError = error);

        // Assert
        capturedError.Should().Be("error");
    }

    [Fact]
    public void OnFailureWithSuccessResultShouldNotExecuteAction()
    {
        // Arrange
        Result<int, string> result = Result<int, string>.Ok(42);
        string capturedError = string.Empty;

        // Act
        result.OnFailure(error => capturedError = error);

        // Assert
        capturedError.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void OkWithVariousValuesShouldStoreCorrectly(int value)
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Ok(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Match(
            onSuccess: v => v.Should().Be(value),
            onFailure: _ => throw new InvalidOperationException("Should not reach here")
        );
    }
}
