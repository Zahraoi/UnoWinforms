using Uno.Core.Cards;
using Uno.Core.Game;
using Uno.Core.Players;
using Uno.Core.Services;

namespace Uno.Tests;

public sealed class RuleEngineTests
{
    private readonly RuleEngine _ruleEngine = new();

    [Fact]
    public void StartNewGame_DealsCardsAndSetsOpeningState()
    {
        var session = _ruleEngine.StartNewGame(CreatePlayers(), new GameOptions(), 42);

        Assert.Equal(4, session.Players.Count);
        Assert.All(session.Players, player => Assert.Equal(7, player.Hand.Count));
        Assert.Single(session.DiscardPile);
        Assert.Equal(session.CurrentCard.Color, session.CurrentColor);
        Assert.False(session.CurrentCard.IsWild);
    }

    [Fact]
    public void PlayCard_RejectsWildDrawFourWhenCurrentColorExists()
    {
        var session = _ruleEngine.StartNewGame(CreatePlayers(2), new GameOptions { AllowDrawFourAnyTime = false }, 7);
        var player = session.CurrentPlayer;

        player.Hand.Clear();
        session.DiscardPile.Clear();
        session.DiscardPile.Add(new Card(CardColor.Red, CardFace.Five));
        session.CurrentColor = CardColor.Red;

        var redTwo = new Card(CardColor.Red, CardFace.Two);
        var wildDrawFour = new Card(CardColor.Wild, CardFace.WildDrawFour);
        player.Hand.Add(redTwo);
        player.Hand.Add(wildDrawFour);

        var result = _ruleEngine.PlayCard(session, wildDrawFour.InstanceId, CardColor.Blue);

        Assert.Equal(MoveStatus.DrawFourNotAllowed, result.Status);
    }

    [Fact]
    public void PlayCard_AppliesDrawTwoAndSkipsTargetTurn()
    {
        var session = _ruleEngine.StartNewGame(CreatePlayers(3), new GameOptions { StopAfterFirstWinner = false }, 5);
        var currentPlayer = session.CurrentPlayer;
        var nextPlayer = session.Players[1];

        currentPlayer.Hand.Clear();
        session.DiscardPile.Clear();
        session.DiscardPile.Add(new Card(CardColor.Green, CardFace.Four));
        session.CurrentColor = CardColor.Green;

        var drawTwo = new Card(CardColor.Green, CardFace.DrawTwo);
        currentPlayer.Hand.Add(drawTwo);
        var nextPlayerStartingCount = nextPlayer.Hand.Count;

        var result = _ruleEngine.PlayCard(session, drawTwo.InstanceId);

        Assert.Equal(MoveStatus.Success, result.Status);
        Assert.Equal(nextPlayerStartingCount + 2, nextPlayer.Hand.Count);
        Assert.Equal(session.Players[2], session.CurrentPlayer);
    }

    [Fact]
    public void DrawCardAndPass_AddsCardAndAdvancesTurn()
    {
        var session = _ruleEngine.StartNewGame(CreatePlayers(2), new GameOptions(), 11);
        var currentPlayer = session.CurrentPlayer;
        var startingCount = currentPlayer.Hand.Count;

        var result = _ruleEngine.DrawCardAndPass(session);

        Assert.Equal(MoveStatus.Success, result.Status);
        Assert.Equal(startingCount + 1, currentPlayer.Hand.Count);
        Assert.Single(result.DrawnCards);
        Assert.Equal(session.Players[1], session.CurrentPlayer);
    }

    [Fact]
    public void PlayCard_CompletesGameWhenFirstWinnerOptionEnabled()
    {
        var session = _ruleEngine.StartNewGame(CreatePlayers(2), new GameOptions { StopAfterFirstWinner = true }, 21);
        var currentPlayer = session.CurrentPlayer;

        currentPlayer.Hand.Clear();
        session.DiscardPile.Clear();
        session.DiscardPile.Add(new Card(CardColor.Blue, CardFace.Three));
        session.CurrentColor = CardColor.Blue;

        var winningCard = new Card(CardColor.Blue, CardFace.Nine);
        currentPlayer.Hand.Add(winningCard);

        var result = _ruleEngine.PlayCard(session, winningCard.InstanceId);

        Assert.True(result.GameCompleted);
        Assert.True(session.IsCompleted);
        Assert.Equal(1, currentPlayer.FinishRank);
        Assert.NotNull(session.CompletedAtUtc);
    }

    private static IReadOnlyList<PlayerDefinition> CreatePlayers(int count = 4)
    {
        var players = new List<PlayerDefinition>();
        for (var index = 0; index < count; index++)
        {
            players.Add(new PlayerDefinition($"Player {index + 1}", index == 0 ? PlayerType.Human : PlayerType.Computer));
        }

        return players;
    }
}
