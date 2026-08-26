namespace Hbk.Platform.Configuration;

public enum DatabaseProvider
{
    Sqlite,
    InMemory,
    PostgreSql
}

public static class DatabaseProviderConfiguration
{
    public static DatabaseProvider GetDatabaseProvider(this IConfiguration configuration)
    {
        var configuredProvider = configuration["Database:Provider"];

        if (Enum.TryParse<DatabaseProvider>(configuredProvider, ignoreCase: true, out var provider))
        {
            return provider;
        }

        throw new InvalidOperationException(
            $"Database:Provider must be one of {string.Join(", ", Enum.GetNames<DatabaseProvider>())}.");
    }

    public static string GetSqliteConnectionString(this IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("HbkSqlite");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var databaseDirectory = Path.Combine(Path.GetTempPath(), "ProjectHabakkuk");
        Directory.CreateDirectory(databaseDirectory);

        return $"Data Source={Path.Combine(databaseDirectory, "hbk-demo.db")}";
    }
}
