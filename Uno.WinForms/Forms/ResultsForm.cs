using Uno.Core.Game;
using Uno.Core.Services;
using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class ResultsForm : Form
{
    public ResultsForm(GameSession session, string persistenceMessage)
    {
        Text = "Match Results";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(860, 640);
        BackColor = UnoTheme.AppBackground;

        BuildLayout(session, persistenceMessage);
    }

    private void BuildLayout(GameSession session, string persistenceMessage)
    {
        var shell = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = UnoTheme.Surface,
            BorderColor = UnoTheme.Border,
            Padding = new Padding(24)
        };

        var winner = session.GetOrderedResults().First();
        var banner = new RoundedPanel
        {
            Dock = DockStyle.Top,
            Height = 116,
            FillColor = UnoTheme.Accent,
            BorderColor = UnoTheme.AccentDark,
            Padding = new Padding(24)
        };

        var winnerTitle = new Label
        {
            Text = $"Winner: {winner.Definition.Name}",
            ForeColor = Color.White,
            Font = UnoTheme.TitleFont,
            Dock = DockStyle.Top,
            Height = 48
        };

        var winnerSub = new Label
        {
            Text = $"{winner.Definition.Type} finished rank 1 with score {ScoringService.CalculateScore(winner, session)}.",
            ForeColor = Color.FromArgb(255, 238, 238),
            Font = UnoTheme.BodyFont,
            Dock = DockStyle.Top,
            Height = 26
        };

        banner.Controls.Add(winnerSub);
        banner.Controls.Add(winnerTitle);

        var rowsPanel = new DoubleBufferedFlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(2, 18, 2, 12)
        };

        foreach (var player in session.GetOrderedResults())
        {
            rowsPanel.Controls.Add(CreateResultRow(player, session));
        }

        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent
        };

        var closeButton = new Button { Text = "Close", Size = new Size(110, 36) };
        UnoTheme.ApplyPrimaryButton(closeButton);
        closeButton.Click += (_, _) => Close();

        var persistenceLabel = new Label
        {
            Text = persistenceMessage,
            AutoSize = true,
            Font = UnoTheme.SmallFont,
            ForeColor = UnoTheme.MutedInk,
            Padding = new Padding(0, 10, 14, 0)
        };

        footer.Controls.Add(closeButton);
        footer.Controls.Add(persistenceLabel);

        shell.Controls.Add(rowsPanel);
        shell.Controls.Add(footer);
        shell.Controls.Add(banner);
        Controls.Add(shell);
    }

    private static Control CreateResultRow(GamePlayer player, GameSession session)
    {
        var row = new RoundedPanel
        {
            Width = 780,
            Height = 94,
            Margin = new Padding(0, 0, 0, 14),
            FillColor = player.FinishRank == 1 ? Color.FromArgb(255, 247, 228) : UnoTheme.SurfaceMuted,
            BorderColor = player.FinishRank == 1 ? UnoTheme.Gold : UnoTheme.Border,
            Padding = new Padding(18, 14, 18, 14)
        };

        var left = new Label
        {
            Text = $"#{player.FinishRank}  {player.Definition.Name}\n{player.Definition.Type}",
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.Ink,
            Size = new Size(220, 54),
            Location = new Point(18, 18)
        };

        var metrics = new Label
        {
            Text = $"Score {ScoringService.CalculateScore(player, session)}   |   Played {player.CardsPlayed}   |   Drawn {player.CardsDrawn}   |   Turns {player.TurnCount}",
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.MutedInk,
            Size = new Size(480, 24),
            Location = new Point(240, 34)
        };

        row.Controls.Add(metrics);
        row.Controls.Add(left);
        return row;
    }
}
