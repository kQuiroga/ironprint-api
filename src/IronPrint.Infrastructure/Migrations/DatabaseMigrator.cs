using DbUp;
using DbUp.Engine;

namespace IronPrint.Infrastructure.Migrations;

public static class DatabaseMigrator
{
    public static void Migrate(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        UpgradeEngine upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseMigrator).Assembly)
            .WithTransaction()
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new InvalidOperationException(
                $"Error al ejecutar migraciones: {result.Error.Message}", result.Error);
    }
}
