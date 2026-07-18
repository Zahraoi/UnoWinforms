using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.Core.Players;

namespace Uno.WinForms.Ui;

public static class UnoTheme
{
    public static readonly Color AppBackground = Color.FromArgb(248, 241, 229);
    public static readonly Color TableOuter = Color.FromArgb(21, 84, 54);
    public static readonly Color TableInner = Color.FromArgb(31, 115, 70);
    public static readonly Color Surface = Color.FromArgb(255, 252, 246);
    public static readonly Color SurfaceMuted = Color.FromArgb(244, 235, 219);
    public static readonly Color Ink = Color.FromArgb(34, 32, 33);
    public static readonly Color MutedInk = Color.FromArgb(103, 92, 85);
    public static readonly Color Accent = Color.FromArgb(232, 61, 52);
    public static readonly Color AccentDark = Color.FromArgb(193, 35, 37);
    public static readonly Color Gold = Color.FromArgb(244, 196, 48);
    public static readonly Color Shadow = Color.FromArgb(25, 15, 15, 15);
    public static readonly Color Border = Color.FromArgb(222, 210, 193);

    public static readonly Font TitleFont = new("Segoe UI", 24, FontStyle.Bold);
    public static readonly Font HeadingFont = new("Segoe UI", 15, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 10, FontStyle.Regular);
    public static readonly Font SmallFont = new("Segoe UI", 9, FontStyle.Regular);
    public static readonly Font BadgeFont = new("Segoe UI", 9, FontStyle.Bold);
    public static readonly Font CardValueFont = new("Segoe UI", 26, FontStyle.Bold);
    public static readonly Font CardLabelFont = new("Segoe UI", 9, FontStyle.Bold);

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
