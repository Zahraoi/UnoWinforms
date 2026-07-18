using Uno.Core.Game;
using Uno.Data.Configuration;
using Uno.Data.Repositories;
using Uno.Data.Services;

namespace Uno.WinForms.Services;

public sealed class GamePersistenceService
{
    private readonly PlayerRepository _playerRepository;
    private readonly MatchRepository _matchRepository;
    private readonly SettingsRepository _settingsRepository;
    private readonly DatabaseInitializer _databaseInitializer;

    public GamePersistenceService()
    {
        var settings = new DatabaseConnectionSettings();
        var connectionFactory = new SqlConnectionFactory(settings);
        _playerRepository = new PlayerRepository(connectionFactory);
        _settingsRepository = new SettingsRepository(connectionFactory);
        _matchRepository = new MatchRepository(connectionFactory, _playerRepository);
        _databaseInitializer = new DatabaseInitializer(connectionFactory);
    }

    public bool DatabaseAvailable { get; private set; }

    public string StatusMessage { get; private set; } = "Database not initialized.";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _databaseInitializer.InitializeAsync(cancellationToken);
            DatabaseAvailable = true;
            StatusMessage = "SQL Server is ready. Matches and players will be saved.";
        }
        catch (Exception ex)
        {
            DatabaseAvailable = false;
            StatusMessage = $"SQL Server unavailable: {ex.Message}";
        }
    }

    public async Task SaveCompletedMatchAsync(GameSession session, CancellationToken cancellationToken = default)
    {
        if (!DatabaseAvailable)
        {
            return;
        }

        await _matchRepository.SaveCompletedMatchAsync(session, cancellationToken);
    }

    public async Task SaveOptionAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        if (!DatabaseAvailable)
        {
            return;
        }

        await _settingsRepository.UpsertAsync(key, value, cancellationToken);
    }
}
