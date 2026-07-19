using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class UnoCardControl : Control
{
    private bool _hovered;
    private int _animatedOffsetY = 8;
    private readonly System.Windows.Forms.Timer _animTimer = new();

    public UnoCardControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Size = new Size(100, 140);
        Cursor = Cursors.Hand;
        
        _animTimer.Interval = 16; // ~60fps
        _animTimer.Tick += (_, _) => 
        {
            int targetY = (_hovered && IsPlayable && !IsFaceDown) ? 0 : 8;
            if (_animatedOffsetY != targetY)
            {
                _animatedOffsetY += _animatedOffsetY < targetY ? 2 : -2;
                if (Math.Abs(_animatedOffsetY - targetY) < 2) _animatedOffsetY = targetY;
                Invalidate();
            }
            else
            {
                _animTimer.Stop();
            }
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _animTimer.Dispose();
        base.Dispose(disposing);
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
        _animTimer.Start();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovered = false;
        _animTimer.Start();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        var cardBounds = new Rectangle(6, _animatedOffsetY, Width - 12, Height - 16);

        // Soft shadow (12% opacity, 20 blur effect approx)
        if (!IsPileCard) // Piles have static container shadow usually, but let's give it to all
        {
            using var shadowPath1 = UnoTheme.CreateRoundedPath(new Rectangle(cardBounds.X + 2, cardBounds.Y + 4, cardBounds.Width, cardBounds.Height), 20);
            using var shadowBrush1 = new SolidBrush(Color.FromArgb(10, 0, 0, 0));
            e.Graphics.FillPath(shadowBrush1, shadowPath1);
            
            using var shadowPath2 = UnoTheme.CreateRoundedPath(new Rectangle(cardBounds.X + 4, cardBounds.Y + 8, cardBounds.Width, cardBounds.Height), 20);
            using var shadowBrush2 = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            e.Graphics.FillPath(shadowBrush2, shadowPath2);
        }

        using var cardPath = UnoTheme.CreateRoundedPath(cardBounds, 20);
        using var whiteBrush = new SolidBrush(Color.White);
        
        // Glowing border if playable and hovered
        var borderColor = IsPlayable && _hovered ? UnoTheme.Gold : Color.FromArgb(236, 236, 236);
        using var borderPen = new Pen(borderColor, IsPlayable && _hovered ? 2.5f : 1f);
        
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

        if (!Enabled && !IsPileCard && !IsFaceDown)
        {
            using var overlay = new SolidBrush(Color.FromArgb(70, 255, 255, 255)); // Lighter overlay for modern look
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
