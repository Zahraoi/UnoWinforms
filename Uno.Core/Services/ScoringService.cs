using Uno.Core.Game;

namespace Uno.Core.Services;

public static class ScoringService
{
    public static int CalculateScore(GamePlayer player, GameSession session)
    {
        return session.Options.ScoringSystem switch
        {
            ScoringSystem.Basic => player.FinishRank ?? session.Players.Count,
            ScoringSystem.UnoValue => player.HandScore,
            _ => player.HandScore
        };
    }
}
