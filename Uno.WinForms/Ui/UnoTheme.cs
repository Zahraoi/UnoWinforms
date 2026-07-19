using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.Core.Players;

namespace Uno.WinForms.Ui;

public static class UnoTheme
{
    public static readonly Color AppBackground = Color.FromArgb(255, 253, 252);
    public static readonly Color TableOuter = Color.FromArgb(21, 84, 54);
    public static readonly Color TableInner = Color.FromArgb(31, 115, 70);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceMuted = Color.FromArgb(244, 235, 219);
    public static readonly Color Ink = Color.FromArgb(30, 41, 59); // #1E293B
    public static readonly Color MutedInk = Color.FromArgb(100, 116, 139); // #64748B
    public static readonly Color Accent = Color.FromArgb(255, 77, 141); // #FF4D8D
    public static readonly Color AccentDark = Color.FromArgb(193, 35, 37);
    public static readonly Color Orange = Color.FromArgb(255, 138, 61); // #FF8A3D
    public static readonly Color Gold = Color.FromArgb(244, 196, 48);
    public static readonly Color Shadow = Color.FromArgb(25, 15, 15, 15);
    public static readonly Color Border = Color.FromArgb(236, 236, 236); // #ECECEC
    public static readonly Color PrimaryPurple = Color.FromArgb(108, 99, 255); // #6C63FF
    public static readonly Color Blue = Color.FromArgb(74, 140, 255);
    public static readonly Color SoftPink = Color.FromArgb(255, 232, 239);
    public static readonly Color SoftPeach = Color.FromArgb(255, 241, 219);
    public static readonly Color SoftBlue = Color.FromArgb(232, 245, 255);

    private static FontFamily GetFontFamily(string preferred, string fallback = "Segoe UI")
    {
        try
        {
            var family = new FontFamily(preferred);
            if (family.Name == preferred) return family;
        }
        catch { }
        return new FontFamily(fallback);
    }

    public static readonly FontFamily MainFontFamily = GetFontFamily("Segoe UI Variable Display");
    public static readonly FontFamily TextFontFamily = GetFontFamily("Segoe UI Variable Text");

    public static readonly Font TitleFont = new(MainFontFamily, 27f, FontStyle.Bold); // ~36px
    public static readonly Font HeadingFont = new(MainFontFamily, 15f, FontStyle.Bold);
    public static readonly Font SubtitleFont = new(TextFontFamily, 12f, FontStyle.Regular); // ~16px
    public static readonly Font PlayerNameFont = new(TextFontFamily, 13.5f, FontStyle.Bold); // ~18px SemiBold
    public static readonly Font ButtonFont = new(TextFontFamily, 12f, FontStyle.Bold); // ~16px
    public static readonly Font BodyFont = new(TextFontFamily, 10f, FontStyle.Regular);
    public static readonly Font LabelFont = new(TextFontFamily, 9f, FontStyle.Regular); // ~12px
    public static readonly Font SmallFont = new(TextFontFamily, 9f, FontStyle.Regular);
    public static readonly Font BadgeFont = new(TextFontFamily, 9f, FontStyle.Bold);
    public static readonly Font CardValueFont = new(MainFontFamily, 21f, FontStyle.Bold); // ~28px
    public static readonly Font CardLabelFont = new(TextFontFamily, 9f, FontStyle.Bold);

    public static Color GetCardColor(CardColor color)
    {
        return color switch
        {
            CardColor.Red => Color.FromArgb(226, 55, 58),
            CardColor.Yellow => Color.FromArgb(247, 199, 42),
            CardColor.Green => Color.FromArgb(41, 168, 88),
            CardColor.Blue => Color.FromArgb(39, 112, 221),
            _ => Color.FromArgb(41, 41, 45)
        };
    }

    public static Color GetCardInk(CardColor color)
    {
        return color == CardColor.Yellow ? Ink : Color.White;
    }

    public static string GetPlayerTypeDisplay(PlayerType playerType)
    {
        return playerType.ToString();
    }

    public static void ApplyPrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.Font = new Font(BodyFont, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    public static void ApplySecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.BackColor = Surface;
        button.ForeColor = Ink;
        button.Font = new Font(BodyFont, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    public static void ApplyInput(Control control)
    {
        control.Font = BodyFont;
        control.BackColor = Color.White;
        control.ForeColor = Ink;
    }

    public static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0) return new GraphicsPath();
        radius = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2);
        var path = new GraphicsPath();
        var diameter = radius * 2;

        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
