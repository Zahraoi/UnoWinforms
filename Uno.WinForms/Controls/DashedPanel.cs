using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class DashedPanel : Panel
{
    public DashedPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        Padding = new Padding(12);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Parent?.BackColor ?? UnoTheme.Surface);
        using var path = UnoTheme.CreateRoundedPath(new Rectangle(1, 1, Width - 3, Height - 3), 12);
        using var pen = new Pen(Color.FromArgb(205, 194, 255), 1.5f) { DashStyle = DashStyle.Dash };
        e.Graphics.DrawPath(pen, path);
        base.OnPaint(e);
    }
}
