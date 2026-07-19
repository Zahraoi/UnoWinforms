using Uno.WinForms.Services;

namespace Uno.WinForms.Forms;

public sealed partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, EventArgs e)
    {
        SoundService.PlayButtonClick();
        Close();
    }
}
