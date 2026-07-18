using Microsoft.Data.SqlClient;
using Uno.Data.Configuration;
using Uno.Data.Models;

namespace Uno.Data.Repositories;

public sealed class PlayerRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PlayerRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<StoredPlayerProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT PlayerId, DisplayName, CreatedAtUtc
            FROM dbo.Players
            ORDER BY DisplayName;
            """;

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<StoredPlayerProfile>();
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new StoredPlayerProfile
            {
                PlayerId = reader.GetInt32(0),
                DisplayName = reader.GetString(1),
                CreatedAtUtc = reader.GetDateTimeOffset(2)
            });
        }

        return results;
    }

    public async Task<StoredPlayerProfile> GetOrCreateAsync(string displayName, CancellationToken cancellationToken = default)
    {
        var normalized = displayName.Trim();

        const string selectSql = """
            SELECT TOP (1) PlayerId, DisplayName, CreatedAtUtc
            FROM dbo.Players
            WHERE DisplayName = @DisplayName;
            """;

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        await using (var selectCommand = new SqlCommand(selectSql, connection))
        {
            selectCommand.Parameters.AddWithValue("@DisplayName", normalized);
            await using var reader = await selectCommand.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new StoredPlayerProfile
                {
                    PlayerId = reader.GetInt32(0),
                    DisplayName = reader.GetString(1),
                    CreatedAtUtc = reader.GetDateTimeOffset(2)
                };
            }
        }

        const string insertSql = """
            INSERT INTO dbo.Players (DisplayName)
            OUTPUT INSERTED.PlayerId, INSERTED.DisplayName, INSERTED.CreatedAtUtc
            VALUES (@DisplayName);
            """;

        await using var insertCommand = new SqlCommand(insertSql, connection);
        insertCommand.Parameters.AddWithValue("@DisplayName", normalized);
        await using var insertReader = await insertCommand.ExecuteReaderAsync(cancellationToken);
        await insertReader.ReadAsync(cancellationToken);

        return new StoredPlayerProfile
        {
            PlayerId = insertReader.GetInt32(0),
            DisplayName = insertReader.GetString(1),
            CreatedAtUtc = insertReader.GetDateTimeOffset(2)
        };
    }
}
