using Uno.Core.Cards;
using Uno.Core.Game;
using Uno.Core.Players;

namespace Uno.Core.Services;

public sealed class RuleEngine
{
    public GameSession StartNewGame(IReadOnlyList<PlayerDefinition> players, GameOptions options, int? randomSeed = null)
    {
        if (players.Count < 2 || players.Count > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(players), "Uno requires between 2 and 4 players.");
        }

        if (options.CardsPerPlayer < 1 || options.CardsPerPlayer > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Cards per player must be between 1 and 15.");
        }

        var gamePlayers = players.Select((player, index) => new GamePlayer(index, player)).ToList();
        var session = new GameSession(gamePlayers, options, randomSeed);
        var random = randomSeed.HasValue ? new Random(randomSeed.Value) : Random.Shared;

        var deck = DeckService.CreateStandardDeck();
        DeckService.Shuffle(deck, random);

        for (var dealRound = 0; dealRound < options.CardsPerPlayer; dealRound++)
        {
            foreach (var player in gamePlayers)
            {
                player.Hand.Add(deck[0]);
                deck.RemoveAt(0);
            }
        }

        foreach (var player in gamePlayers)
        {
            DeckService.SortHand(player.Hand);
        }

        while (deck[0].IsWild)
        {
            deck.Add(deck[0]);
            deck.RemoveAt(0);
        }

        session.DiscardPile.Add(deck[0]);
        deck.RemoveAt(0);
        session.DrawPile.AddRange(deck);
        session.CurrentColor = session.CurrentCard.Color;
        session.CurrentPlayerIndex = 0;
        session.CurrentPlayer.TurnCount++;
        session.LastAction = $"Game started with {session.CurrentCard}.";

