using Microsoft.Data.SqlClient;
using Uno.Core.Game;
using Uno.Core.Services;
using Uno.Data.Configuration;

namespace Uno.Data.Repositories;

public sealed class MatchRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly PlayerRepository _playerRepository;

    public MatchRepository(SqlConnectionFactory connectionFactory, PlayerRepository playerRepository)
    {
        _connectionFactory = connectionFactory;
        _playerRepository = playerRepository;
    }

    public async Task<int> SaveCompletedMatchAsync(GameSession session, CancellationToken cancellationToken = default)
    {
        if (!session.IsCompleted)
        {
            throw new InvalidOperationException("Only completed sessions can be saved.");
        }

        var playerProfiles = new Dictionary<int, int>();
        foreach (var player in session.Players)
        {
            var profile = await _playerRepository.GetOrCreateAsync(player.Definition.Name, cancellationToken);
            playerProfiles[player.SeatNumber] = profile.PlayerId;
        }

        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            const string matchSql = """
                INSERT INTO dbo.Matches (PlayedAtUtc, WinnerPlayerId, ScoringSystem, PlayerCount, Notes)
                OUTPUT INSERTED.MatchId
                VALUES (@PlayedAtUtc, @WinnerPlayerId, @ScoringSystem, @PlayerCount, @Notes);
                """;

            var winner = session.GetOrderedResults().FirstOrDefault();
            var winnerPlayerId = winner is null ? (object)DBNull.Value : playerProfiles[winner.SeatNumber];

            await using var matchCommand = new SqlCommand(matchSql, connection, (SqlTransaction)transaction);
            matchCommand.Parameters.AddWithValue("@PlayedAtUtc", session.CompletedAtUtc ?? DateTimeOffset.UtcNow);
            matchCommand.Parameters.AddWithValue("@WinnerPlayerId", winnerPlayerId);
            matchCommand.Parameters.AddWithValue("@ScoringSystem", session.Options.ScoringSystem.ToString());
            matchCommand.Parameters.AddWithValue("@PlayerCount", session.Players.Count);
            matchCommand.Parameters.AddWithValue("@Notes", "Saved by Uno WinForms v1");

            var matchId = (int)(await matchCommand.ExecuteScalarAsync(cancellationToken))!;

            const string playerSql = """
                INSERT INTO dbo.MatchPlayers
                (
                    MatchId,
                    PlayerId,
                    SeatNumber,
                    FinishRank,
                    Score,
                    CardsPlayed,
                    CardsDrawn,
                    TurnCount,
                    PlayerType
                )
                VALUES
                (
                    @MatchId,
                    @PlayerId,
                    @SeatNumber,
                    @FinishRank,
                    @Score,
                    @CardsPlayed,
                    @CardsDrawn,
                    @TurnCount,
                    @PlayerType
                );
                """;

            foreach (var player in session.Players)
            {
                await using var playerCommand = new SqlCommand(playerSql, connection, (SqlTransaction)transaction);
                playerCommand.Parameters.AddWithValue("@MatchId", matchId);
                playerCommand.Parameters.AddWithValue("@PlayerId", playerProfiles[player.SeatNumber]);
                playerCommand.Parameters.AddWithValue("@SeatNumber", player.SeatNumber + 1);
                playerCommand.Parameters.AddWithValue("@FinishRank", player.FinishRank ?? session.Players.Count);
                playerCommand.Parameters.AddWithValue("@Score", ScoringService.CalculateScore(player, session));
                playerCommand.Parameters.AddWithValue("@CardsPlayed", player.CardsPlayed);
                playerCommand.Parameters.AddWithValue("@CardsDrawn", player.CardsDrawn);
                playerCommand.Parameters.AddWithValue("@TurnCount", player.TurnCount);
                playerCommand.Parameters.AddWithValue("@PlayerType", player.Definition.Type.ToString());
                await playerCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return matchId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
