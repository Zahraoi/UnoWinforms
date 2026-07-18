using System.Drawing.Drawing2D;
using Uno.Core.Game;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Controls;

public sealed class PlayerSeatControl : Control
{
    private GamePlayer? _player;
    private bool _isCurrent;

    public PlayerSeatControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Size = new Size(220, 86);
    }

    public void Bind(GamePlayer player, bool isCurrent)
    {
        _player = player;
        _isCurrent = isCurrent;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        var fill = _isCurrent ? Color.FromArgb(255, 245, 224) : UnoTheme.Surface;
        var border = _isCurrent ? UnoTheme.Gold : UnoTheme.Border;

        using (var shadowPath = UnoTheme.CreateRoundedPath(new Rectangle(2, 4, Width - 6, Height - 8), 16))
        using (var shadowBrush = new SolidBrush(UnoTheme.Shadow))
        {
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        using var path = UnoTheme.CreateRoundedPath(new Rectangle(0, 0, Width - 4, Height - 6), 16);
        using var fillBrush = new SolidBrush(fill);
        using var borderPen = new Pen(border, _isCurrent ? 2f : 1f);
        e.Graphics.FillPath(fillBrush, path);
        e.Graphics.DrawPath(borderPen, path);

        if (_player is null)
        {
            return;
        }

        using var headingBrush = new SolidBrush(UnoTheme.Ink);
        using var bodyBrush = new SolidBrush(UnoTheme.MutedInk);
        e.Graphics.DrawString(_player.Definition.Name, UnoTheme.BodyFont, headingBrush, new RectangleF(12, 8, Width - 24, 18));
        e.Graphics.DrawString(UnoTheme.GetPlayerTypeDisplay(_player.Definition.Type), UnoTheme.SmallFont, bodyBrush, new RectangleF(12, 28, Width - 24, 14));

        var cardBadgeBounds = new Rectangle(12, Height - 30, 72, 18);
        using (var badgePath = UnoTheme.CreateRoundedPath(cardBadgeBounds, 12))
        using (var badgeBrush = new SolidBrush(UnoTheme.TableInner))
        using (var badgeInk = new SolidBrush(Color.White))
        {
            e.Graphics.FillPath(badgeBrush, badgePath);
            e.Graphics.DrawString($"Cards {_player.Hand.Count}", UnoTheme.SmallFont, badgeInk, cardBadgeBounds, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        var rankText = _player.FinishRank.HasValue ? $"Rank {_player.FinishRank.Value}" : (_isCurrent ? "Turn" : "Waiting");
        using var rankBrush = new SolidBrush(_isCurrent ? UnoTheme.Accent : UnoTheme.MutedInk);
        e.Graphics.DrawString(rankText, UnoTheme.BadgeFont, rankBrush, new RectangleF(Width - 78, Height - 30, 66, 18), new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
    }
}
