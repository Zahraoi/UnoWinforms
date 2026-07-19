using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class GradientTitleControl : Control
{
    public GradientTitleControl()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        AutoSize = true;
        BackColor = Color.Transparent;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        using var g = CreateGraphics();
        var size1 = g.MeasureString("Let's Play ", UnoTheme.TitleFont);
        var size2 = g.MeasureString("UNO!", UnoTheme.TitleFont);
        return new Size((int)(size1.Width + size2.Width), (int)Math.Max(size1.Height, size2.Height));
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        
        const string first = "Let's Play ";
        const string second = "UNO!";
        
        var firstFont = UnoTheme.TitleFont;
        
        var firstSize = e.Graphics.MeasureString(first, firstFont, new PointF(0,0), StringFormat.GenericTypographic);
        var secondSize = e.Graphics.MeasureString(second, firstFont, new PointF(0,0), StringFormat.GenericTypographic);
        
        var totalWidth = firstSize.Width + secondSize.Width;
        var x = (Width - totalWidth) / 2f;
        var y = Math.Max(0, (Height - firstFont.Height) / 2f);

        using var ink = new SolidBrush(UnoTheme.Ink);
        e.Graphics.DrawString(first, firstFont, ink, x, y, StringFormat.GenericTypographic);
        
        using var gradient = new LinearGradientBrush(new RectangleF(x + firstSize.Width, y, secondSize.Width + 2, firstFont.Height), UnoTheme.Accent, UnoTheme.Orange, 0f);
        e.Graphics.DrawString(second, firstFont, gradient, x + firstSize.Width, y, StringFormat.GenericTypographic);
    }
}