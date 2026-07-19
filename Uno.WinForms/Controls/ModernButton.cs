using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public class ModernButton : Control
{
    private bool _hovered;
    private bool _pressed;

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color AccentColor { get; set; } = UnoTheme.PrimaryPurple;
    
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CornerRadius { get; set; } = 20;
    
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsGradient { get; set; }

    public ModernButton()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.Selectable |
            ControlStyles.StandardClick |
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.ResizeRedraw,
            true);
        BackColor = Color.Transparent; // Transparent so parent gradient shows through corners
        ForeColor = UnoTheme.Ink;
        Font = UnoTheme.ButtonFont;
        Cursor = Cursors.Hand;
        Padding = new Padding(12, 0, 12, 0);
        
        MouseEnter += (_, _) => { _hovered = true; Invalidate(); };
        MouseLeave += (_, _) => { _hovered = false; _pressed = false; Invalidate(); };
        MouseDown += (_, _) => { _pressed = true; Invalidate(); };
        MouseUp += (_, _) => { _pressed = false; Invalidate(); };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        int offsetY = _pressed ? 2 : 0;
        var bounds = new Rectangle(1, 1 + offsetY, Width - 3, Height - 4 - offsetY);
        
        using var path = UnoTheme.CreateRoundedPath(bounds, CornerRadius);
        
        if (IsGradient)
        {
            var c1 = UnoTheme.Orange;
            var c2 = UnoTheme.Accent;
            if (_hovered && !_pressed)
            {
                c1 = ControlPaint.Light(c1, 0.1f);
                c2 = ControlPaint.Light(c2, 0.1f);
            }
            using var gradient = new LinearGradientBrush(bounds, c1, c2, 0f);
            e.Graphics.FillPath(gradient, path);
        }
        else
        {
            var fillC = BackColor == Color.Transparent ? Color.White : BackColor;
            if (_hovered && !_pressed) fillC = Color.FromArgb(250, 249, 255);
            if (_pressed) fillC = Color.FromArgb(242, 240, 250);
            
            using var fill = new SolidBrush(fillC);
            e.Graphics.FillPath(fill, path);
            
            // 1px light gray border — never black
            using var pen = new Pen(UnoTheme.Border, 1f);
            e.Graphics.DrawPath(pen, path);
        }

        TextRenderer.DrawText(e.Graphics, Text, Font, bounds, IsGradient ? Color.White : ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }

    protected override void OnClick(EventArgs e)
    {
        SoundService.PlayButtonClick();
        base.OnClick(e);
    }
}
