using Uno.Core.Cards;

namespace Uno.Core.Game;

public sealed class GameSession
{
    public GameSession(IReadOnlyList<GamePlayer> players, GameOptions options, int? randomSeed = null)
    {
        SessionId = Guid.NewGuid();
        Players = players;
        Options = options;
        DrawPile = new List<Card>();
        DiscardPile = new List<Card>();
        RandomSeed = randomSeed;
    }

    public Guid SessionId { get; }

    public IReadOnlyList<GamePlayer> Players { get; }

    public GameOptions Options { get; }

    public List<Card> DrawPile { get; }

    public List<Card> DiscardPile { get; }

    public int CurrentPlayerIndex { get; set; }

    public int Direction { get; set; } = 1;

    public CardColor CurrentColor { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset StartedAtUtc { get; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAtUtc { get; set; }

    public int? RandomSeed { get; }

    public string LastAction { get; set; } = string.Empty;

    public GamePlayer CurrentPlayer => Players[CurrentPlayerIndex];

    public Card CurrentCard => DiscardPile[^1];

    public int FinishedPlayerCount => Players.Count(player => player.IsFinished);

    public int ActivePlayerCount => Players.Count(player => !player.IsFinished);

    public IEnumerable<GamePlayer> GetOrderedResults() => Players
        .OrderBy(player => player.FinishRank ?? int.MaxValue)
        .ThenBy(player => player.Hand.Count)
        .ThenBy(player => player.HandScore)
        .ThenBy(player => player.SeatNumber);
}
