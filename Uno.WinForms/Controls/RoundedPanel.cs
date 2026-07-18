using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public class RoundedPanel : Panel
{
    public RoundedPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = Color.Transparent;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius { get; set; } = 22;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FillColor { get; set; } = UnoTheme.Surface;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BorderColor { get; set; } = UnoTheme.Border;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int BorderThickness { get; set; } = 1;

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var canvasColor = Parent is RoundedPanel roundedParent
            ? roundedParent.FillColor
            : Parent?.BackColor ?? UnoTheme.AppBackground;
        e.Graphics.Clear(canvasColor);

        var shadowBounds = new Rectangle(4, 6, Width - 9, Height - 10);
        using (var shadowPath = UnoTheme.CreateRoundedPath(shadowBounds, CornerRadius))
        using (var shadowBrush = new SolidBrush(UnoTheme.Shadow))
        {
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        var bounds = new Rectangle(0, 0, Width - 5, Height - 7);
        using var path = UnoTheme.CreateRoundedPath(bounds, CornerRadius);
        using var brush = new SolidBrush(FillColor);
        using var pen = new Pen(BorderColor, BorderThickness);

        e.Graphics.FillPath(brush, path);
        e.Graphics.DrawPath(pen, path);
        base.OnPaint(e);
    }
}
