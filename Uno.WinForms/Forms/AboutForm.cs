using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class AboutForm : Form
{
    public AboutForm()
    {
        Text = "About UNO WinForms";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 320);
        MinimumSize = new Size(460, 280);
        BackColor = UnoTheme.AppBackground;

        var shell = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderColor = UnoTheme.Border,
            CornerRadius = 20,
            Padding = new Padding(24)
        };

        var title = new Label
        {
            Text = "UNO WinForms",
            Dock = DockStyle.Top,
            Height = 44,
            Font = UnoTheme.TitleFont,
            ForeColor = UnoTheme.Ink
        };

        var body = new Label
        {
            Dock = DockStyle.Fill,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk,
            Text = "A modern C# WinForms UNO project inspired by the DouglasHeriot/Uno reference game.\n\nThis version includes local game logic, SQL Server/LocalDB support for saved match data, and a refreshed desktop interface.",
            AutoSize = false
        };

        var closeButton = new ModernButton
        {
            Text = "Close",
            Dock = DockStyle.Bottom,
            Height = 46,
            IsGradient = true
        };
        closeButton.Click += (_, _) =>
        {
            SoundService.PlayButtonClick();
            Close();
        };

        shell.Controls.Add(body);
        shell.Controls.Add(closeButton);
        shell.Controls.Add(title);
        Controls.Add(shell);
    }
}
