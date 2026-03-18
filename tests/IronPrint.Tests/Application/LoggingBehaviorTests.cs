using FluentAssertions;
using IronPrint.Application.Behaviors;
using IronPrint.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IronPrint.Tests.Application;

public class LoggingBehaviorTests
{
    private readonly ILogger<LoggingBehavior<TestRequest, Result<string>>> _logger =
        Substitute.For<ILogger<LoggingBehavior<TestRequest, Result<string>>>>();

    private LoggingBehavior<TestRequest, Result<string>> CreateSut() => new(_logger);

    [Fact]
    public async Task Handle_SuccessResult_ReturnsResponse()
    {
        var sut = CreateSut();
        var expected = Result.Success("ok");
        RequestHandlerDelegate<Result<string>> next = _ => Task.FromResult(expected);

        var result = await sut.Handle(new TestRequest(), next, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_FailureResult_ReturnsFailureWithoutThrowing()
    {
        var sut = CreateSut();
        var failure = Result.Failure<string>(Error.NotFound("Exercise"));
        RequestHandlerDelegate<Result<string>> next = _ => Task.FromResult(failure);

        var result = await sut.Handle(new TestRequest(), next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(failure.Error);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_Rethrows()
    {
        var sut = CreateSut();
        RequestHandlerDelegate<Result<string>> next = _ => throw new InvalidOperationException("boom");

        var act = async () => await sut.Handle(new TestRequest(), next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
    }

    public record TestRequest : IRequest<Result<string>>;
}
