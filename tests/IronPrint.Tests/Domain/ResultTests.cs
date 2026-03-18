using FluentAssertions;
using IronPrint.Domain.Common;

namespace IronPrint.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_IsSuccess_True()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_IsFailure_True()
    {
        var error = Error.NotFound("Routine");
        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessT_Value_Accessible()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureT_Value_ThrowsInvalidOperation()
    {
        var result = Result.Failure<int>(Error.NotFound("Exercise"));

        result.Invoking(r => _ = r.Value)
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Success_WithError_ThrowsInvalidOperation()
    {
        var act = () => new TestResult(true, Error.NotFound("X"));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_WithNoneError_ThrowsInvalidOperation()
    {
        var act = () => new TestResult(false, Error.None);

        act.Should().Throw<InvalidOperationException>();
    }

    // Subclase de acceso para testear el constructor protegido
    private sealed class TestResult : Result
    {
        public TestResult(bool isSuccess, Error error) : base(isSuccess, error) { }
    }
}
