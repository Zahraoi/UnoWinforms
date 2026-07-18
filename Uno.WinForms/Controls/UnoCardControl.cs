using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class UnoCardControl : Control
{
    private bool _hovered;

    public UnoCardControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Size = new Size(108, 156);
        Cursor = Cursors.Hand;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Card? Card { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsPlayable { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsFaceDown { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsPileCard { get; set; }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var offsetY = IsPlayable && _hovered ? 0 : 8;
        var cardBounds = new Rectangle(6, offsetY, Width - 12, Height - 16);

        using (var shadowPath = UnoTheme.CreateRoundedPath(new Rectangle(cardBounds.X + 4, cardBounds.Y + 6, cardBounds.Width, cardBounds.Height), 18))
        using (var shadowBrush = new SolidBrush(UnoTheme.Shadow))
        {
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        using var cardPath = UnoTheme.CreateRoundedPath(cardBounds, 18);
        using var whiteBrush = new SolidBrush(Color.White);
        using var borderPen = new Pen(IsPlayable ? UnoTheme.Gold : Color.FromArgb(225, 225, 225), IsPlayable ? 3f : 1.5f);
        e.Graphics.FillPath(whiteBrush, cardPath);
        e.Graphics.DrawPath(borderPen, cardPath);

        e.Graphics.SetClip(cardPath);

        if (IsFaceDown)
        {
            DrawCardImage(e.Graphics, cardBounds, CardImageProvider.GetBackImage());
            e.Graphics.ResetClip();
            return;
        }

        if (Card is null)
        {
            e.Graphics.ResetClip();
            return;
        }

        DrawCardImage(e.Graphics, cardBounds, CardImageProvider.GetCardImage(Card));

        if (!Enabled)
        {
            using var overlay = new SolidBrush(Color.FromArgb(90, 25, 25, 25));
            e.Graphics.FillPath(overlay, cardPath);
        }

        e.Graphics.ResetClip();
    }

    private static void DrawCardImage(Graphics graphics, Rectangle bounds, Image? image)
    {
        if (image is not null)
        {
            graphics.DrawImage(image, bounds);
            return;
        }

        var inner = Rectangle.Inflate(bounds, -8, -8);
        using var fallbackBrush = new SolidBrush(Color.FromArgb(242, 242, 242));
        graphics.FillRectangle(fallbackBrush, inner);
    }
}
