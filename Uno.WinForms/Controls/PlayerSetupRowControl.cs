using Uno.Core.Players;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class PlayerSetupRowControl : Panel
{
    private readonly TextBox _nameTextBox = new();
    private readonly ComboBox _typeComboBox = new();
    private readonly Button _removeButton = new();

    public PlayerSetupRowControl(int slotNumber, string name, PlayerType playerType, bool removable)
    {
        DoubleBuffered = true;
        Height = 86;
        Width = 780;
        BackColor = Color.White;
        BorderStyle = BorderStyle.FixedSingle;
        Padding = new Padding(16, 12, 16, 12);

        var slotLabel = new Label
        {
            Text = $"Player {slotNumber}",
            Font = UnoTheme.BadgeFont,
            ForeColor = UnoTheme.MutedInk,
            AutoSize = true,
            Location = new Point(16, 10)
        };

        _nameTextBox.Text = name;
        _nameTextBox.Location = new Point(16, 34);
        _nameTextBox.Size = new Size(360, 30);
        UnoTheme.ApplyInput(_nameTextBox);

        _typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _typeComboBox.Items.AddRange(Enum.GetNames<PlayerType>());
        _typeComboBox.SelectedItem = playerType.ToString();
        _typeComboBox.Location = new Point(392, 34);
        _typeComboBox.Size = new Size(220, 30);
        UnoTheme.ApplyInput(_typeComboBox);

        _removeButton.Text = "Remove";
        _removeButton.Location = new Point(630, 32);
        _removeButton.Size = new Size(110, 34);
        _removeButton.Visible = removable;
        _removeButton.Enabled = removable;
        UnoTheme.ApplySecondaryButton(_removeButton);
        _removeButton.Click += (_, _) => RemoveRequested?.Invoke(this, EventArgs.Empty);

        Controls.Add(slotLabel);
        Controls.Add(_nameTextBox);
        Controls.Add(_typeComboBox);
        Controls.Add(_removeButton);
    }

    public event EventHandler? RemoveRequested;

    public string PlayerName => _nameTextBox.Text.Trim();

    public PlayerType PlayerType => Enum.TryParse<PlayerType>(_typeComboBox.SelectedItem?.ToString(), out var playerType)
        ? playerType
        : PlayerType.Human;

    public bool IsValid(out string message)
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            message = "Each player needs a name before the match can start.";
            return false;
        }

        message = string.Empty;
        return true;
    }
}
