using FluentAssertions;
using IronPrint.Domain.Ports;

namespace IronPrint.Tests.Domain;

public class RefreshTokenRecordTests
{
    [Fact]
    public void IsValid_NotRevokedAndNotExpired_ReturnsTrue()
    {
        var record = new RefreshTokenRecord(Guid.NewGuid(), "user1", DateTime.UtcNow.AddDays(30), null);

        record.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_Revoked_ReturnsFalse()
    {
        var record = new RefreshTokenRecord(Guid.NewGuid(), "user1", DateTime.UtcNow.AddDays(30), DateTime.UtcNow);

        record.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_Expired_ReturnsFalse()
    {
        var record = new RefreshTokenRecord(Guid.NewGuid(), "user1", DateTime.UtcNow.AddDays(-1), null);

        record.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_RevokedAndExpired_ReturnsFalse()
    {
        var record = new RefreshTokenRecord(Guid.NewGuid(), "user1", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(-2));

        record.IsValid.Should().BeFalse();
    }
}
