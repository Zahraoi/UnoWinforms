using Uno.Core.Game;
using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class OptionsForm : Form
{
    private readonly NumericUpDown _cardsPerPlayer = new();
    private readonly NumericUpDown _computerDelay = new();
    private readonly CheckBox _highlightPlayable = new();
    private readonly CheckBox _useAnimations = new();
    private readonly CheckBox _stopAfterFirstWinner = new();
    private readonly CheckBox _allowDrawFour = new();
    private readonly CheckBox _zeroRotatesHands = new();
    private readonly ComboBox _scoringSystem = new();

    public OptionsForm(GameOptions sourceOptions)
    {
        Options = sourceOptions.Clone();

        Text = "Game Options";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(560, 470);
        BackColor = UnoTheme.AppBackground;

        BuildLayout();
    }

    public GameOptions Options { get; private set; }

    private void BuildLayout()
    {
        var shell = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(18),
            Padding = new Padding(24),
            FillColor = UnoTheme.Surface,
            BorderColor = UnoTheme.Border
        };

        var title = new Label
        {
            Text = "Match Options",
            Dock = DockStyle.Top,
            Height = 36,
            Font = UnoTheme.HeadingFont,
            ForeColor = UnoTheme.Ink
        };

        var description = new Label
        {
            Text = "Tune card count, scoring, automation, and optional rules for this match.",
            Dock = DockStyle.Top,
            Height = 42,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(0, 10, 0, 10)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

        _cardsPerPlayer.Minimum = 1;
        _cardsPerPlayer.Maximum = 15;
        _cardsPerPlayer.Value = Options.CardsPerPlayer;
        _cardsPerPlayer.Font = UnoTheme.BodyFont;

        _computerDelay.Minimum = 100;
        _computerDelay.Maximum = 5000;
        _computerDelay.Increment = 100;
        _computerDelay.Value = Options.ComputerPlayerDelayMs;
        _computerDelay.Font = UnoTheme.BodyFont;

        _highlightPlayable.Text = "Highlight playable cards";
        _highlightPlayable.Checked = Options.HighlightPlayableCards;
        _useAnimations.Text = "Enable animations";
        _useAnimations.Checked = Options.UseAnimations;
        _stopAfterFirstWinner.Text = "Stop after first winner";
        _stopAfterFirstWinner.Checked = Options.StopAfterFirstWinner;
        _allowDrawFour.Text = "Allow Wild Draw 4 anytime";
        _allowDrawFour.Checked = Options.AllowDrawFourAnyTime;
        _zeroRotatesHands.Text = "0 rotates active hands";
        _zeroRotatesHands.Checked = Options.ZeroRotatesHands;

        foreach (var checkBox in new[] { _highlightPlayable, _useAnimations, _stopAfterFirstWinner, _allowDrawFour, _zeroRotatesHands })
        {
            checkBox.Font = UnoTheme.BodyFont;
            checkBox.ForeColor = UnoTheme.Ink;
            checkBox.AutoSize = true;
        }

        _scoringSystem.DropDownStyle = ComboBoxStyle.DropDownList;
        _scoringSystem.DataSource = Enum.GetValues<ScoringSystem>();
        _scoringSystem.SelectedItem = Options.ScoringSystem;
        _scoringSystem.Font = UnoTheme.BodyFont;

        AddField(layout, 0, "Cards per player", _cardsPerPlayer);
        AddField(layout, 1, "Computer delay (ms)", _computerDelay);
        AddField(layout, 2, "Scoring system", _scoringSystem);
        AddFullWidth(layout, 3, _highlightPlayable);
        AddFullWidth(layout, 4, _useAnimations);
        AddFullWidth(layout, 5, _stopAfterFirstWinner);
        AddFullWidth(layout, 6, _allowDrawFour);
        AddFullWidth(layout, 7, _zeroRotatesHands);

        var buttonBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent
        };

        var saveButton = new Button { Text = "Save", Size = new Size(110, 36) };
        UnoTheme.ApplyPrimaryButton(saveButton);
        saveButton.Click += (_, _) =>
        {
            SoundService.PlayButtonClick();
            SaveAndClose();
        };

        var cancelButton = new Button { Text = "Cancel", Size = new Size(110, 36) };
        UnoTheme.ApplySecondaryButton(cancelButton);
        cancelButton.Click += (_, _) =>
        {
            SoundService.PlayButtonClick();
            DialogResult = DialogResult.Cancel;
        };

        buttonBar.Controls.Add(saveButton);
        buttonBar.Controls.Add(cancelButton);

        shell.Controls.Add(layout);
        shell.Controls.Add(buttonBar);
        shell.Controls.Add(description);
        shell.Controls.Add(title);
        Controls.Add(shell);
    }

    private static void AddField(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.Ink
        };
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private static void AddFullWidth(TableLayoutPanel layout, int row, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.Controls.Add(control, 0, row);
        layout.SetColumnSpan(control, 2);
    }

    private void SaveAndClose()
    {
        Options.CardsPerPlayer = (int)_cardsPerPlayer.Value;
        Options.ComputerPlayerDelayMs = (int)_computerDelay.Value;
        Options.ScoringSystem = (ScoringSystem)_scoringSystem.SelectedItem!;
        Options.HighlightPlayableCards = _highlightPlayable.Checked;
        Options.UseAnimations = _useAnimations.Checked;
        Options.StopAfterFirstWinner = _stopAfterFirstWinner.Checked;
        Options.AllowDrawFourAnyTime = _allowDrawFour.Checked;
        Options.ZeroRotatesHands = _zeroRotatesHands.Checked;

        DialogResult = DialogResult.OK;
    }
}
