using Uno.Core.Cards;

namespace Uno.Core.Services;

public static class DeckService
{
    public static List<Card> CreateStandardDeck()
    {
        var deck = new List<Card>(108);

        foreach (var color in new[] { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue })
        {
            deck.Add(new Card(color, CardFace.Zero));

            for (var value = CardFace.One; value <= CardFace.DrawTwo; value++)
            {
                deck.Add(new Card(color, value));
                deck.Add(new Card(color, value));
            }
        }

        for (var index = 0; index < 4; index++)
        {
            deck.Add(new Card(CardColor.Wild, CardFace.Wild));
            deck.Add(new Card(CardColor.Wild, CardFace.WildDrawFour));
        }

        return deck;
    }

    public static void Shuffle(IList<Card> cards, Random random)
    {
        for (var index = cards.Count - 1; index > 0; index--)
        {
            var swapIndex = random.Next(index + 1);
            (cards[index], cards[swapIndex]) = (cards[swapIndex], cards[index]);
        }
    }

    public static void SortHand(List<Card> cards)
    {
        cards.Sort((left, right) => left.SortingValue.CompareTo(right.SortingValue));
    }
}
