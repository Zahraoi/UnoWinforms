using Uno.Core.Game;
using Uno.Core.Players;

namespace Uno.Data.Models;

public sealed class StoredMatchResult
{
    public DateTimeOffset PlayedAtUtc { get; init; }

    public string? WinnerName { get; init; }

    public ScoringSystem ScoringSystem { get; init; }

    public int PlayerCount { get; init; }

    public IReadOnlyList<StoredMatchPlayerResult> Players { get; init; } = Array.Empty<StoredMatchPlayerResult>();
}

public sealed class StoredMatchPlayerResult
{
    public string PlayerName { get; init; } = string.Empty;

    public PlayerType PlayerType { get; init; }

    public int SeatNumber { get; init; }

    public int FinishRank { get; init; }

    public int Score { get; init; }

    public int CardsPlayed { get; init; }

    public int CardsDrawn { get; init; }

    public int TurnCount { get; init; }
}
