using Uno.Core.Cards;
using Uno.Core.Game;
using Uno.Core.Players;

namespace Uno.Core.Services;

public sealed class ComputerPlayerService
{
    public (Card? card, CardColor? chosenColor) ChooseMove(GameSession session, RuleEngine ruleEngine)
    {
        var player = session.CurrentPlayer;
        if (player.Definition.Type == PlayerType.Human)
        {
            return (null, null);
        }

        var playableCards = ruleEngine.GetPlayableCards(session, player).ToList();
        if (playableCards.Count == 0)
        {
            return (null, null);
        }

        var selectedCard = player.Definition.Type == PlayerType.SmartComputer
            ? SelectSmartCard(player, playableCards, session.CurrentColor, session.CurrentCard.Face)
            : playableCards[0];

        if (!selectedCard.IsWild)
        {
            return (selectedCard, null);
        }

        return (selectedCard, ChooseBestColor(player));
    }

    private static Card SelectSmartCard(GamePlayer player, IReadOnlyList<Card> playableCards, CardColor currentColor, CardFace currentFace)
    {
        var sameColor = playableCards.Where(card => card.Color == currentColor && !card.IsWild)
            .OrderByDescending(card => card.ScoringValue)
            .ToList();
        if (sameColor.Count > 0)
        {
            return sameColor[0];
        }

        var sameFace = playableCards.Where(card => card.Face == currentFace && !card.IsWild)
            .OrderByDescending(card => card.ScoringValue)
            .ToList();
        if (sameFace.Count > 0)
        {
            return sameFace[0];
        }

        return playableCards.OrderByDescending(card => card.ScoringValue).First();
    }

    private static CardColor ChooseBestColor(GamePlayer player)
    {
        var grouped = player.Hand
            .Where(card => !card.IsWild)
            .GroupBy(card => card.Color)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .FirstOrDefault();

        return grouped?.Key ?? CardColor.Red;
    }
}
