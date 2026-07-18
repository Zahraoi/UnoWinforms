using Uno.Core.Game;
using Uno.Core.Players;
using Uno.Core.Services;
using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class StartupForm : Form
{
    private readonly RuleEngine _ruleEngine;
    private readonly ComputerPlayerService _computerPlayerService;
    private readonly GamePersistenceService _persistenceService;
    private readonly DoubleBufferedFlowLayoutPanel _playerRowsPanel = new();
    private readonly DoubleBufferedPanel _playerRowsViewport = new();
    private readonly Label _databaseStatusLabel = new();
    private readonly Label _optionsSummaryLabel = new();
    private readonly Button _startButton = new();
    private GameOptions _options = new();

    public StartupForm(RuleEngine ruleEngine, ComputerPlayerService computerPlayerService, GamePersistenceService persistenceService)
    {
        _ruleEngine = ruleEngine;
        _computerPlayerService = computerPlayerService;
        _persistenceService = persistenceService;

        Text = "Uno WinForms";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(920, 720);
        MinimumSize = new Size(900, 700);
        BackColor = UnoTheme.AppBackground;
        DoubleBuffered = true;

        BuildLayout();
        AddPlayerRow("You", PlayerType.Human, false);
        AddPlayerRow("CPU 1", PlayerType.SmartComputer, false);
        AddPlayerRow("CPU 2", PlayerType.Computer, true);
        AddPlayerRow("CPU 3", PlayerType.Computer, true);
        UpdateOptionsSummary();
    }

    private void BuildLayout()
    {
        var shell = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24)
        };

        var setupCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UnoTheme.Surface,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(28)
        };

        var title = new Label
        {
            Text = "Start a Match",
            Dock = DockStyle.Top,
            Height = 44,
            Font = UnoTheme.TitleFont,
            ForeColor = UnoTheme.Ink
        };

        var subtitle = new Label
        {
            Text = "Set the player names, choose Human or AI, adjust rules if needed, and start the game.",
            Dock = DockStyle.Top,
            Height = 42,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk
        };

        var centerPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 16, 0, 12),
            ColumnCount = 1,
            RowCount = 2
        };
        centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 98));

        _playerRowsViewport.Dock = DockStyle.Fill;
        _playerRowsViewport.AutoScroll = true;
        _playerRowsViewport.Padding = new Padding(0, 0, 6, 0);
        _playerRowsViewport.BackColor = UnoTheme.Surface;

        _playerRowsPanel.Dock = DockStyle.None;
        _playerRowsPanel.Location = new Point(0, 0);
        _playerRowsPanel.Height = 0;
        _playerRowsPanel.FlowDirection = FlowDirection.TopDown;
        _playerRowsPanel.WrapContents = false;
        _playerRowsPanel.AutoScroll = false;
        _playerRowsPanel.BackColor = UnoTheme.Surface;

        _playerRowsViewport.Controls.Add(_playerRowsPanel);

        var helperCard = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 90,
            BackColor = Color.FromArgb(247, 243, 233),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(18)
        };

        var helperLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk,
            Text = "How to play: after you press Start Match, click one of your highlighted cards on your turn. If nothing is playable, use Draw and Pass."
        };
        helperCard.Controls.Add(helperLabel);

        centerPanel.Controls.Add(_playerRowsViewport, 0, 0);
        centerPanel.Controls.Add(helperCard, 0, 1);

        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 138,
            ColumnCount = 2
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        var leftFooter = new Panel { Dock = DockStyle.Fill };
        _optionsSummaryLabel.Dock = DockStyle.Top;
        _optionsSummaryLabel.Height = 24;
        _optionsSummaryLabel.Font = UnoTheme.SmallFont;
        _optionsSummaryLabel.ForeColor = UnoTheme.MutedInk;

        _databaseStatusLabel.Dock = DockStyle.Top;
        _databaseStatusLabel.Height = 28;
        _databaseStatusLabel.Font = UnoTheme.SmallFont;
        _databaseStatusLabel.ForeColor = UnoTheme.MutedInk;
        _databaseStatusLabel.Text = _persistenceService.StatusMessage;

        var leftButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 46,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent
        };

        var addPlayerButton = new Button { Text = "Add Player", Size = new Size(116, 38) };
        UnoTheme.ApplySecondaryButton(addPlayerButton);
        addPlayerButton.Click += (_, _) => AddPlayerRow($"Player {_playerRowsPanel.Controls.Count + 1}", PlayerType.Human, true);

        var optionsButton = new Button { Text = "Options", Size = new Size(96, 38) };
        UnoTheme.ApplySecondaryButton(optionsButton);
        optionsButton.Click += (_, _) => EditOptions();

        leftButtons.Controls.Add(addPlayerButton);
        leftButtons.Controls.Add(optionsButton);
        leftFooter.Controls.Add(_databaseStatusLabel);
        leftFooter.Controls.Add(_optionsSummaryLabel);
        leftFooter.Controls.Add(leftButtons);

        var rightFooter = new Panel { Dock = DockStyle.Fill };
        _startButton.Text = "Start Match";
        _startButton.Size = new Size(180, 52);
        _startButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _startButton.Location = new Point(60, 10);
        UnoTheme.ApplyPrimaryButton(_startButton);
        _startButton.Font = new Font(UnoTheme.BodyFont, FontStyle.Bold);
        _startButton.Click += (_, _) => StartMatch();
        rightFooter.Controls.Add(_startButton);

        footer.Controls.Add(leftFooter, 0, 0);
        footer.Controls.Add(rightFooter, 1, 0);

        setupCard.Controls.Add(centerPanel);
        setupCard.Controls.Add(footer);
        setupCard.Controls.Add(subtitle);
        setupCard.Controls.Add(title);

        shell.Controls.Add(setupCard);
        Controls.Add(shell);
    }

    private void AddPlayerRow(string name, PlayerType playerType, bool removable)
    {
        if (_playerRowsPanel.Controls.Count >= 4)
        {
            return;
        }

        var row = new PlayerSetupRowControl(_playerRowsPanel.Controls.Count + 1, name, playerType, removable)
        {
            Margin = new Padding(0, 0, 0, 12)
        };
        row.RemoveRequested += (_, _) => RemovePlayerRow(row);
        _playerRowsPanel.Controls.Add(row);
        SyncPlayerRowsLayout();
        _playerRowsViewport.ScrollControlIntoView(row);
    }

    private void RemovePlayerRow(PlayerSetupRowControl row)
    {
        if (_playerRowsPanel.Controls.Count <= 2)
        {
            return;
        }

        _playerRowsPanel.Controls.Remove(row);
        row.Dispose();
        SyncPlayerRowsLayout();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        SyncPlayerRowsLayout();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        SyncPlayerRowsLayout();
    }

    private void SyncPlayerRowsLayout()
    {
        var availableWidth = Math.Max(720, _playerRowsViewport.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10);
        foreach (var row in _playerRowsPanel.Controls.OfType<PlayerSetupRowControl>())
        {
            row.Width = availableWidth;
        }

        var totalHeight = _playerRowsPanel.Controls.OfType<Control>().Sum(control => control.Height + control.Margin.Vertical);
        _playerRowsPanel.Height = totalHeight + 4;
        _playerRowsPanel.Width = availableWidth;
        _playerRowsViewport.AutoScrollMinSize = new Size(_playerRowsPanel.Width, _playerRowsPanel.Height);
    }

    private void EditOptions()
    {
        using var dialog = new OptionsForm(_options);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _options = dialog.Options;
            UpdateOptionsSummary();
            _ = _persistenceService.SaveOptionAsync("ScoringSystem", _options.ScoringSystem.ToString());
        }
    }

    private void UpdateOptionsSummary()
    {
        _optionsSummaryLabel.Text = $"Cards per player: {_options.CardsPerPlayer}   |   Scoring: {_options.ScoringSystem}   |   Draw 4 anytime: {(_options.AllowDrawFourAnyTime ? "On" : "Off")}";
    }

    private void StartMatch()
    {
        var players = new List<PlayerDefinition>();
        foreach (var control in _playerRowsPanel.Controls.OfType<PlayerSetupRowControl>())
        {
            if (!control.IsValid(out var message))
            {
                MessageBox.Show(this, message, "Invalid Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            players.Add(new PlayerDefinition(control.PlayerName, control.PlayerType));
        }

        if (players.Count < 2 || players.Count > 4)
        {
            MessageBox.Show(this, "Choose between 2 and 4 players.", "Invalid Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var session = _ruleEngine.StartNewGame(players, _options.Clone());
        using var gameForm = new GameForm(session, _ruleEngine, _computerPlayerService, _persistenceService);
        gameForm.ShowDialog(this);
        _databaseStatusLabel.Text = _persistenceService.StatusMessage;
    }
}
