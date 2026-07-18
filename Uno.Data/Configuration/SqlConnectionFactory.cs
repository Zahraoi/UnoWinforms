using Microsoft.Data.SqlClient;

namespace Uno.Data.Configuration;

public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(DatabaseConnectionSettings settings)
    {
        _connectionString = settings.ConnectionString;
    }

    public string ConnectionString => _connectionString;

    public SqlConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
