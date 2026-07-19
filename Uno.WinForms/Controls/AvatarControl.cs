using System.ComponentModel;
using System.Drawing.Drawing2D;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class AvatarControl : Control
{
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color AccentColor { get; set; } = UnoTheme.PrimaryPurple;

    public AvatarControl()
    {
        Size = new Size(70, 70);
        BackColor = Color.White;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new LinearGradientBrush(ClientRectangle, AccentColor, ControlPaint.Light(AccentColor), 45f);
        e.Graphics.FillEllipse(brush, 1, 1, Width - 3, Height - 3);
        using var person = new SolidBrush(Color.White);
        var center = Width / 2;
        e.Graphics.FillEllipse(person, center - 9, 14, 18, 18);
        e.Graphics.FillEllipse(person, center - 18, 34, 36, 22);
    }
}
