using Uno.Core.Cards;

namespace Uno.WinForms.Ui;

public static class CardImageProvider
{
    private static readonly Dictionary<string, Image> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static Image? GetCardImage(Card card)
    {
        return GetByKey(GetCardFileName(card));
    }

    public static Image? GetBackImage()
    {
        return GetByKey("card_back.png");
    }

    private static Image? GetByKey(string fileName)
    {
        if (Cache.TryGetValue(fileName, out var cached))
        {
            return cached;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Cards", fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        var image = new Bitmap(path);
        Cache[fileName] = image;
        return image;
    }

    private static string GetCardFileName(Card card)
    {
        if (card.Color == CardColor.Wild)
        {
            return card.Face == CardFace.WildDrawFour ? "card_wild_drawfour.png" : "card_wild.png";
        }

        var color = card.Color.ToString().ToLowerInvariant();
        var face = card.Face switch
        {
            CardFace.Skip => "skip",
            CardFace.Reverse => "reverse",
            CardFace.DrawTwo => "drawtwo",
            _ => ((int)card.Face).ToString()
        };

        return $"card_{color}_{face}.png";
    }
}
