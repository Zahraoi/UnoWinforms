using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class ModernTextBox : TextBox
{
    public ModernTextBox()
    {
        BorderStyle = BorderStyle.None;
        BackColor = Color.White;
        ForeColor = UnoTheme.Ink;
        Font = UnoTheme.BodyFont;
        AutoSize = false;
        Height = 36;
        Padding = new Padding(4, 8, 4, 0);
    }
}
