using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.Core.Game;
using Uno.Core.Players;
using Uno.Core.Services;
using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class GameForm : Form
{
    private readonly GameSession _session;
    private readonly RuleEngine _ruleEngine;
    private readonly ComputerPlayerService _computerPlayerService;
    private readonly GamePersistenceService _persistenceService;
    private readonly DoubleBufferedFlowLayoutPanel _mainPanel = new();
    private readonly UnoCardControl _discardCard = new();
    private readonly UnoCardControl _drawPileCard = new();
    private readonly Label _drawCountLabel = new();
    private readonly Label _discardCountLabel = new();
    private readonly System.Windows.Forms.Timer _computerTimer = new();
    private bool _resultShown;
    private readonly List<GamePlayerCardControl> _playerCards = [];

    /// <summary>
    /// When set, calling this action returns to the start screen instead of closing a window.
    /// StartupForm sets this when embedding GameForm for single-window navigation.
    /// </summary>
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Action? NavigateBack { get; set; }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
            return cp;
        }
    }

    public GameForm(GameSession session, RuleEngine ruleEngine, ComputerPlayerService computerPlayerService, GamePersistenceService persistenceService)
    {
        _session = session;
        _ruleEngine = ruleEngine;
        _computerPlayerService = computerPlayerService;
        _persistenceService = persistenceService;

        Text = "UNO Game";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1400, 960);
        MinimumSize = new Size(1200, 800);
        BackColor = UnoTheme.AppBackground;
        DoubleBuffered = true;

        _computerTimer.Tick += ComputerTimerOnTick;

        // Suppress flicker during resize and maximize/restore
        ResizeBegin += (_, _) => SuspendLayout();
        ResizeEnd += (_, _) => ResumeLayout(true);

        BuildLayout();
        SoundService.StartBackgroundLoop();
        RefreshBoard();
        TriggerComputerTurnIfNeeded();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _computerTimer.Stop();
        SoundService.StopBackgroundLoop();
        base.OnFormClosed(e);
    }

    private void BuildLayout()
    {
        var root = new DoubleBufferedTableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Padding = new Padding(24)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 77));

        root.Controls.Add(BuildSidebar(), 0, 0);
        root.Controls.Add(BuildMainPanel(), 1, 0);

        var bg = new PastelBackgroundPanel { Dock = DockStyle.Fill };
        bg.Controls.Add(root);
        Controls.Add(bg);
    }

    private Control BuildSidebar()
    {
        var sidebar = new DoubleBufferedTableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 0, 24, 0)
        };
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 170)); // Art
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 180)); // Draw
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 180)); // Discard
        sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Spacer
        sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // End Game
        sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // New Game
        sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Footer

        // 1. Art - Natively on background
        var art = new DecorativeUnoPanel { Dock = DockStyle.Fill, Height = 160, Margin = new Padding(0, 0, 0, 16) };
        sidebar.Controls.Add(art, 0, 0);

        // 2. Draw Pile
        _drawPileCard.IsFaceDown = true;
        _drawPileCard.IsPileCard = true;
        _drawPileCard.Size = new Size(114, 160);
        _drawCountLabel.Font = new Font(UnoTheme.MainFontFamily, 44, FontStyle.Bold);
        _drawCountLabel.ForeColor = UnoTheme.Ink;
        _drawCountLabel.TextAlign = ContentAlignment.BottomRight;
        _drawCountLabel.Dock = DockStyle.Fill;
        sidebar.Controls.Add(CreatePileCard("DRAW PILE", _drawPileCard, _drawCountLabel), 0, 1);

        // 3. Discard Pile
        _discardCard.IsPileCard = true;
        _discardCard.Size = new Size(114, 160);
        _discardCountLabel.Font = new Font(UnoTheme.MainFontFamily, 44, FontStyle.Bold);
        _discardCountLabel.ForeColor = UnoTheme.Ink;
        _discardCountLabel.TextAlign = ContentAlignment.BottomRight;
        _discardCountLabel.Dock = DockStyle.Fill;
        sidebar.Controls.Add(CreatePileCard("DISCARD PILE", _discardCard, _discardCountLabel), 0, 2);

        // 4. Spacer (empty)
        sidebar.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, 3);

        // 5. End Game
        var endBtn = new ModernButton { Text = "⏻   End Game", Dock = DockStyle.Top, Height = 52, Margin = new Padding(0, 0, 0, 12), IsGradient = true };
        endBtn.Click += (_, _) => (NavigateBack ?? Close)();
        sidebar.Controls.Add(endBtn, 0, 4);

        // 6. New Game
        var newBtn = new ModernButton { Text = "+   New Game", Dock = DockStyle.Top, Height = 52, Margin = new Padding(0, 0, 0, 16), AccentColor = UnoTheme.PrimaryPurple, BackColor = Color.White, ForeColor = UnoTheme.PrimaryPurple };
        newBtn.Click += (_, _) => (NavigateBack ?? Close)();
        sidebar.Controls.Add(newBtn, 0, 5);
        
        // 7. Footer
        var footer = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Top, Height = 44, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, Margin = new Padding(0) };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        var helpBtn = new ModernButton
        {
            Text = "❔ Help",
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 8, 0),
            BackColor = Color.White,
            ForeColor = UnoTheme.Ink,
            AccentColor = UnoTheme.Border,
            Font = new Font(UnoTheme.TextFontFamily, 11f, FontStyle.Regular),
            CornerRadius = 16
        };
        helpBtn.Click += (_, _) =>
        {
            using var helpForm = new HelpForm();
            helpForm.ShowDialog(this.TopLevelControl as Form ?? this);
        };
        var aboutBtn = new ModernButton
        {
            Text = "ⓘ About",
            Dock = DockStyle.Fill,
            Margin = new Padding(8, 0, 0, 0),
            BackColor = Color.White,
            ForeColor = UnoTheme.Ink,
            AccentColor = UnoTheme.Border,
            Font = new Font(UnoTheme.TextFontFamily, 11f, FontStyle.Regular),
            CornerRadius = 16
        };
        aboutBtn.Click += (_, _) =>
        {
            using var aboutForm = new AboutForm();
            aboutForm.ShowDialog(this.TopLevelControl as Form ?? this);
        };
        footer.Controls.Add(helpBtn, 0, 0);
        footer.Controls.Add(aboutBtn, 1, 0);
        sidebar.Controls.Add(footer, 0, 6);

        return sidebar;
    }

    private Control CreatePileCard(string titleText, Control cardControl, Label countLabel)
    {
        var card = new RoundedPanel { Dock = DockStyle.Fill, FillColor = Color.White, CornerRadius = 20, Margin = new Padding(0, 0, 0, 24) };
        var layout = new DoubleBufferedTableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Color.Transparent, Padding = new Padding(14) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label { Text = titleText, Font = UnoTheme.ButtonFont, ForeColor = UnoTheme.MutedInk, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopRight };
        
        var cardHolder = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        cardHolder.Controls.Add(cardControl);
        layout.SetRowSpan(cardHolder, 2);
        
        cardControl.Location = new Point(0, 0); // Put it directly top-left in the available space

        layout.Controls.Add(cardHolder, 0, 0);
        layout.Controls.Add(title, 1, 0);
        layout.Controls.Add(countLabel, 1, 1);
        
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildMainPanel()
    {
        _mainPanel.Dock = DockStyle.Fill;
        _mainPanel.FlowDirection = FlowDirection.TopDown;
        _mainPanel.WrapContents = false;
        _mainPanel.AutoScroll = true;
        _mainPanel.BackColor = Color.Transparent;

        for (int i = 0; i < 4; i++)
        {
            var pCard = new GamePlayerCardControl();
            pCard.CardClicked += (s, card) => HandleHumanCardClick(card);
            _playerCards.Add(pCard);
            _mainPanel.Controls.Add(pCard);
        }

        _mainPanel.Resize += (_, _) =>
        {
            foreach (Control c in _mainPanel.Controls)
            {
                c.Width = _mainPanel.ClientSize.Width - 16;
            }
        };

        return _mainPanel;
    }

    private async void HandleHumanCardClick(Card card)
    {
        CardColor? chosenColor = null;
        if (card.IsWild)
        {
            using var dialog = new WildColorDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            chosenColor = dialog.SelectedColor;
        }

        var result = _ruleEngine.PlayCard(_session, card.InstanceId, chosenColor);
        await HandleMoveResultAsync(result);
    }

    private void RefreshBoard()
    {
        _drawCountLabel.Text = _session.DrawPile.Count.ToString();
        _discardCountLabel.Text = _session.DiscardPile.Count.ToString();

        _discardCard.Card = _session.CurrentCard;
        _discardCard.IsFaceDown = false;
        _discardCard.Invalidate();

        var isHumanTurn = _session.CurrentPlayer.Definition.Type == PlayerType.Human && !_session.IsCompleted;
        var playableCards = _ruleEngine.GetPlayableCards(_session, _session.CurrentPlayer).Select(c => c.InstanceId).ToHashSet();

        for (int i = 0; i < 4; i++)
        {
            if (i < _session.Players.Count)
            {
                var player = _session.Players[i];
                bool isCurrent = ReferenceEquals(player, _session.CurrentPlayer);
                _playerCards[i].Bind(player, isCurrent, isHumanTurn, playableCards);
            }
            else
            {
                _playerCards[i].SetEmpty();
            }
        }
        
        // Auto-draw if human has no playable cards
        if (isHumanTurn && playableCards.Count == 0)
        {
            // Give them a moment to see they have no moves before drawing
            System.Windows.Forms.Timer t = new();
            t.Interval = 1000;
            t.Tick += async (_, _) => 
            {
                t.Stop();
                t.Dispose();
                if (!_session.IsCompleted && _session.CurrentPlayer.Definition.Type == PlayerType.Human)
                {
                    var result = _ruleEngine.DrawCardAndPass(_session);
                    await HandleMoveResultAsync(result);
                }
            };
            t.Start();
        }
    }

    private async Task HandleMoveResultAsync(MoveResult result)
    {
        if (result.Status != MoveStatus.Success)
        {
            SoundService.PlayError();
            RefreshBoard();
            return;
        }

        if (result.PlayedCard is not null)
        {
            SoundService.PlayCardPlay();
        }
        else if (result.DrawnCards.Count > 0)
        {
            SoundService.PlayCardDraw();
        }

        RefreshBoard();

        if (result.GameCompleted)
        {
            await ShowResultsAsync();
            return;
        }

        TriggerComputerTurnIfNeeded();
    }

    private void TriggerComputerTurnIfNeeded()
    {
        if (_session.IsCompleted || _session.CurrentPlayer.Definition.Type == PlayerType.Human) return;
        _computerTimer.Interval = Math.Max(150, _session.Options.ComputerPlayerDelayMs);
        _computerTimer.Start();
    }

    private async void ComputerTimerOnTick(object? sender, EventArgs e)
    {
        _computerTimer.Stop();

        if (_session.IsCompleted || _session.CurrentPlayer.Definition.Type == PlayerType.Human) return;

        var (card, chosenColor) = _computerPlayerService.ChooseMove(_session, _ruleEngine);
        var result = card is null
            ? _ruleEngine.DrawCardAndPass(_session)
            : _ruleEngine.PlayCard(_session, card.InstanceId, chosenColor);

        await HandleMoveResultAsync(result);
    }

    private async Task ShowResultsAsync()
    {
        if (_resultShown) return;
        _resultShown = true;

        try { await _persistenceService.SaveCompletedMatchAsync(_session); }
        catch { }

        var persistenceMessage = _persistenceService.DatabaseAvailable
            ? "Match results were saved to SQL Server."
            : _persistenceService.StatusMessage;

        var winner = _session.GetOrderedResults().FirstOrDefault();
        if (winner is not null)
        {
            if (winner.Definition.Type == PlayerType.Human)
            {
                SoundService.PlayWin();
            }
            else
            {
                SoundService.PlayLose();
            }

            await Task.Delay(650);
        }

        using var resultsForm = new ResultsForm(_session, persistenceMessage);
        resultsForm.ShowDialog(this.TopLevelControl as Form ?? this);
        (NavigateBack ?? Close)();
    }
}
