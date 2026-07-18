using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Uno.Data.Configuration;

namespace Uno.Data.Services;

public sealed class DatabaseInitializer
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DatabaseInitializer(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var builder = new SqlConnectionStringBuilder(_connectionFactory.ConnectionString);
        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            throw new InvalidOperationException("Connection string must include an Initial Catalog.");
        }

        var databaseName = builder.InitialCatalog;
        var masterBuilder = new SqlConnectionStringBuilder(_connectionFactory.ConnectionString)
        {
            InitialCatalog = "master"
        };

        var escapedDatabaseName = databaseName.Replace("]", "]]", StringComparison.Ordinal);
        var createDatabaseSql = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{escapedDatabaseName}];";

        await using (var masterConnection = new SqlConnection(masterBuilder.ConnectionString))
        {
            await masterConnection.OpenAsync(cancellationToken);
            await using var createCommand = new SqlCommand(createDatabaseSql, masterConnection);
            await createCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        var scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "CreateSchema.sql");
        var scriptText = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        var batches = Regex.Split(scriptText, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        foreach (var batch in batches)
        {
            var statement = batch.Trim();
            if (statement.Length == 0)
            {
                continue;
            }

            await using var command = new SqlCommand(statement, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
