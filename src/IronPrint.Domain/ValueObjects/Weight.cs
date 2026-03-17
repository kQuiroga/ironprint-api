namespace IronPrint.Domain.ValueObjects;

public record Weight
{
    public decimal Value { get; }
    public WeightUnit Unit { get; }

    private Weight(decimal value, WeightUnit unit)
    {
        if (value < 0)
            throw new ArgumentException("El peso no puede ser negativo.", nameof(value));

        Value = value;
        Unit = unit;
    }

    public static Weight Zero => new(0, WeightUnit.Kg);
    public static Weight FromKg(decimal value) => new(value, WeightUnit.Kg);
    public static Weight FromLb(decimal value) => new(value, WeightUnit.Lb);

    public Weight ToKg() => Unit == WeightUnit.Kg ? this : new(Math.Round(Value * 0.453592m, 2), WeightUnit.Kg);
    public Weight ToLb() => Unit == WeightUnit.Lb ? this : new(Math.Round(Value * 2.20462m, 2), WeightUnit.Lb);

    public override string ToString() => $"{Value} {Unit}";
}

public enum WeightUnit
{
    Kg,
    Lb
}
