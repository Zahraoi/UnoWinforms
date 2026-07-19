using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class PastelBackgroundPanel : Panel
{
    public PastelBackgroundPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(UnoTheme.AppBackground);

        using var backgroundGradient = new LinearGradientBrush(ClientRectangle, Color.FromArgb(255, 241, 246), Color.FromArgb(255, 250, 253), 35f);
        e.Graphics.FillRectangle(backgroundGradient, ClientRectangle);

        FillGlow(e.Graphics, new Rectangle(-200, 100, Width / 2 + 400, Height / 2 + 400), Color.FromArgb(120, 255, 194, 211));
        FillGlow(e.Graphics, new Rectangle(Width / 4, Height / 4, Width / 2 + 300, Height / 2 + 300), Color.FromArgb(90, 255, 221, 169));
        FillGlow(e.Graphics, new Rectangle(Width / 2, Height / 2, Width / 2 + 400, Height / 2 + 400), Color.FromArgb(100, 170, 221, 255));
        
        DrawConfetti(e.Graphics);
    }

    private void DrawConfetti(Graphics g)
    {
        using var pink = new SolidBrush(Color.FromArgb(150, 255, 100, 150));
        using var green = new Pen(Color.FromArgb(150, 100, 200, 100), 3);
        using var purple = new SolidBrush(Color.FromArgb(150, 150, 100, 255));
        using var orange = new SolidBrush(Color.FromArgb(150, 255, 150, 50));
        
        // Triangle 1
        g.FillPolygon(pink, new[] { new Point(120, 150), new Point(130, 165), new Point(110, 160) });
        // Circle 1
        g.FillEllipse(purple, 250, 80, 8, 8);
        // Squiggle 1
        g.DrawBezier(green, 300, 150, 310, 130, 320, 170, 330, 150);
        
        // Triangle 2
        g.FillPolygon(purple, new[] { new Point(200, Height - 200), new Point(215, Height - 210), new Point(205, Height - 190) });
        // Circle 2
        g.FillEllipse(orange, 80, Height - 250, 10, 10);
        // Squiggle 2
        g.DrawBezier(green, 50, Height - 300, 60, Height - 280, 40, Height - 260, 50, Height - 240);

        // More scattered shapes across the left side
        g.FillPolygon(pink, new[] { new Point(350, Height - 350), new Point(365, Height - 365), new Point(355, Height - 340) });
        g.FillEllipse(purple, 150, Height - 100, 8, 8);
        
        g.DrawString("✦", new Font("Segoe UI", 16), purple, 180, 220);
        g.DrawString("+", new Font("Segoe UI", 18), pink, 80, 350);
        g.DrawString("✦", new Font("Segoe UI", 14), orange, 380, 280);
    }

    private static void FillGlow(Graphics graphics, Rectangle bounds, Color color)
    {
        using var path = new GraphicsPath();
        path.AddEllipse(bounds);
        using var brush = new PathGradientBrush(path) { CenterColor = color, SurroundColors = [Color.FromArgb(0, color)] };
        graphics.FillPath(brush, path);
    }
}
