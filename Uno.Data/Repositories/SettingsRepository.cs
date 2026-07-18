using Microsoft.Data.SqlClient;
using Uno.Data.Configuration;

namespace Uno.Data.Repositories;

public sealed class SettingsRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SettingsRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Dictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT SettingKey, SettingValue FROM dbo.Settings ORDER BY SettingKey;";

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (await reader.ReadAsync(cancellationToken))
        {
            settings[reader.GetString(0)] = reader.GetString(1);
        }

        return settings;
    }

    public async Task UpsertAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        const string sql = """
            MERGE dbo.Settings AS target
            USING (SELECT @SettingKey AS SettingKey, @SettingValue AS SettingValue) AS source
            ON target.SettingKey = source.SettingKey
            WHEN MATCHED THEN
                UPDATE SET SettingValue = source.SettingValue, UpdatedAtUtc = SYSUTCDATETIME()
            WHEN NOT MATCHED THEN
                INSERT (SettingKey, SettingValue) VALUES (source.SettingKey, source.SettingValue);
            """;

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@SettingKey", key);
        command.Parameters.AddWithValue("@SettingValue", value);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
