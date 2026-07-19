using Uno.WinForms.Services;

namespace Uno.WinForms.Forms;

public sealed partial class HelpForm : Form
{
    public HelpForm()
    {
        InitializeComponent();
        PopulateContent();
    }

    private void PopulateContent()
    {
        SetTabContent(newGameRichTextBox, """
            1. Choose between 2 and 4 players.
            2. Set each player's type before starting:
               - Human: controlled by a real player.
               - Computer / SmartComputer: controlled by the app.
            3. Use the plus and minus buttons to add or remove players.
            4. Press Start Game to begin dealing cards and move to the table.

            Tip:
            If you add more players than fit in the visible setup area, scroll vertically to review them before starting.
            """);

        SetTabContent(optionsRichTextBox, """
            The game options control how a round behaves.

            - Cards per player: how many cards each player starts with.
            - Scoring system: changes how points are calculated.
            - Draw 4 anytime: if enabled, Wild Draw 4 can be played more freely.
            - Zero rotates hands: if enabled, playing 0 rotates all active hands.
            - Computer delay: controls how quickly AI players act.

            Choose options before starting the match. The app saves selected settings when SQL Server is available.
            """);

        SetTabContent(playingRichTextBox, """
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
            """);

        SetTabContent(rulesRichTextBox, """
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
            """);
    }

    private static void SetTabContent(RichTextBox textBox, string content)
    {
        textBox.Text = content.Replace("\n", Environment.NewLine);
    }

    private void closeButton_Click(object? sender, EventArgs e)
    {
        SoundService.PlayButtonClick();
        Close();
    }
}
