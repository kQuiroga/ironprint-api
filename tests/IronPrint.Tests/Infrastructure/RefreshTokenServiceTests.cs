using FluentAssertions;
using IronPrint.Infrastructure.Identity;

namespace IronPrint.Tests.Infrastructure;

public class RefreshTokenServiceTests
{
    private readonly RefreshTokenService _sut = new();

    [Fact]
    public void GenerateToken_ReturnsNonEmptyString()
    {
        var token = _sut.GenerateToken();

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_EachCallReturnsUniqueToken()
    {
        var tokens = Enumerable.Range(0, 10).Select(_ => _sut.GenerateToken()).ToList();

        tokens.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GenerateToken_HasSufficientLength()
    {
        // 64 bytes en base64 = 88 chars
        var token = _sut.GenerateToken();

        token.Length.Should().BeGreaterThanOrEqualTo(80);
    }

    [Fact]
    public void HashToken_IsDeterministic()
    {
        var token = _sut.GenerateToken();

        var hash1 = _sut.HashToken(token);
        var hash2 = _sut.HashToken(token);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void HashToken_DifferentTokensProduceDifferentHashes()
    {
        var hash1 = _sut.HashToken(_sut.GenerateToken());
        var hash2 = _sut.HashToken(_sut.GenerateToken());

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashToken_ReturnsLowercaseHex()
    {
        var hash = _sut.HashToken("cualquier-token");

        hash.Should().MatchRegex("^[0-9a-f]+$");
    }

    [Fact]
    public void HashToken_IsSha256Length()
    {
        // SHA-256 = 32 bytes = 64 hex chars
        var hash = _sut.HashToken("cualquier-token");

        hash.Length.Should().Be(64);
    }
}
