using Uno.Core.Game;
using Uno.WinForms.Services;

namespace Uno.WinForms.Forms;

public sealed partial class OptionsForm : Form
{
    public OptionsForm(GameOptions sourceOptions)
    {
        Options = sourceOptions.Clone();
        InitializeComponent();
        LoadOptions();
    }

    public GameOptions Options { get; private set; }

    private void LoadOptions()
    {
        cardsPerPlayerNumericUpDown.Value = Options.CardsPerPlayer;
        computerDelayNumericUpDown.Value = Options.ComputerPlayerDelayMs;
        scoringSystemComboBox.DataSource = Enum.GetValues<ScoringSystem>();
        scoringSystemComboBox.SelectedItem = Options.ScoringSystem;
        highlightPlayableCheckBox.Checked = Options.HighlightPlayableCards;
        useAnimationsCheckBox.Checked = Options.UseAnimations;
        stopAfterFirstWinnerCheckBox.Checked = Options.StopAfterFirstWinner;
        allowDrawFourCheckBox.Checked = Options.AllowDrawFourAnyTime;
        zeroRotatesHandsCheckBox.Checked = Options.ZeroRotatesHands;
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        SoundService.PlayButtonClick();
        Options.CardsPerPlayer = (int)cardsPerPlayerNumericUpDown.Value;
        Options.ComputerPlayerDelayMs = (int)computerDelayNumericUpDown.Value;
        Options.ScoringSystem = (ScoringSystem)scoringSystemComboBox.SelectedItem!;
        Options.HighlightPlayableCards = highlightPlayableCheckBox.Checked;
        Options.UseAnimations = useAnimationsCheckBox.Checked;
        Options.StopAfterFirstWinner = stopAfterFirstWinnerCheckBox.Checked;
        Options.AllowDrawFourAnyTime = allowDrawFourCheckBox.Checked;
        Options.ZeroRotatesHands = zeroRotatesHandsCheckBox.Checked;
        DialogResult = DialogResult.OK;
    }

    private void cancelButton_Click(object? sender, EventArgs e)
    {
        SoundService.PlayButtonClick();
        DialogResult = DialogResult.Cancel;
    }
}
