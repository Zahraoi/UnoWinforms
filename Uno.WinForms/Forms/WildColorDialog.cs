using Uno.Core.Cards;
using Uno.WinForms.Controls;
using Uno.WinForms.Services;
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
        ClientSize = new Size(540, 220);
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

        var colorGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 16, 0, 0),
            BackColor = Color.Transparent,
            ColumnCount = 4,
            RowCount = 1
        };
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        var colors = new[] { CardColor.Red, CardColor.Yellow, CardColor.Green, CardColor.Blue };
        for (var index = 0; index < colors.Length; index++)
        {
            var color = colors[index];
            var tile = new Button
            {
                Text = color.ToString(),
                Size = new Size(88, 88),
                Dock = DockStyle.Fill,
                BackColor = UnoTheme.GetCardColor(color),
                ForeColor = UnoTheme.GetCardInk(color),
                Font = new Font(UnoTheme.BodyFont, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = index == colors.Length - 1 ? Padding.Empty : new Padding(0, 0, 12, 0)
            };
            tile.FlatAppearance.BorderSize = 0;
            tile.Click += (_, _) =>
            {
                SoundService.PlayButtonClick();
                SelectedColor = color;
                DialogResult = DialogResult.OK;
            };
            colorGrid.Controls.Add(tile, index, 0);
        }

        shell.Controls.Add(colorGrid);
        shell.Controls.Add(note);
        shell.Controls.Add(title);
        Controls.Add(shell);
    }

    public CardColor SelectedColor { get; private set; }
}
