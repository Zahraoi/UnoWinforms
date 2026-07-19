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
    private readonly Label _databaseStatusLabel = new();
    private readonly Label _optionsSummaryLabel = new();
    private readonly Label _numPlayersValueLabel = new();
    private readonly DoubleBufferedTableLayoutPanel _shellLayout = new();
    private readonly DecorativeUnoPanel _artPanel = new();
    private readonly DoubleBufferedTableLayoutPanel _rightLayout = new();
    private readonly DoubleBufferedPanel _playerRowsViewport = new();
    private GameOptions _options = new();

    // Single-window navigation: one root content panel swaps children
    private readonly DoubleBufferedPanel _contentPanel = new();
    private Control? _startBackground; // cached reference to startup background panel
    private GameForm? _activeGame;     // currently embedded game screen

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - eliminates all resize/maximize flicker
            return cp;
        }
    }

    public StartupForm(RuleEngine ruleEngine, ComputerPlayerService computerPlayerService, GamePersistenceService persistenceService)
    {
        _ruleEngine = ruleEngine;
        _computerPlayerService = computerPlayerService;
        _persistenceService = persistenceService;
        Text = "New UNO Game";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1500, 900);
        MinimumSize = new Size(1100, 800);
        BackColor = UnoTheme.AppBackground;
        DoubleBuffered = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        
        // Suppress flicker during resize and maximize/restore
        ResizeBegin += (_, _) => SuspendLayout();
        ResizeEnd += (_, _) => ResumeLayout(true);
        
        // Root content panel fills the whole form — children swap in/out here
        _contentPanel.Dock = DockStyle.Fill;
        _contentPanel.BackColor = Color.Transparent;
        Controls.Add(_contentPanel);
        
        BuildLayout();
        
        AddPlayerRow("Player 1", PlayerType.Human, false);
        AddPlayerRow("Player 2", PlayerType.SmartComputer, false);
        
        UpdateOptionsSummary();
        UpdateNumPlayersValue();
    }

    private void BuildLayout()
    {
        var shell = _shellLayout;
        shell.Dock = DockStyle.Fill;
        shell.ColumnCount = 2;
        shell.RowCount = 1;
        shell.Padding = new Padding(40, 32, 40, 40);
        shell.BackColor = Color.Transparent;
        shell.AutoScroll = false;
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // 30% left panel
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // 70% right panel

        var art = _artPanel;
        art.Dock = DockStyle.Fill;
        art.Margin = new Padding(0, 0, 32, 0); 
        shell.Controls.Add(art, 0, 0);

        var right = _rightLayout;
        right.Dock = DockStyle.Fill;
        right.ColumnCount = 1;
        right.RowCount = 3;
        right.BackColor = Color.Transparent;
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var header = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 24) };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        
        var titlePanel = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0) };
        titlePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        titlePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        
        var title = new GradientTitleControl { Dock = DockStyle.Top, Margin = new Padding(0) };
        var subtitle = new Label { Text = "Add players and start the fun 🎉", Dock = DockStyle.Top, AutoSize = true, Font = UnoTheme.SubtitleFont, ForeColor = UnoTheme.MutedInk, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(0, 4, 0, 0), BackColor = Color.Transparent };
        titlePanel.Controls.Add(title, 0, 0);
        titlePanel.Controls.Add(subtitle, 0, 1);
        header.Controls.Add(titlePanel, 0, 0);

        var countCard = new RoundedPanel { Size = new Size(240, 100), Anchor = AnchorStyles.Top | AnchorStyles.Right, FillColor = Color.White, CornerRadius = 18, Margin = new Padding(0) };
        var lblNum = new Label { Text = "Number of Players", AutoSize = true, Font = UnoTheme.BadgeFont, ForeColor = UnoTheme.Ink, Location = new Point(54, 16), BackColor = Color.White };
        
        var minusBtn = CreateTinyButton("−", 28, 42, UnoTheme.SoftPink, UnoTheme.Accent);
        minusBtn.Click += (_, _) => 
        { 
            if (_playerRowsPanel.Controls.Count > 2) 
                RemovePlayerRow((PlayerSetupRowControl)_playerRowsPanel.Controls[_playerRowsPanel.Controls.Count - 1]); 
        };
        
        _numPlayersValueLabel.Text = "2";
        _numPlayersValueLabel.TextAlign = ContentAlignment.MiddleCenter;
        _numPlayersValueLabel.Font = UnoTheme.CardValueFont;
        _numPlayersValueLabel.ForeColor = UnoTheme.Ink;
        _numPlayersValueLabel.BackColor = Color.White;
        _numPlayersValueLabel.Location = new Point(84, 38);
        _numPlayersValueLabel.Size = new Size(70, 50);
        
        var plusBtn = CreateTinyButton("+", 164, 42, UnoTheme.SoftPink, UnoTheme.Accent);
        plusBtn.Click += (_, _) => 
        { 
            if (_playerRowsPanel.Controls.Count < 4) 
                AddPlayerRow($"Player {_playerRowsPanel.Controls.Count + 1}", PlayerType.Human, true); 
        };
        
        countCard.Controls.AddRange([lblNum, minusBtn, _numPlayersValueLabel, plusBtn]);
        header.Controls.Add(countCard, 1, 0);

        var playersCard = new RoundedPanel { Dock = DockStyle.Fill, FillColor = Color.White, CornerRadius = 24, Padding = new Padding(32) };
        var playersCardLayout = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.Transparent };
        playersCardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        playersCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        
        var playersTitle = new Label { Text = "👥 PLAYERS", AutoSize = true, Font = UnoTheme.HeadingFont, ForeColor = UnoTheme.PrimaryPurple, Margin = new Padding(0, 0, 0, 16), BackColor = Color.White };
        
        _playerRowsViewport.Dock = DockStyle.Fill;
        _playerRowsViewport.AutoScroll = true;
        _playerRowsViewport.BackColor = Color.Transparent;
        _playerRowsViewport.Padding = new Padding(0, 0, 8, 0);

        _playerRowsPanel.Dock = DockStyle.None;
        _playerRowsPanel.FlowDirection = FlowDirection.TopDown;
        _playerRowsPanel.WrapContents = false;
        _playerRowsPanel.AutoScroll = false;
        _playerRowsPanel.BackColor = Color.Transparent;
        _playerRowsPanel.Location = new Point(0, 0);
        
        playersCardLayout.Controls.Add(playersTitle, 0, 0);
        _playerRowsViewport.Controls.Add(_playerRowsPanel);
        playersCardLayout.Controls.Add(_playerRowsViewport, 0, 1);
        playersCard.Controls.Add(playersCardLayout);

        var footerLayout = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Bottom, Height = 60, ColumnCount = 3, RowCount = 1, Margin = new Padding(0, 24, 0, 0) };
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        
        var optionsButton = new ModernButton { Text = "⚙ Game Options", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 0), AccentColor = UnoTheme.PrimaryPurple };
        optionsButton.Click += (_, _) => EditOptions();
        
        var aboutButton = new ModernButton { Text = "ⓘ About", Dock = DockStyle.Fill, Margin = new Padding(12, 0, 12, 0), AccentColor = UnoTheme.Blue };
        aboutButton.Click += (_, _) =>
        {
            using var aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        };
        
        var startButton = new ModernButton { Text = "▶ Start Game", Dock = DockStyle.Fill, Margin = new Padding(12, 0, 0, 0), IsGradient = true };
        startButton.Click += (_, _) => StartMatch();
        
        footerLayout.Controls.Add(optionsButton, 0, 0);
        footerLayout.Controls.Add(aboutButton, 1, 0);
        footerLayout.Controls.Add(startButton, 2, 0);

        right.Controls.Add(header, 0, 0);
        right.Controls.Add(playersCard, 0, 1);
        right.Controls.Add(footerLayout, 0, 2);
        shell.Controls.Add(right, 1, 0);
        
        var background = new PastelBackgroundPanel { Dock = DockStyle.Fill };
        background.Controls.Add(shell);
        _startBackground = background; // cache for re-showing later
        _contentPanel.Controls.Add(background);
    }

    private static ModernButton CreateTinyButton(string text, int x, int y, Color backColor, Color foreColor) => 
        new() { Text = text, Location = new Point(x, y), Size = new Size(46, 46), BackColor = backColor, ForeColor = foreColor, AccentColor = foreColor, CornerRadius = 14, Font = new Font("Segoe UI", 18) };

    private void AddPlayerRow(string name, PlayerType playerType, bool removable)
    {
        if (_playerRowsPanel.Controls.Count >= 4) return;
        
        SuspendLayout();
        _playerRowsPanel.SuspendLayout();
        _shellLayout.SuspendLayout();
        
        var row = new PlayerSetupRowControl(_playerRowsPanel.Controls.Count + 1, name, playerType, removable) { Margin = new Padding(0, 0, 0, 16) };
        _playerRowsPanel.Controls.Add(row);
        SyncPlayerRowsLayout();
        _playerRowsViewport.ScrollControlIntoView(row);
        _playerRowsViewport.VerticalScroll.Value = _playerRowsViewport.VerticalScroll.Maximum;
        
        _shellLayout.ResumeLayout(true);
        _playerRowsPanel.ResumeLayout(true);
        ResumeLayout(true);
    }

    private void RemovePlayerRow(PlayerSetupRowControl row)
    {
        if (_playerRowsPanel.Controls.Count <= 2) return;
        
        SuspendLayout();
        _playerRowsPanel.SuspendLayout();
        _shellLayout.SuspendLayout();
        
        _playerRowsPanel.Controls.Remove(row);
        row.Dispose();
        SyncPlayerRowsLayout();
        
        _shellLayout.ResumeLayout(true);
        _playerRowsPanel.ResumeLayout(true);
        ResumeLayout(true);
    }

    protected override void OnShown(EventArgs e) { base.OnShown(e); SyncPlayerRowsLayout(); }
    protected override void OnResize(EventArgs e) { base.OnResize(e); SyncPlayerRowsLayout(); }

    private void SyncPlayerRowsLayout()
    {
        if (_playerRowsViewport.ClientSize.Width <= 0) return;
        var availableWidth = Math.Max(0, _playerRowsViewport.ClientSize.Width - 8);
        foreach (var row in _playerRowsPanel.Controls.OfType<PlayerSetupRowControl>()) 
        {
            row.Width = availableWidth;
        }
        var totalHeight = _playerRowsPanel.Controls.OfType<Control>().Sum(control => control.Height + control.Margin.Vertical);
        _playerRowsPanel.Size = new Size(availableWidth, totalHeight + 4);
        _playerRowsViewport.AutoScrollMinSize = new Size(0, totalHeight + 8);
        UpdateNumPlayersValue();
    }
    
    private void UpdateNumPlayersValue()
    {
        if (_numPlayersValueLabel != null)
        {
            _numPlayersValueLabel.Text = _playerRowsPanel.Controls.Count.ToString();
        }
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

    private void UpdateOptionsSummary() => _optionsSummaryLabel.Text = $"Cards per player: {_options.CardsPerPlayer} | Scoring: {_options.ScoringSystem} | Draw 4 anytime: {(_options.AllowDrawFourAnyTime ? "On" : "Off")}";

    private void StartMatch()
    {
        var players = new List<PlayerDefinition>();
        foreach (var control in _playerRowsPanel.Controls.OfType<PlayerSetupRowControl>())
        {
            if (!control.IsValid(out var message))
            {
                SoundService.PlayError();
                MessageBox.Show(this, message, "Invalid Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            players.Add(new PlayerDefinition(control.PlayerName, control.PlayerType));
        }
        if (players.Count < 2 || players.Count > 4)
        {
            SoundService.PlayError();
            MessageBox.Show(this, "Choose between 2 and 4 players.", "Invalid Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var session = _ruleEngine.StartNewGame(players, _options.Clone());
        NavigateToGame(session);
    }

    // ── Single-window navigation ─────────────────────────────────────────────

    private void NavigateToGame(GameSession session)
    {
        SuspendLayout();
        _contentPanel.SuspendLayout();

        // Remove startup screen (keeps it alive in memory for instant back-navigation)
        if (_startBackground != null)
            _contentPanel.Controls.Remove(_startBackground);

        // Build & embed the game screen as a non-top-level child
        _activeGame?.Dispose();
        _activeGame = new GameForm(session, _ruleEngine, _computerPlayerService, _persistenceService)
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };
        // Wire back-navigation: when the game signals "done", return to start screen
        _activeGame.NavigateBack = NavigateToStart;

        _contentPanel.Controls.Add(_activeGame);
        _activeGame.Show();

        _contentPanel.ResumeLayout(true);
        ResumeLayout(true);
    }

    private void NavigateToStart()
    {
        SuspendLayout();
        _contentPanel.SuspendLayout();

        // Remove game screen
        if (_activeGame != null)
        {
            _contentPanel.Controls.Remove(_activeGame);
            _activeGame.Dispose();
            _activeGame = null;
        }

        // Restore cached startup screen
        if (_startBackground != null && !_contentPanel.Controls.Contains(_startBackground))
            _contentPanel.Controls.Add(_startBackground);

        _databaseStatusLabel.Text = _persistenceService.StatusMessage;

        _contentPanel.ResumeLayout(true);
        ResumeLayout(true);

        SyncPlayerRowsLayout();
    }
}
