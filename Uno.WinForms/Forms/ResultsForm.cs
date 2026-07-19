using Uno.Core.Game;
using Uno.Core.Services;
using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed partial class ResultsForm : Form
{
    public ResultsForm(GameSession session, string persistenceMessage)
    {
        InitializeComponent();
        PopulateResults(session, persistenceMessage);
    }

    private void PopulateResults(GameSession session, string persistenceMessage)
    {
        var winner = session.GetOrderedResults().First();
        winnerTitleLabel.Text = $"Winner: {winner.Definition.Name}";
        winnerSubLabel.Text = $"{winner.Definition.Type} finished rank 1 with score {ScoringService.CalculateScore(winner, session)}.";
        persistenceLabel.Text = persistenceMessage;

        rowsPanel.Controls.Clear();
        foreach (var player in session.GetOrderedResults())
        {
            rowsPanel.Controls.Add(CreateResultRow(player, session));
        }
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

    private void closeButton_Click(object? sender, EventArgs e)
    {
        SoundService.PlayButtonClick();
        Close();
    }
}
