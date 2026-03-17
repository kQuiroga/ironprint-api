namespace IronPrint.Domain.Common;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Se proporcionó un valor nulo.");

    public static Error NotFound(string resource) =>
        new($"{resource}.NotFound", $"{resource} no encontrado.");

    public static Error Conflict(string resource) =>
        new($"{resource}.Conflict", $"{resource} ya existe.");

    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);
}
