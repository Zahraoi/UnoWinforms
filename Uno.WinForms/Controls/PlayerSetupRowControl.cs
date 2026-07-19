using Uno.Core.Players;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class PlayerSetupRowControl : RoundedPanel
{
    private readonly ModernTextBox _nameTextBox = new();
    private readonly ModernComboBox _typeComboBox = new();

    public PlayerSetupRowControl(int slotNumber, string name, PlayerType playerType, bool removable)
    {
        Height = 120;
        FillColor = Color.White;
        BorderColor = UnoTheme.Border; // #ECECEC
        CornerRadius = 20;
        HasShadow = true;

        var accent = slotNumber switch { 1 => UnoTheme.PrimaryPurple, 2 => UnoTheme.Accent, 3 => UnoTheme.Blue, _ => UnoTheme.Orange };
        
        var avatar = new AvatarControl 
        { 
            AccentColor = accent, 
            Size = new Size(60, 60),
            BackColor = Color.White
        };
        
        _nameTextBox.Text = name;
        _nameTextBox.Font = UnoTheme.PlayerNameFont;
        _nameTextBox.BackColor = Color.White;
        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        var typeLabel = new Label 
        { 
            Text = "Type", 
            AutoSize = true, 
            Font = UnoTheme.BodyFont, 
            ForeColor = UnoTheme.MutedInk,
            BackColor = Color.White,
            Anchor = AnchorStyles.Right
        };

        _typeComboBox.Items.AddRange(Enum.GetNames<PlayerType>());
        _typeComboBox.SelectedItem = playerType.ToString();
        _typeComboBox.Size = new Size(160, 32);
        _typeComboBox.Dock = DockStyle.Fill;
        _typeComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        var table = new DoubleBufferedTableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 1,
            BackColor = Color.Transparent,
            Padding = new Padding(24, 0, 24, 0)
        };
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Define responsive columns:
        // Col 0: Avatar (fixed 72px width)
        // Col 1: Player Name (Percent 100, fills middle area dynamically)
        // Col 2: Spacer (24px)
        // Col 3: "Type" label (AutoSize)
        // Col 4: ComboBox (fixed 160px width)
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));

        // Align elements vertically in the cell center
        avatar.Anchor = AnchorStyles.Left;
        
        table.Controls.Add(avatar, 0, 0);
        table.Controls.Add(_nameTextBox, 1, 0);
        // Column 2 is spacer
        table.Controls.Add(typeLabel, 3, 0);
        table.Controls.Add(_typeComboBox, 4, 0);

        Controls.Add(table);
    }

    public string PlayerName => _nameTextBox.Text.Trim();
    public PlayerType PlayerType => Enum.TryParse<PlayerType>(_typeComboBox.SelectedItem?.ToString(), out var playerType) ? playerType : PlayerType.Human;

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
