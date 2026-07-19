using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class ModernComboBox : ComboBox
{
    public ModernComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 30;
        FlatStyle = FlatStyle.Flat;
        Font = UnoTheme.BodyFont;
        BackColor = Color.White;
        ForeColor = UnoTheme.Ink;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0)
        {
            using var brush = new SolidBrush(ForeColor);
            e.Graphics.DrawString(GetItemText(Items[e.Index]), Font, brush, e.Bounds.Left + 10, e.Bounds.Top + 7);
        }
        e.DrawFocusRectangle();
    }

    private const int WM_PAINT = 0x000F;
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg == WM_PAINT)
        {
            // Erase the default black flat border by drawing our own light border over it
            using var g = Graphics.FromHwnd(Handle);
            using var pen = new Pen(UnoTheme.Border, 1);
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
