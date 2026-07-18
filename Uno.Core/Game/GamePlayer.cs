using Uno.Core.Cards;
using Uno.Core.Players;

namespace Uno.Core.Game;

public sealed class GamePlayer
{
    public GamePlayer(int seatNumber, PlayerDefinition definition)
    {
        SeatNumber = seatNumber;
        Definition = definition;
        Hand = new List<Card>();
    }

    public int SeatNumber { get; }

    public PlayerDefinition Definition { get; }

    public List<Card> Hand { get; }

    public int CardsPlayed { get; set; }

    public int CardsDrawn { get; set; }

    public int TurnCount { get; set; }

    public int? FinishRank { get; set; }

    public bool IsFinished => FinishRank.HasValue;

    public int HandScore => Hand.Sum(card => card.ScoringValue);
}
