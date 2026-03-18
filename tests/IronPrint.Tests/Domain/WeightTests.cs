using FluentAssertions;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Tests.Domain;

public class WeightTests
{
    [Fact]
    public void FromKg_CreatesKgWeight()
    {
        var weight = Weight.FromKg(100);

        weight.Value.Should().Be(100);
        weight.Unit.Should().Be(WeightUnit.Kg);
    }

    [Fact]
    public void FromLb_CreatesLbWeight()
    {
        var weight = Weight.FromLb(220);

        weight.Value.Should().Be(220);
        weight.Unit.Should().Be(WeightUnit.Lb);
    }

    [Fact]
    public void NegativeValue_ThrowsArgumentException()
    {
        var act = () => Weight.FromKg(-1);

        act.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void Zero_IsKgWithValueZero()
    {
        Weight.Zero.Value.Should().Be(0);
        Weight.Zero.Unit.Should().Be(WeightUnit.Kg);
    }

    [Fact]
    public void ToLb_ConvertsFromKgCorrectly()
    {
        var weight = Weight.FromKg(100).ToLb();

        weight.Unit.Should().Be(WeightUnit.Lb);
        weight.Value.Should().Be(220.46m);
    }

    [Fact]
    public void ToKg_ConvertsFromLbCorrectly()
    {
        var weight = Weight.FromLb(100).ToKg();

        weight.Unit.Should().Be(WeightUnit.Kg);
        weight.Value.Should().Be(45.36m);
    }

    [Fact]
    public void ToKg_WhenAlreadyKg_ReturnsSameInstance()
    {
        var weight = Weight.FromKg(50);

        weight.ToKg().Should().BeSameAs(weight);
    }

    [Fact]
    public void ToLb_WhenAlreadyLb_ReturnsSameInstance()
    {
        var weight = Weight.FromLb(50);

        weight.ToLb().Should().BeSameAs(weight);
    }

    [Fact]
    public void ToString_IncludesValueAndUnit()
    {
        Weight.FromKg(75).ToString().Should().Be("75 Kg");
        Weight.FromLb(165).ToString().Should().Be("165 Lb");
    }
}
