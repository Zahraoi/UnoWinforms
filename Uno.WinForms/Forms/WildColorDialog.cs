using Uno.Core.Cards;
using Uno.WinForms.Services;

namespace Uno.WinForms.Forms;

public sealed partial class WildColorDialog : Form
{
    public WildColorDialog()
    {
        InitializeComponent();
    }

    public CardColor SelectedColor { get; private set; }

    private void redButton_Click(object? sender, EventArgs e) => SelectColor(CardColor.Red);
    private void yellowButton_Click(object? sender, EventArgs e) => SelectColor(CardColor.Yellow);
    private void greenButton_Click(object? sender, EventArgs e) => SelectColor(CardColor.Green);
    private void blueButton_Click(object? sender, EventArgs e) => SelectColor(CardColor.Blue);

    private void SelectColor(CardColor color)
    {
        SoundService.PlayButtonClick();
        SelectedColor = color;
        DialogResult = DialogResult.OK;
    }
}