        return session;
    }

    public IReadOnlyList<Card> GetPlayableCards(GameSession session, GamePlayer player)
    {
        return player.Hand.Where(card => CanPlayCard(session, player, card, null) == MoveStatus.Success)
            .OrderBy(card => card.SortingValue)
            .ToList();
    }

    public MoveStatus CanPlayCard(GameSession session, GamePlayer player, Card card, CardColor? chosenColor)
    {
        if (session.IsCompleted)
        {
            return MoveStatus.GameAlreadyFinished;
        }

        if (!ReferenceEquals(session.CurrentPlayer, player))
        {
            return MoveStatus.NotPlayersTurn;
        }

        if (!player.Hand.Contains(card))
        {
            return MoveStatus.CardNotInHand;
        }

        if (card.IsWild && chosenColor is null)
        {
            return MoveStatus.WildColorRequired;
        }

        if (!card.IsWild && card.Color != session.CurrentColor && card.Face != session.CurrentCard.Face)
        {
            return MoveStatus.WrongColorOrFace;
        }

        if (!session.Options.AllowDrawFourAnyTime && card.Face == CardFace.WildDrawFour)
        {
            var hasCurrentColor = player.Hand.Any(handCard => handCard.Color == session.CurrentColor && handCard.InstanceId != card.InstanceId);
            if (hasCurrentColor)
            {
                return MoveStatus.DrawFourNotAllowed;
            }
        }

        return MoveStatus.Success;
    }

    public MoveResult PlayCard(GameSession session, Guid cardInstanceId, CardColor? chosenColor = null)
    {
        if (session.IsCompleted)
        {
            return Failure(MoveStatus.GameAlreadyFinished, "The game is already complete.");
        }

        var player = session.CurrentPlayer;
        var card = player.Hand.FirstOrDefault(item => item.InstanceId == cardInstanceId);
        if (card is null)
        {
            return Failure(MoveStatus.InvalidCard, "The selected card was not found.");
        }

        var status = CanPlayCard(session, player, card, chosenColor);
        if (status != MoveStatus.Success)
        {
            return Failure(status, GetFailureMessage(status));
        }

        player.Hand.Remove(card);
        session.DiscardPile.Add(card);
        player.CardsPlayed++;
        session.CurrentColor = card.IsWild ? chosenColor!.Value : card.Color;
        session.LastAction = $"{player.Definition.Name} played {card}.";

        if (card.Face == CardFace.Zero && session.Options.ZeroRotatesHands)
        {
            RotateHands(session);
            session.LastAction = $"{player.Definition.Name} played 0 and rotated all active hands.";
        }

        if (player.Hand.Count == 0)
        {
            player.FinishRank = session.FinishedPlayerCount + 1;
            session.LastAction = $"{player.Definition.Name} went out.";
        }

        if (ShouldComplete(session))
        {
            FinalizeGame(session);
            return new MoveResult
            {
                Status = MoveStatus.Success,
                Message = session.LastAction,
                PlayedCard = card,
                TurnAdvanced = false,
                GameCompleted = true
            };
        }

        AdvanceTurnAfterPlay(session, card);
        return new MoveResult
        {
            Status = MoveStatus.Success,
            Message = session.LastAction,
            PlayedCard = card,
            TurnAdvanced = true,
            GameCompleted = session.IsCompleted
        };
    }

    public MoveResult DrawCardAndPass(GameSession session)
    {
        if (session.IsCompleted)
        {
            return Failure(MoveStatus.GameAlreadyFinished, "The game is already complete.");
        }

        if (!TryDrawCards(session, session.CurrentPlayer, 1, out var drawnCards))
        {
            return Failure(MoveStatus.CannotDraw, "No cards are available to draw.");
        }

        session.LastAction = $"{session.CurrentPlayer.Definition.Name} drew a card and passed.";
        AdvanceToNextActivePlayer(session, 1);
        session.CurrentPlayer.TurnCount++;

        return new MoveResult
        {
            Status = MoveStatus.Success,
            Message = session.LastAction,
            DrawnCards = drawnCards,
            TurnAdvanced = true,
            GameCompleted = false
        };
    }

    private static MoveResult Failure(MoveStatus status, string message)
    {
        return new MoveResult
        {
            Status = status,
            Message = message
        };
    }

    private static string GetFailureMessage(MoveStatus status)
    {
        return status switch
        {
            MoveStatus.CardNotInHand => "That card is not in the current player's hand.",
            MoveStatus.WrongColorOrFace => "That card does not match the current color or face.",
            MoveStatus.WildColorRequired => "Choose a color before playing a wild card.",
            MoveStatus.DrawFourNotAllowed => "Wild Draw 4 is only allowed when no card of the current color is available.",
            MoveStatus.NotPlayersTurn => "It is not that player's turn.",
            _ => "That move is not allowed."
        };
    }

    private void AdvanceTurnAfterPlay(GameSession session, Card card)
    {
        var stepCount = 1;

        switch (card.Face)
        {
            case CardFace.Reverse:
                session.Direction *= -1;
                if (session.ActivePlayerCount == 2)
                {
                    stepCount++;
                }

                session.LastAction = $"{session.CurrentPlayer.Definition.Name} reversed play order.";
                break;

            case CardFace.Skip:
                stepCount++;
                session.LastAction = $"{session.CurrentPlayer.Definition.Name} skipped the next player.";
                break;

            case CardFace.DrawTwo:
                ApplyForcedDraw(session, 2);
                stepCount++;
                session.LastAction = $"{session.CurrentPlayer.Definition.Name} forced the next player to draw 2.";
                break;

            case CardFace.Wild:
                session.LastAction = $"{session.CurrentPlayer.Definition.Name} changed the color to {session.CurrentColor}.";
                break;

            case CardFace.WildDrawFour:
                ApplyForcedDraw(session, 4);
                stepCount++;
                session.LastAction = $"{session.CurrentPlayer.Definition.Name} changed the color to {session.CurrentColor} and forced a draw 4.";
                break;
        }

        AdvanceToNextActivePlayer(session, stepCount);
        session.CurrentPlayer.TurnCount++;
    }

    private void ApplyForcedDraw(GameSession session, int count)
    {
        var targetIndex = FindNextActiveIndex(session, session.CurrentPlayerIndex, 1);
        var targetPlayer = session.Players[targetIndex];
        TryDrawCards(session, targetPlayer, count, out _);
    }

    private bool TryDrawCards(GameSession session, GamePlayer player, int count, out IReadOnlyList<Card> drawnCards)
    {
        var results = new List<Card>(count);

        for (var drawNumber = 0; drawNumber < count; drawNumber++)
        {
            if (!EnsureDrawPileAvailable(session))
            {
                drawnCards = results;
                return false;
            }

            var card = session.DrawPile[0];
            session.DrawPile.RemoveAt(0);
            player.Hand.Add(card);
            player.CardsDrawn++;
            results.Add(card);
        }

        DeckService.SortHand(player.Hand);
        drawnCards = results;
        return true;
    }

    private static bool EnsureDrawPileAvailable(GameSession session)
    {
        if (session.DrawPile.Count > 0)
        {
            return true;
        }

        if (session.DiscardPile.Count <= 1)
        {
            return false;
        }

        var cardsToRecycle = session.DiscardPile.Take(session.DiscardPile.Count - 1).ToList();
        session.DiscardPile.RemoveRange(0, session.DiscardPile.Count - 1);
        DeckService.Shuffle(cardsToRecycle, Random.Shared);
        session.DrawPile.AddRange(cardsToRecycle);

        return session.DrawPile.Count > 0;
    }

    private static int FindNextActiveIndex(GameSession session, int startIndex, int stepCount)
    {
        var index = startIndex;
        for (var step = 0; step < stepCount; step++)
        {
            do
            {
                index += session.Direction;

                if (index >= session.Players.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = session.Players.Count - 1;
                }
            }
            while (session.Players[index].IsFinished);
        }

        return index;
    }

    private static void AdvanceToNextActivePlayer(GameSession session, int steps)
    {
        session.CurrentPlayerIndex = FindNextActiveIndex(session, session.CurrentPlayerIndex, steps);
    }

    private static void RotateHands(GameSession session)
    {
        var activePlayers = session.Players.Where(player => !player.IsFinished).OrderBy(player => player.SeatNumber).ToList();
        if (activePlayers.Count <= 1)
        {
            return;
        }

        var clonedHands = activePlayers.Select(player => player.Hand.ToList()).ToList();
        for (var index = 0; index < activePlayers.Count; index++)
        {
            var sourceIndex = session.Direction == 1
                ? (index + activePlayers.Count - 1) % activePlayers.Count
                : (index + 1) % activePlayers.Count;

            activePlayers[index].Hand.Clear();
            activePlayers[index].Hand.AddRange(clonedHands[sourceIndex]);
            DeckService.SortHand(activePlayers[index].Hand);
        }
    }

    private static bool ShouldComplete(GameSession session)
    {
        return session.Options.StopAfterFirstWinner
            ? session.FinishedPlayerCount > 0
            : session.ActivePlayerCount <= 1;
    }

    private static void FinalizeGame(GameSession session)
    {
        var nextRank = session.FinishedPlayerCount + 1;
        foreach (var player in session.Players
                     .Where(player => !player.IsFinished)
                     .OrderBy(player => player.Hand.Count)
                     .ThenBy(player => player.HandScore)
                     .ThenBy(player => player.SeatNumber))
        {
            player.FinishRank = nextRank++;
        }

        session.IsCompleted = true;
        session.CompletedAtUtc = DateTimeOffset.UtcNow;
    }
}
