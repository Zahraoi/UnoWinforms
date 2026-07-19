using Uno.WinForms.Controls;
using Uno.WinForms.Services;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

public sealed class HelpForm : Form
{
    public HelpForm()
    {
        Text = "UNO Help";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(920, 680);
        MinimumSize = new Size(760, 540);
        BackColor = UnoTheme.AppBackground;

        BuildLayout();
    }

    private void BuildLayout()
    {
        var shell = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderColor = UnoTheme.Border,
            CornerRadius = 20,
            Padding = new Padding(20)
        };

        var title = new Label
        {
            Text = "How to Play UNO",
            Dock = DockStyle.Top,
            Height = 42,
            Font = UnoTheme.TitleFont,
            ForeColor = UnoTheme.Ink
        };

        var subtitle = new Label
        {
            Text = "Based on the structure of the original reference game's help pages: New Game, Game Options, Playing a Game, and UNO Rules.",
            Dock = DockStyle.Top,
            Height = 36,
            Font = UnoTheme.SubtitleFont,
            ForeColor = UnoTheme.MutedInk
        };

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = UnoTheme.BodyFont
        };

        tabs.TabPages.Add(CreatePage("New Game", """
            1. Choose between 2 and 4 players.
            2. Set each player's type before starting:
               - Human: controlled by a real player.
               - Computer / SmartComputer: controlled by the app.
            3. Use the plus and minus buttons to add or remove players.
            4. Press Start Game to begin dealing cards and move to the table.

            Tip:
            If you add more players than fit in the visible setup area, scroll vertically to review them before starting.
            """));

        tabs.TabPages.Add(CreatePage("Game Options", """
            The game options control how a round behaves.

            - Cards per player: how many cards each player starts with.
            - Scoring system: changes how points are calculated.
            - Draw 4 anytime: if enabled, Wild Draw 4 can be played more freely.
            - Zero rotates hands: if enabled, playing 0 rotates all active hands.
            - Computer delay: controls how quickly AI players act.

            Choose options before starting the match. The app saves selected settings when SQL Server is available.
            """));

        tabs.TabPages.Add(CreatePage("Playing a Game", """
            On your turn, play a highlighted card if one is available.

            - Human players click a playable card.
            - Computer players move automatically after a short delay.
            - The draw pile is on the left side of the game screen.
            - The top card in the discard pile determines what can be played next.

            If you cannot play:
            - use Draw and Pass, or
            - in some situations the game may automatically draw when no playable move exists.

            Extra controls:
            - End Game: leave the current game and return.
            - New Game: return to the start screen and set up another match.
            - Help: open this guide.
            - About: show app information.

            If a player has too many cards to fit in one row, use the horizontal scrollbar in that player's row to view the rest.
            """));

        tabs.TabPages.Add(CreatePage("UNO Rules", """
            This app uses the implemented UNO-style rules for this project.

            Basic play:
            - Match by color, number, or action.
            - Wild cards can be played when needed and let you choose the next color.

            Action cards:
            - Skip: next player loses their turn.
            - Reverse: turn order changes direction.
            - Draw Two: next player draws 2 cards.
            - Wild: choose a new color.
            - Wild Draw Four: choose a new color and force the next player to draw 4.

            Winning:
            - A player finishes when they play their last card.
            - Depending on selected options, the game may stop after the first winner or continue ranking players.

            Note:
            This build follows the current implemented ruleset in your project and may not include every official challenge variation from physical UNO editions.
            """));

        var closeButton = new ModernButton
        {
            Text = "Close",
            Dock = DockStyle.Bottom,
            Height = 46,
            Margin = new Padding(0, 16, 0, 0),
            IsGradient = true
        };
        closeButton.Click += (_, _) =>
        {
            SoundService.PlayButtonClick();
            Close();
        };

        shell.Controls.Add(tabs);
        shell.Controls.Add(closeButton);
        shell.Controls.Add(subtitle);
        shell.Controls.Add(title);
        Controls.Add(shell);
    }

    private static TabPage CreatePage(string title, string content)
    {
        var page = new TabPage(title)
        {
            BackColor = Color.White,
            Padding = new Padding(12)
        };

        var contentBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            ForeColor = UnoTheme.Ink,
            Font = UnoTheme.BodyFont,
            Text = content.Replace("\n", Environment.NewLine),
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        page.Controls.Add(contentBox);
        return page;
    }
}
