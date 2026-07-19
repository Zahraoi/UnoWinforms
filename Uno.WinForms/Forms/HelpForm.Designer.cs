using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

#nullable disable

partial class HelpForm
{
    private System.ComponentModel.IContainer components = null;
    private RoundedPanel shellPanel = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TabControl helpTabControl = null!;
    private TabPage newGameTabPage = null!;
    private TabPage optionsTabPage = null!;
    private TabPage playingTabPage = null!;
    private TabPage rulesTabPage = null!;
    private RichTextBox newGameRichTextBox = null!;
    private RichTextBox optionsRichTextBox = null!;
    private RichTextBox playingRichTextBox = null!;
    private RichTextBox rulesRichTextBox = null!;
    private ModernButton closeButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        shellPanel = new RoundedPanel();
        helpTabControl = new TabControl();
        newGameTabPage = new TabPage();
        newGameRichTextBox = new RichTextBox();
        optionsTabPage = new TabPage();
        optionsRichTextBox = new RichTextBox();
        playingTabPage = new TabPage();
        playingRichTextBox = new RichTextBox();
        rulesTabPage = new TabPage();
        rulesRichTextBox = new RichTextBox();
        closeButton = new ModernButton();
        subtitleLabel = new Label();
        titleLabel = new Label();
        shellPanel.SuspendLayout();
        helpTabControl.SuspendLayout();
        newGameTabPage.SuspendLayout();
        optionsTabPage.SuspendLayout();
        playingTabPage.SuspendLayout();
        rulesTabPage.SuspendLayout();
        SuspendLayout();

        shellPanel.Dock = DockStyle.Fill;
        shellPanel.FillColor = Color.White;
        shellPanel.BorderColor = UnoTheme.Border;
        shellPanel.Padding = new Padding(20);
        shellPanel.Controls.Add(helpTabControl);
        shellPanel.Controls.Add(closeButton);
        shellPanel.Controls.Add(subtitleLabel);
        shellPanel.Controls.Add(titleLabel);

        titleLabel.Dock = DockStyle.Top;
        titleLabel.Height = 42;
        titleLabel.Text = "How to Play UNO";
        titleLabel.Font = UnoTheme.TitleFont;
        titleLabel.ForeColor = UnoTheme.Ink;

        subtitleLabel.Dock = DockStyle.Top;
        subtitleLabel.Height = 36;
        subtitleLabel.Text = "Based on the structure of the original reference game's help pages: New Game, Game Options, Playing a Game, and UNO Rules.";
        subtitleLabel.Font = UnoTheme.SubtitleFont;
        subtitleLabel.ForeColor = UnoTheme.MutedInk;

        helpTabControl.Dock = DockStyle.Fill;
        helpTabControl.Font = UnoTheme.BodyFont;
        helpTabControl.Controls.Add(newGameTabPage);
        helpTabControl.Controls.Add(optionsTabPage);
        helpTabControl.Controls.Add(playingTabPage);
        helpTabControl.Controls.Add(rulesTabPage);

        ConfigureTabPage(newGameTabPage, "New Game", newGameRichTextBox);
        ConfigureTabPage(optionsTabPage, "Game Options", optionsRichTextBox);
        ConfigureTabPage(playingTabPage, "Playing a Game", playingRichTextBox);
        ConfigureTabPage(rulesTabPage, "UNO Rules", rulesRichTextBox);

        closeButton.Dock = DockStyle.Bottom;
        closeButton.Height = 46;
        closeButton.Margin = new Padding(0, 16, 0, 0);
        closeButton.IsGradient = true;
        closeButton.Text = "Close";
        closeButton.Click += closeButton_Click;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UnoTheme.AppBackground;
        ClientSize = new Size(920, 680);
        Controls.Add(shellPanel);
        MinimumSize = new Size(760, 540);
        StartPosition = FormStartPosition.CenterParent;
        Text = "UNO Help";
        shellPanel.ResumeLayout(false);
        helpTabControl.ResumeLayout(false);
        newGameTabPage.ResumeLayout(false);
        optionsTabPage.ResumeLayout(false);
        playingTabPage.ResumeLayout(false);
        rulesTabPage.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void ConfigureTabPage(TabPage page, string title, RichTextBox textBox)
    {
        page.Text = title;
        page.BackColor = Color.White;
        page.Padding = new Padding(12);
        textBox.Dock = DockStyle.Fill;
        textBox.ReadOnly = true;
        textBox.BorderStyle = BorderStyle.None;
        textBox.BackColor = Color.White;
        textBox.ForeColor = UnoTheme.Ink;
        textBox.Font = UnoTheme.BodyFont;
        textBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        page.Controls.Add(textBox);
    }
}
