using Uno.Core.Cards;

namespace Uno.Core.Services;

public static class DeckService
{
    public static List<Card> CreateStandardDeck()
    {
        var deck = new List<Card>(50);

        foreach (var color in new[] { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue })
        {
            for (var value = CardFace.Zero; value <= CardFace.Nine; value++)
            {
                deck.Add(new Card(color, value));
            }
        }

        for (var index = 0; index < 2; index++)
        {
            deck.Add(new Card(CardColor.Red, CardFace.Skip));
            deck.Add(new Card(CardColor.Yellow, CardFace.Reverse));
            deck.Add(new Card(CardColor.Green, CardFace.DrawTwo));
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
