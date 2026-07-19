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
    private readonly DoubleBufferedFlowLayoutPanel _handPanel = new();
    private readonly System.Windows.Forms.Timer _pulseTimer = new();
    private bool _isCurrent;
    private int _arrowOffset;
    private bool _arrowMovingUp = true;
    private bool _isEmpty = true;

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
        
        _handPanel.FlowDirection = FlowDirection.LeftToRight;
        _handPanel.WrapContents = false;
        _handPanel.AutoScroll = false; // We shrink cards if they overflow, or just let them overlap
        _handPanel.BackColor = Color.White;
        
        Controls.AddRange([_avatar, _nameLabel, _handPanel]);

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

        foreach (var card in player.Hand)
        {
            var canPlay = playableCards.Contains(card.InstanceId);
            var cardControl = new UnoCardControl
            {
                Card = card,
                IsFaceDown = !isHuman, // CPU cards face down
                IsPlayable = isHumanTurn && canPlay && isHuman,
                Enabled = isHumanTurn && (!canPlay ? false : true) || !isHumanTurn,
                Margin = new Padding(0, 0, 8, 0),
                Size = new Size(100, 140)
            };

            // If it's human's turn but card not playable, we disable it visually (gray out) 
            // wait, we shouldn't gray out cards when it's not their turn.
            if (!isHumanTurn) cardControl.Enabled = true; // show normal colors
            else if (!canPlay) cardControl.Enabled = false;

            if (isHuman) cardControl.Click += CardButtonOnClick;
            _handPanel.Controls.Add(cardControl);
        }

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
        
        _avatar.Location = new Point(32, 32);
        _nameLabel.Location = new Point(32 + (_avatar.Width - _nameLabel.Width) / 2, _avatar.Bottom + 12);
        
        int handX = 180;
        _handPanel.Location = new Point(handX, 15);
        _handPanel.Size = new Size(Width - handX - 24, Height - 30);
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
            e.Graphics.DrawString("↑", font, brush, new PointF(120, Height / 2 - 24 + _arrowOffset));
        }
    }
}
