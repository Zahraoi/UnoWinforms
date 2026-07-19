using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public class RoundedPanel : Panel
{
    public RoundedPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Transparent;
        BorderStyle = BorderStyle.None; // Ensure no WinForms borders
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius { get; set; } = 20;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FillColor { get; set; } = Color.White;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BorderColor { get; set; } = UnoTheme.Border;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int BorderThickness { get; set; } = 1;
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasShadow { get; set; } = true;

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        int shadowSize = HasShadow ? 6 : 0;
        var bounds = new Rectangle(shadowSize, shadowSize, Width - shadowSize * 2 - 1, Height - shadowSize * 2 - 1);
        
        using var path = UnoTheme.CreateRoundedPath(bounds, CornerRadius);

        if (HasShadow)
        {
            using var shadowPath = UnoTheme.CreateRoundedPath(new Rectangle(bounds.X + 2, bounds.Y + 4, bounds.Width, bounds.Height), CornerRadius);
            // Extremely soft, very transparent shadow to prevent dark edges
            using var shadowBrush = new SolidBrush(Color.FromArgb(8, 0, 0, 0));
            e.Graphics.FillPath(shadowBrush, shadowPath);
            
            using var shadowPath2 = UnoTheme.CreateRoundedPath(new Rectangle(bounds.X + 1, bounds.Y + 2, bounds.Width, bounds.Height), CornerRadius);
            using var shadowBrush2 = new SolidBrush(Color.FromArgb(12, 0, 0, 0));
            e.Graphics.FillPath(shadowBrush2, shadowPath2);
        }

        using var fillBrush = new SolidBrush(FillColor);
        e.Graphics.FillPath(fillBrush, path);

        if (BorderThickness > 0)
        {
            // Strict 1px #ECECEC border, NO black lines
            using var pen = new Pen(BorderColor, BorderThickness);
            e.Graphics.DrawPath(pen, path);
        }
    }
}
