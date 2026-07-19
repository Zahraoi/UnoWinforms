using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

#nullable disable

partial class WildColorDialog
{
    private System.ComponentModel.IContainer components = null;
    private RoundedPanel shellPanel = null!;
    private Label titleLabel = null!;
    private Label noteLabel = null!;
    private TableLayoutPanel colorGrid = null!;
    private Button redButton = null!;
    private Button yellowButton = null!;
    private Button greenButton = null!;
    private Button blueButton = null!;

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
        colorGrid = new TableLayoutPanel();
        redButton = new Button();
        yellowButton = new Button();
        greenButton = new Button();
        blueButton = new Button();
        noteLabel = new Label();
        titleLabel = new Label();
        shellPanel.SuspendLayout();
        colorGrid.SuspendLayout();
        SuspendLayout();
        // 
        // shellPanel
        // 
        shellPanel.BorderColor = UnoTheme.Border;
        shellPanel.CornerRadius = 20;
        shellPanel.Dock = DockStyle.Fill;
        shellPanel.FillColor = UnoTheme.Surface;
        shellPanel.Controls.Add(colorGrid);
        shellPanel.Controls.Add(noteLabel);
        shellPanel.Controls.Add(titleLabel);
        shellPanel.Padding = new Padding(24);
        shellPanel.Name = "shellPanel";
        shellPanel.TabIndex = 0;
        // 
        // colorGrid
        // 
        colorGrid.ColumnCount = 4;
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        colorGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        colorGrid.Controls.Add(redButton, 0, 0);
        colorGrid.Controls.Add(yellowButton, 1, 0);
        colorGrid.Controls.Add(greenButton, 2, 0);
        colorGrid.Controls.Add(blueButton, 3, 0);
        colorGrid.Dock = DockStyle.Fill;
        colorGrid.Padding = new Padding(0, 16, 0, 0);
        colorGrid.RowCount = 1;
        colorGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        colorGrid.Name = "colorGrid";
        colorGrid.TabIndex = 0;
        // 
        // redButton
        // 
        redButton.BackColor = UnoTheme.GetCardColor(Uno.Core.Cards.CardColor.Red);
        redButton.Dock = DockStyle.Fill;
        redButton.FlatAppearance.BorderSize = 0;
        redButton.FlatStyle = FlatStyle.Flat;
        redButton.Font = new Font(UnoTheme.BodyFont, FontStyle.Bold);
        redButton.ForeColor = UnoTheme.GetCardInk(Uno.Core.Cards.CardColor.Red);
        redButton.Margin = new Padding(0, 0, 12, 0);
        redButton.Name = "redButton";
        redButton.Size = new Size(105, 84);
        redButton.TabIndex = 0;
        redButton.Text = "Red";
        redButton.UseVisualStyleBackColor = false;
        redButton.Click += redButton_Click;
        // 
        // yellowButton
        // 
        yellowButton.BackColor = UnoTheme.GetCardColor(Uno.Core.Cards.CardColor.Yellow);
        yellowButton.Dock = DockStyle.Fill;
        yellowButton.FlatAppearance.BorderSize = 0;
        yellowButton.FlatStyle = FlatStyle.Flat;
        yellowButton.Font = new Font(UnoTheme.BodyFont, FontStyle.Bold);
        yellowButton.ForeColor = UnoTheme.GetCardInk(Uno.Core.Cards.CardColor.Yellow);
        yellowButton.Margin = new Padding(0, 0, 12, 0);
        yellowButton.Name = "yellowButton";
        yellowButton.Size = new Size(105, 84);
        yellowButton.TabIndex = 1;
        yellowButton.Text = "Yellow";
        yellowButton.UseVisualStyleBackColor = false;
        yellowButton.Click += yellowButton_Click;
        // 
        // greenButton
        // 
        greenButton.BackColor = UnoTheme.GetCardColor(Uno.Core.Cards.CardColor.Green);
        greenButton.Dock = DockStyle.Fill;
        greenButton.FlatAppearance.BorderSize = 0;
        greenButton.FlatStyle = FlatStyle.Flat;
        greenButton.Font = new Font(UnoTheme.BodyFont, FontStyle.Bold);
        greenButton.ForeColor = UnoTheme.GetCardInk(Uno.Core.Cards.CardColor.Green);
        greenButton.Margin = new Padding(0, 0, 12, 0);
        greenButton.Name = "greenButton";
        greenButton.Size = new Size(105, 84);
        greenButton.TabIndex = 2;
        greenButton.Text = "Green";
        greenButton.UseVisualStyleBackColor = false;
        greenButton.Click += greenButton_Click;
        // 
        // blueButton
        // 
        blueButton.BackColor = UnoTheme.GetCardColor(Uno.Core.Cards.CardColor.Blue);
        blueButton.Dock = DockStyle.Fill;
        blueButton.FlatAppearance.BorderSize = 0;
        blueButton.FlatStyle = FlatStyle.Flat;
        blueButton.Font = new Font(UnoTheme.BodyFont, FontStyle.Bold);
        blueButton.ForeColor = UnoTheme.GetCardInk(Uno.Core.Cards.CardColor.Blue);
        blueButton.Margin = Padding.Empty;
        blueButton.Name = "blueButton";
        blueButton.Size = new Size(106, 84);
        blueButton.TabIndex = 3;
        blueButton.Text = "Blue";
        blueButton.UseVisualStyleBackColor = false;
        blueButton.Click += blueButton_Click;
        // 
        // noteLabel
        // 
        noteLabel.Dock = DockStyle.Top;
        noteLabel.Font = UnoTheme.BodyFont;
        noteLabel.ForeColor = UnoTheme.MutedInk;
        noteLabel.Height = 34;
        noteLabel.Name = "noteLabel";
        noteLabel.TabIndex = 1;
        noteLabel.Text = "Choose the color that will control the next turn.";
        // 
        // titleLabel
        // 
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Font = UnoTheme.HeadingFont;
        titleLabel.ForeColor = UnoTheme.Ink;
        titleLabel.Height = 36;
        titleLabel.Name = "titleLabel";
        titleLabel.TabIndex = 2;
        titleLabel.Text = "Pick the next color";
        // 
        // WildColorDialog
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UnoTheme.AppBackground;
        ClientSize = new Size(540, 220);
        Controls.Add(shellPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "WildColorDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Choose Wild Color";
        shellPanel.ResumeLayout(false);
        colorGrid.ResumeLayout(false);
        ResumeLayout(false);
    }
}
