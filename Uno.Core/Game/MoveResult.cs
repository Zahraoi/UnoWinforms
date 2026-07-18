using Uno.Core.Cards;

namespace Uno.Core.Game;

public sealed class MoveResult
{
    public MoveStatus Status { get; init; }

    public string Message { get; init; } = string.Empty;

    public Card? PlayedCard { get; init; }

    public IReadOnlyList<Card> DrawnCards { get; init; } = Array.Empty<Card>();

    public bool TurnAdvanced { get; init; }

    public bool GameCompleted { get; init; }
}
