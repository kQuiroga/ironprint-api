using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Persistence;

public static class DapperConfig
{
    public static void Configure()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        // Mapeo explícito para IdentityUser (snake_case → PascalCase)
        SqlMapper.SetTypeMap(typeof(IdentityUser), new CustomPropertyTypeMap(
            typeof(IdentityUser),
            (type, columnName) =>
            {
                var normalized = columnName.Replace("_", string.Empty);
                return type.GetProperties()
                    .FirstOrDefault(p => p.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase))!;
            }));
    }
}

public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
        => parameter.Value = value.ToDateTime(TimeOnly.MinValue);

    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);
}
