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
    private readonly TableLayoutPanel _seatPanel = new();
    private readonly DoubleBufferedFlowLayoutPanel _handPanel = new();
    private readonly Label _turnBannerLabel = new();
    private readonly Label _statusLabel = new();
    private readonly Label _currentColorLabel = new();
    private readonly UnoCardControl _discardCard = new();
    private readonly UnoCardControl _drawPileCard = new();
    private readonly Label _drawCountLabel = new();
    private readonly Button _drawButton = new();
    private readonly Button _closeButton = new();
    private readonly System.Windows.Forms.Timer _computerTimer = new();
    private bool _resultShown;

    public GameForm(GameSession session, RuleEngine ruleEngine, ComputerPlayerService computerPlayerService, GamePersistenceService persistenceService)
    {
        _session = session;
        _ruleEngine = ruleEngine;
        _computerPlayerService = computerPlayerService;
        _persistenceService = persistenceService;

        Text = "Uno Match";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1260, 820);
        MinimumSize = new Size(1120, 760);
        BackColor = UnoTheme.AppBackground;
        DoubleBuffered = true;

        _computerTimer.Tick += ComputerTimerOnTick;

        BuildLayout();
        RefreshBoard();
        TriggerComputerTurnIfNeeded();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _computerTimer.Stop();
        base.OnFormClosed(e);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            RowCount = 3,
            BackColor = UnoTheme.AppBackground
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 188));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 250));

        root.Controls.Add(BuildHeaderPanel(), 0, 0);
        root.Controls.Add(BuildBoardPanel(), 0, 1);
        root.Controls.Add(BuildHandPanel(), 0, 2);
        Controls.Add(root);
    }

    private Control BuildHeaderPanel()
    {
        var host = CreateSurfacePanel();
        host.Padding = new Padding(16, 14, 16, 12);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.Transparent
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _turnBannerLabel.Dock = DockStyle.Fill;
        _turnBannerLabel.Font = UnoTheme.HeadingFont;
        _turnBannerLabel.ForeColor = UnoTheme.Ink;
        _turnBannerLabel.TextAlign = ContentAlignment.MiddleLeft;

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.Font = UnoTheme.BodyFont;
        _statusLabel.ForeColor = UnoTheme.MutedInk;
        _statusLabel.AutoEllipsis = true;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        _seatPanel.Dock = DockStyle.Fill;
        _seatPanel.ColumnCount = 2;
        _seatPanel.RowCount = 2;
        _seatPanel.BackColor = Color.Transparent;
        _seatPanel.Padding = new Padding(0, 8, 0, 0);
        _seatPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _seatPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _seatPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        _seatPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        layout.Controls.Add(_turnBannerLabel, 0, 0);
        layout.Controls.Add(_statusLabel, 0, 1);
        layout.Controls.Add(_seatPanel, 0, 2);
        host.Controls.Add(layout);
        return host;
    }

    private Control BuildBoardPanel()
    {
        var board = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UnoTheme.TableOuter,
            Padding = new Padding(18)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            BackColor = Color.Transparent
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));

        layout.Controls.Add(BuildInfoColumn(), 0, 0);
        layout.Controls.Add(BuildCenterBoard(), 1, 0);
        layout.Controls.Add(BuildActionsColumn(), 2, 0);
        board.Controls.Add(layout);
        return board;
    }

    private Control BuildInfoColumn()
    {
        var panel = CreateSurfacePanel();
        panel.Padding = new Padding(14);

        var title = new Label
        {
            Text = "Round Pulse",
            Dock = DockStyle.Top,
            Height = 32,
            Font = UnoTheme.HeadingFont,
            ForeColor = UnoTheme.Ink
        };

        _currentColorLabel.Dock = DockStyle.Top;
        _currentColorLabel.Height = 44;
        _currentColorLabel.Font = UnoTheme.HeadingFont;
        _currentColorLabel.TextAlign = ContentAlignment.MiddleCenter;

        var help = new Label
        {
            Text = "Play a glowing card. If none glow, use Draw and Pass.",
            Dock = DockStyle.Top,
            Height = 58,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk,
            Padding = new Padding(0, 12, 0, 0)
        };

        panel.Controls.Add(help);
        panel.Controls.Add(_currentColorLabel);
        panel.Controls.Add(title);
        return panel;
    }

    private Control BuildCenterBoard()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = Color.Transparent,
            Padding = new Padding(16, 10, 16, 10)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        _drawPileCard.IsFaceDown = true;
        _drawPileCard.IsPileCard = true;
        _drawPileCard.Size = new Size(120, 174);
        _drawPileCard.Anchor = AnchorStyles.None;

        _discardCard.IsPileCard = true;
        _discardCard.Size = new Size(120, 174);
        _discardCard.Anchor = AnchorStyles.None;

        layout.Controls.Add(CreatePileHost("Draw Pile", _drawPileCard, _drawCountLabel), 0, 0);
        layout.Controls.Add(CreateTopCardHost(), 1, 0);
        return layout;
    }

    private Control CreatePileHost(string titleText, Control cardControl, Label footerLabel)
    {
        var host = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            BackColor = Color.Transparent
        };
        host.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        host.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        var title = new Label
        {
            Text = titleText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = UnoTheme.BodyFont,
            ForeColor = Color.White
        };

        var cardHolder = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        cardHolder.Controls.Add(cardControl);
        cardControl.Location = new Point((Math.Max(0, cardHolder.Width - cardControl.Width)) / 2, 0);
        cardHolder.Resize += (_, _) =>
        {
            cardControl.Location = new Point((Math.Max(0, cardHolder.Width - cardControl.Width)) / 2, Math.Max(0, (cardHolder.Height - cardControl.Height) / 2));
        };

        footerLabel.Dock = DockStyle.Fill;
        footerLabel.TextAlign = ContentAlignment.MiddleCenter;
        footerLabel.Font = UnoTheme.SmallFont;
        footerLabel.ForeColor = Color.White;

        host.Controls.Add(title, 0, 0);
        host.Controls.Add(cardHolder, 0, 1);
        host.Controls.Add(footerLabel, 0, 2);
        return host;
    }

    private Control CreateTopCardHost()
    {
        var host = CreatePileHost("Top Card", _discardCard, new Label());
        return host;
    }

    private Control BuildActionsColumn()
    {
        var panel = CreateSurfacePanel();
        panel.Padding = new Padding(14);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        var title = new Label
        {
            Text = "Actions",
            Dock = DockStyle.Fill,
            Font = UnoTheme.HeadingFont,
            ForeColor = UnoTheme.Ink
        };

        var help = new Label
        {
            Text = "Use Draw and Pass only when you cannot play a highlighted card.",
            Dock = DockStyle.Fill,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk
        };

        _drawButton.Text = "Draw and Pass";
        _drawButton.Dock = DockStyle.Fill;
        UnoTheme.ApplyPrimaryButton(_drawButton);
        _drawButton.Click += (_, _) => DrawCardForHuman();

        _closeButton.Text = "Close Match";
        _closeButton.Dock = DockStyle.Fill;
        UnoTheme.ApplySecondaryButton(_closeButton);
        _closeButton.Click += (_, _) => Close();

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(help, 0, 1);
        layout.Controls.Add(_drawButton, 0, 2);
        layout.Controls.Add(_closeButton, 0, 3);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildHandPanel()
    {
        var host = CreateSurfacePanel();
        host.Padding = new Padding(16, 12, 16, 12);

        var title = new Label
        {
            Text = "Current Hand",
            Dock = DockStyle.Top,
            Height = 30,
            Font = UnoTheme.HeadingFont,
            ForeColor = UnoTheme.Ink
        };

        _handPanel.Dock = DockStyle.Fill;
        _handPanel.FlowDirection = FlowDirection.LeftToRight;
        _handPanel.WrapContents = false;
        _handPanel.AutoScroll = true;
        _handPanel.Padding = new Padding(6, 10, 6, 10);
        _handPanel.BackColor = Color.Transparent;

        host.Controls.Add(_handPanel);
        host.Controls.Add(title);
        return host;
    }

    private Panel CreateSurfacePanel()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = UnoTheme.Surface,
            Padding = new Padding(12)
        };
    }

    private void RefreshBoard()
    {
        _turnBannerLabel.Text = $"{_session.CurrentPlayer.Definition.Name}'s turn - {_session.CurrentPlayer.Definition.Type}";
        _statusLabel.Text = _session.CurrentPlayer.Definition.Type == PlayerType.Human
            ? $"{_session.LastAction} Click a glowing card to play, or draw if needed."
            : _session.LastAction;

        _currentColorLabel.Text = $"Color: {_session.CurrentColor}";
        _currentColorLabel.BackColor = UnoTheme.GetCardColor(_session.CurrentColor);
        _currentColorLabel.ForeColor = UnoTheme.GetCardInk(_session.CurrentColor);
        _drawCountLabel.Text = $"{_session.DrawPile.Count} cards left";

        _discardCard.Card = _session.CurrentCard;
        _discardCard.IsFaceDown = false;
        _discardCard.Invalidate();

        RenderSeatSummary();
        RenderHand();
    }

    private void RenderSeatSummary()
    {
        _seatPanel.SuspendLayout();
        _seatPanel.Controls.Clear();

        for (var index = 0; index < 4; index++)
        {
            var cell = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 8),
                BackColor = index < _session.Players.Count && ReferenceEquals(_session.Players[index], _session.CurrentPlayer)
                    ? Color.FromArgb(255, 247, 222)
                    : Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10, 8, 10, 8)
            };

            if (index < _session.Players.Count)
            {
                var player = _session.Players[index];
                var name = new Label
                {
                    Text = player.Definition.Name,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = UnoTheme.BodyFont,
                    ForeColor = UnoTheme.Ink
                };
                var details = new Label
                {
                    Text = $"{player.Definition.Type}  |  Cards: {player.Hand.Count}",
                    Dock = DockStyle.Top,
                    Height = 18,
                    Font = UnoTheme.SmallFont,
                    ForeColor = UnoTheme.MutedInk
                };
                var state = new Label
                {
                    Text = player.FinishRank.HasValue ? $"Rank {player.FinishRank.Value}" : ReferenceEquals(player, _session.CurrentPlayer) ? "Turn" : "Waiting",
                    Dock = DockStyle.Bottom,
                    Height = 18,
                    Font = UnoTheme.BadgeFont,
                    ForeColor = ReferenceEquals(player, _session.CurrentPlayer) ? UnoTheme.Accent : UnoTheme.MutedInk,
                    TextAlign = ContentAlignment.MiddleRight
                };

                cell.Controls.Add(state);
                cell.Controls.Add(details);
                cell.Controls.Add(name);
            }

            _seatPanel.Controls.Add(cell, index % 2, index / 2);
        }

        _seatPanel.ResumeLayout();
    }

    private void RenderHand()
    {
        _handPanel.SuspendLayout();
        _handPanel.Controls.Clear();

        var currentPlayer = _session.CurrentPlayer;
        var playableCards = _ruleEngine.GetPlayableCards(_session, currentPlayer).Select(card => card.InstanceId).ToHashSet();
        var isHumanTurn = currentPlayer.Definition.Type == PlayerType.Human && !_session.IsCompleted;

        foreach (var card in currentPlayer.Hand)
        {
            var canPlay = playableCards.Contains(card.InstanceId);
            var cardControl = new UnoCardControl
            {
                Card = card,
                IsPlayable = isHumanTurn && canPlay,
                Enabled = isHumanTurn && (!_session.Options.HighlightPlayableCards || canPlay),
                Margin = new Padding(0, 0, 8, 0),
                Size = new Size(108, 156)
            };

            cardControl.Click += CardButtonOnClick;
            _handPanel.Controls.Add(cardControl);
        }

        _drawButton.Enabled = isHumanTurn;
        if (isHumanTurn && playableCards.Count == 0)
        {
            _statusLabel.Text = "No playable card right now. Use Draw and Pass.";
        }

        _handPanel.ResumeLayout();
    }

    private async void CardButtonOnClick(object? sender, EventArgs e)
    {
        if (sender is not UnoCardControl { Card: not null } cardControl)
        {
            return;
        }

        var card = cardControl.Card;
        CardColor? chosenColor = null;
        if (card.IsWild)
        {
            using var dialog = new WildColorDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            chosenColor = dialog.SelectedColor;
        }

        var result = _ruleEngine.PlayCard(_session, card.InstanceId, chosenColor);
        await HandleMoveResultAsync(result);
    }

    private async void DrawCardForHuman()
    {
        var result = _ruleEngine.DrawCardAndPass(_session);
        await HandleMoveResultAsync(result);
    }

    private async Task HandleMoveResultAsync(MoveResult result)
    {
        if (result.Status != MoveStatus.Success)
        {
            MessageBox.Show(this, result.Message, "Move Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshBoard();
            return;
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
        if (_session.IsCompleted || _session.CurrentPlayer.Definition.Type == PlayerType.Human)
        {
            return;
        }

        _statusLabel.Text = $"{_session.CurrentPlayer.Definition.Name} is thinking...";
        _computerTimer.Interval = Math.Max(150, _session.Options.ComputerPlayerDelayMs);
        _computerTimer.Start();
    }

    private async void ComputerTimerOnTick(object? sender, EventArgs e)
    {
        _computerTimer.Stop();

        if (_session.IsCompleted || _session.CurrentPlayer.Definition.Type == PlayerType.Human)
        {
            return;
        }

        var (card, chosenColor) = _computerPlayerService.ChooseMove(_session, _ruleEngine);
        var result = card is null
            ? _ruleEngine.DrawCardAndPass(_session)
            : _ruleEngine.PlayCard(_session, card.InstanceId, chosenColor);

        await HandleMoveResultAsync(result);
    }

    private async Task ShowResultsAsync()
    {
        if (_resultShown)
        {
            return;
        }

        _resultShown = true;
        try
        {
            await _persistenceService.SaveCompletedMatchAsync(_session);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"The match finished, but saving failed: {ex.Message}", "Save Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        var persistenceMessage = _persistenceService.DatabaseAvailable
            ? "Match results were saved to SQL Server."
            : _persistenceService.StatusMessage;

        using var resultsForm = new ResultsForm(_session, persistenceMessage);
        resultsForm.ShowDialog(this);
        Close();
    }
}
