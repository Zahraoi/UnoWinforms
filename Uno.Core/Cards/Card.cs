using System.Globalization;

namespace Uno.Core.Cards;

public sealed class Card
{
    public Card(CardColor color, CardFace face)
    {
        Color = color;
        Face = face;
        InstanceId = Guid.NewGuid();
    }

    public Guid InstanceId { get; }

    public CardColor Color { get; }

    public CardFace Face { get; }

    public bool IsWild => Color == CardColor.Wild;

    public int SortingValue => ((int)Color * 100) + (int)Face;

    public int ScoringValue => Face switch
    {
        CardFace.Zero => 0,
        CardFace.One => 1,
        CardFace.Two => 2,
        CardFace.Three => 3,
        CardFace.Four => 4,
        CardFace.Five => 5,
        CardFace.Six => 6,
        CardFace.Seven => 7,
        CardFace.Eight => 8,
        CardFace.Nine => 9,
        CardFace.Skip => 20,
        CardFace.Reverse => 20,
        CardFace.DrawTwo => 20,
        CardFace.Wild => 50,
        CardFace.WildDrawFour => 50,
        _ => 0
    };

    public override string ToString()
    {
        if (IsWild)
        {
            return Face switch
            {
                CardFace.WildDrawFour => "Wild Draw 4",
                _ => "Wild"
            };
        }

        var colorName = Color.ToString();
        var faceName = Face <= CardFace.Nine
            ? ((int)Face).ToString(CultureInfo.InvariantCulture)
            : Face switch
            {
                CardFace.DrawTwo => "Draw 2",
                _ => Face.ToString()
            };

        return $"{colorName} {faceName}";
    }
}
