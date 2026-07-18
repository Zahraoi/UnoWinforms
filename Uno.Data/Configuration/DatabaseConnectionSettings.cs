namespace Uno.Data.Configuration;

public sealed class DatabaseConnectionSettings
{
    public string ConnectionString { get; init; } =
        Environment.GetEnvironmentVariable("UNO_CONNECTION_STRING")
        ?? "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=UnoWinFormsDb;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true";
}
