using System.Drawing.Drawing2D;
using Uno.Core.Cards;
using Uno.Core.Game;
using Uno.Core.Players;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class GamePlayerCardControl : RoundedPanel
{
    private readonly AvatarControl _avatar = new();
    private readonly Label _nameLabel = new();
    private readonly DoubleBufferedPanel _handViewport = new();
    private readonly DoubleBufferedFlowLayoutPanel _handPanel = new();
    private readonly System.Windows.Forms.Timer _pulseTimer = new();
    private bool _isCurrent;
    private int _arrowOffset;
    private bool _arrowMovingUp = true;
    private bool _isEmpty = true;
    private int _handStartX = 180;
    public event EventHandler<Card>? CardClicked;

    public GamePlayerCardControl()
    {
        Height = 170;
        FillColor = Color.White;
        CornerRadius = 20;
        HasShadow = true;
        Margin = new Padding(0, 0, 0, 24);
        
        _avatar.Size = new Size(70, 70);
        _avatar.BackColor = Color.White;
        
        _nameLabel.Font = UnoTheme.PlayerNameFont;
        _nameLabel.ForeColor = UnoTheme.Ink;
        _nameLabel.AutoSize = true;
        _nameLabel.BackColor = Color.White;
        _nameLabel.TextAlign = ContentAlignment.MiddleCenter;

        _handViewport.BackColor = Color.White;
        _handViewport.AutoScroll = true;
        _handViewport.BorderStyle = BorderStyle.None;
        _handViewport.Controls.Add(_handPanel);
        _handViewport.Resize += (_, _) => SyncHandViewport();
        
        _handPanel.FlowDirection = FlowDirection.LeftToRight;
        _handPanel.WrapContents = false;
        _handPanel.AutoScroll = false;
        _handPanel.BackColor = Color.White;
        _handPanel.Location = new Point(0, 0);
        _handPanel.Padding = new Padding(0);
        
        Controls.AddRange([_avatar, _nameLabel, _handViewport]);

        _pulseTimer.Interval = 50;
        _pulseTimer.Tick += (_, _) => 
        {
            if (!_isCurrent) return;
            if (_arrowMovingUp) { _arrowOffset--; if (_arrowOffset <= -8) _arrowMovingUp = false; }
            else { _arrowOffset++; if (_arrowOffset >= 0) _arrowMovingUp = true; }
            Invalidate(new Rectangle(110, 0, 60, Height));
        };
        
        Resize += (_, _) => LayoutControls();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _pulseTimer.Dispose();
        base.Dispose(disposing);
    }

    public void SetEmpty()
    {
        _isEmpty = true;
        _isCurrent = false;
        _pulseTimer.Stop();
        _avatar.Visible = false;
        _nameLabel.Visible = false;
        _handViewport.Visible = false;
        _handPanel.Visible = false;
        FillColor = UnoTheme.Surface;
        Invalidate();
    }

    public void Bind(GamePlayer player, bool isCurrent, bool isHumanTurn, HashSet<Guid> playableCards)
    {
        _isEmpty = false;
        _isCurrent = isCurrent;
        _avatar.Visible = true;
        _nameLabel.Visible = true;
        _handViewport.Visible = true;
        _handPanel.Visible = true;
        FillColor = Color.White;

        _avatar.AccentColor = player.Definition.Type == PlayerType.Human ? UnoTheme.PrimaryPurple : UnoTheme.Accent;
        _nameLabel.Text = player.Definition.Name;

        if (_isCurrent && !_pulseTimer.Enabled) _pulseTimer.Start();
        else if (!_isCurrent) { _pulseTimer.Stop(); _arrowOffset = 0; }

        LayoutControls();
        RenderHand(player, isHumanTurn, playableCards);
        Invalidate();
    }

    private void RenderHand(GamePlayer player, bool isHumanTurn, HashSet<Guid> playableCards)
    {
        _handPanel.SuspendLayout();
        
        // Unsubscribe old events
        foreach (Control c in _handPanel.Controls)
        {
            if (c is UnoCardControl uc) uc.Click -= CardButtonOnClick;
        }
        _handPanel.Controls.Clear();

        bool isHuman = player.Definition.Type == PlayerType.Human;
        var (cardSize, cardGap) = GetHandCardMetrics(player.Hand.Count);

        foreach (var card in player.Hand)
        {
            var canPlay = playableCards.Contains(card.InstanceId);
            var cardControl = new UnoCardControl
            {
                Card = card,
                IsFaceDown = !isHuman, // CPU cards face down
                IsPlayable = isHumanTurn && canPlay && isHuman,
                Enabled = isHumanTurn && (!canPlay ? false : true) || !isHumanTurn,
                Margin = new Padding(0, 0, cardGap, 0),
                Size = cardSize
            };

            // If it's human's turn but card not playable, we disable it visually (gray out) 
            // wait, we shouldn't gray out cards when it's not their turn.
            if (!isHumanTurn) cardControl.Enabled = true; // show normal colors
            else if (!canPlay) cardControl.Enabled = false;

            if (isHuman) cardControl.Click += CardButtonOnClick;
            _handPanel.Controls.Add(cardControl);
        }

        SyncHandViewport();

        _handPanel.ResumeLayout();
    }

    private void CardButtonOnClick(object? sender, EventArgs e)
    {
        if (sender is UnoCardControl { Card: not null } cardControl)
        {
            CardClicked?.Invoke(this, cardControl.Card);
        }
    }

    private void LayoutControls()
    {
        if (_isEmpty) return;

        var compact = Width < 980;
        _avatar.Size = compact ? new Size(60, 60) : new Size(70, 70);
        _handStartX = compact ? 150 : 180;
        
        _avatar.Location = new Point(compact ? 24 : 32, compact ? 26 : 32);
        _nameLabel.Location = new Point(_avatar.Left + ((_avatar.Width - _nameLabel.Width) / 2), _avatar.Bottom + 10);
        
        _handViewport.Location = new Point(_handStartX, 15);
        _handViewport.Size = new Size(Math.Max(120, Width - _handStartX - 24), Height - 30);
        SyncHandViewport();
    }

    private (Size cardSize, int gap) GetHandCardMetrics(int cardCount)
    {
        const int defaultWidth = 100;
        const int defaultHeight = 140;
        const int defaultGap = 8;
        const int minWidth = 76;
        const int minGap = 4;

        var viewportWidth = _handViewport.ClientSize.Width;
        if (viewportWidth <= 0 || cardCount <= 0)
        {
            return (new Size(defaultWidth, defaultHeight), defaultGap);
        }

        var gap = defaultGap;
        var targetWidth = (viewportWidth - (defaultGap * Math.Max(0, cardCount - 1))) / cardCount;

        if (targetWidth >= defaultWidth)
        {
            return (new Size(defaultWidth, defaultHeight), defaultGap);
        }

        targetWidth = (viewportWidth - (minGap * Math.Max(0, cardCount - 1))) / cardCount;
        if (targetWidth < minWidth)
        {
            var compactWidth = Math.Max(72, (viewportWidth - (minGap * Math.Max(0, cardCount - 1))) / Math.Max(1, cardCount));
            if (compactWidth < 72)
            {
                return (new Size(defaultWidth, defaultHeight), defaultGap);
            }

            var compactHeight = Math.Max(104, (int)Math.Round(compactWidth * 1.38));
            return (new Size(compactWidth, compactHeight), minGap);
        }

        gap = minGap;
        var width = Math.Max(minWidth, targetWidth);
        var height = Math.Max(118, (int)Math.Round(width * 1.4));
        return (new Size(width, height), gap);
    }

    private void SyncHandViewport()
    {
        if (_handViewport.Width <= 0)
        {
            return;
        }

        var contentWidth = 0;
        foreach (Control control in _handPanel.Controls)
        {
            contentWidth += control.Width + control.Margin.Horizontal;
        }

        contentWidth = Math.Max(contentWidth, _handViewport.ClientSize.Width);
        _handPanel.Size = new Size(contentWidth, Math.Max(0, _handViewport.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight));
        _handViewport.AutoScrollMinSize = new Size(contentWidth, _handPanel.Height);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        if (_isEmpty)
        {
            var iconRect = new Rectangle(32, Height / 2 - 25, 50, 50);
            using var brush = new SolidBrush(Color.FromArgb(240, 240, 245));
            e.Graphics.FillEllipse(brush, iconRect);
            
            using var textBrush = new SolidBrush(UnoTheme.MutedInk);
            e.Graphics.DrawString("Empty Slot\nWaiting for player...", UnoTheme.BodyFont, textBrush, new Point(100, Height / 2 - 16));
            return;
        }

        if (_isCurrent)
        {
            // Draw pulsing purple arrow
            using var font = new Font("Segoe UI", 24, FontStyle.Bold);
            using var brush = new SolidBrush(UnoTheme.PrimaryPurple);
            e.Graphics.DrawString("↑", font, brush, new PointF(_handStartX - 60, Height / 2 - 24 + _arrowOffset));
        }
    }
}
