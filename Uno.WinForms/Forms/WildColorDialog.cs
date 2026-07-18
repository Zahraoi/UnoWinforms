using Uno.Core.Cards;
using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class WildColorDialog : Form
{
    public WildColorDialog()
    {
        Text = "Choose Wild Color";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(440, 220);
        BackColor = UnoTheme.AppBackground;

        var shell = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = UnoTheme.Surface,
            BorderColor = UnoTheme.Border,
            Padding = new Padding(24)
        };

        var title = new Label
        {
            Text = "Pick the next color",
            Dock = DockStyle.Top,
            Height = 36,
            Font = UnoTheme.HeadingFont,
            ForeColor = UnoTheme.Ink
        };

        var note = new Label
        {
            Text = "Choose the color that will control the next turn.",
            Dock = DockStyle.Top,
            Height = 34,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 16, 0, 0),
            BackColor = Color.Transparent
        };

        foreach (var color in new[] { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue })
        {
            var tile = new Button
            {
                Text = color.ToString(),
                Size = new Size(88, 88),
                BackColor = UnoTheme.GetCardColor(color),
                ForeColor = UnoTheme.GetCardInk(color),
                Font = new Font(UnoTheme.BodyFont, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 16, 0)
            };
            tile.FlatAppearance.BorderSize = 0;
            tile.Click += (_, _) =>
            {
                SelectedColor = color;
                DialogResult = DialogResult.OK;
            };
            flow.Controls.Add(tile);
        }

        shell.Controls.Add(flow);
        shell.Controls.Add(note);
        shell.Controls.Add(title);
        Controls.Add(shell);
    }

    public CardColor SelectedColor { get; private set; }
}
