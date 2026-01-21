namespace SSS.Quality1500.Domain.Tests.Models;

using FluentAssertions;
using SSS.Quality1500.Domain.Models;
using Xunit;

public sealed class ResultTests
{
    [Fact]
    public void Ok_ShouldCreateSuccessResult()
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Ok(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Fail_ShouldCreateFailureResult()
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Fail("error");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Match_WithSuccessResult_ShouldCallSuccessFunction()
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
    public void Match_WithFailureResult_ShouldCallFailureFunction()
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
    public void OnSuccess_WithSuccessResult_ShouldExecuteAction()
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
    public void OnSuccess_WithFailureResult_ShouldNotExecuteAction()
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
    public void OnFailure_WithFailureResult_ShouldExecuteAction()
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
    public void OnFailure_WithSuccessResult_ShouldNotExecuteAction()
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
    public void Ok_WithVariousValues_ShouldStoreCorrectly(int value)
    {
        // Arrange & Act
        Result<int, string> result = Result<int, string>.Ok(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Match(
            onSuccess: v => v.Should().Be(value),
            onFailure: _ => throw new Exception("Should not reach here")
        );
    }
}