namespace Uno.Core.Game;

public sealed class GameOptions
{
    public int CardsPerPlayer { get; set; } = 7;

    public bool HighlightPlayableCards { get; set; } = true;

    public bool UseAnimations { get; set; } = true;

    public int ComputerPlayerDelayMs { get; set; } = 900;

    public bool StopAfterFirstWinner { get; set; } = true;

    public bool AllowDrawFourAnyTime { get; set; }

    public bool ZeroRotatesHands { get; set; }

    public ScoringSystem ScoringSystem { get; set; } = ScoringSystem.Basic;

    public GameOptions Clone()
    {
        return (GameOptions)MemberwiseClone();
    }
}
