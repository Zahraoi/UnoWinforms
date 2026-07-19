using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

#nullable disable

partial class OptionsForm
{
    private System.ComponentModel.IContainer components = null;
    private RoundedPanel shellPanel = null!;
    private Label titleLabel = null!;
    private Label descriptionLabel = null!;
    private TableLayoutPanel contentLayout = null!;
    private NumericUpDown cardsPerPlayerNumericUpDown = null!;
    private NumericUpDown computerDelayNumericUpDown = null!;
    private ComboBox scoringSystemComboBox = null!;
    private CheckBox highlightPlayableCheckBox = null!;
    private CheckBox useAnimationsCheckBox = null!;
    private CheckBox stopAfterFirstWinnerCheckBox = null!;
    private CheckBox allowDrawFourCheckBox = null!;
    private CheckBox zeroRotatesHandsCheckBox = null!;
    private FlowLayoutPanel buttonBar = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

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
        contentLayout = new TableLayoutPanel();
        cardsPerPlayerNumericUpDown = new NumericUpDown();
        computerDelayNumericUpDown = new NumericUpDown();
        scoringSystemComboBox = new ComboBox();
        highlightPlayableCheckBox = new CheckBox();
        useAnimationsCheckBox = new CheckBox();
        stopAfterFirstWinnerCheckBox = new CheckBox();
        allowDrawFourCheckBox = new CheckBox();
        zeroRotatesHandsCheckBox = new CheckBox();
        buttonBar = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        descriptionLabel = new Label();
        titleLabel = new Label();
        ((System.ComponentModel.ISupportInitialize)cardsPerPlayerNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)computerDelayNumericUpDown).BeginInit();
        shellPanel.SuspendLayout();
        contentLayout.SuspendLayout();
        buttonBar.SuspendLayout();
        SuspendLayout();

        shellPanel.Dock = DockStyle.Fill;
        shellPanel.FillColor = UnoTheme.Surface;
        shellPanel.BorderColor = UnoTheme.Border;
        shellPanel.Padding = new Padding(24);
        shellPanel.Controls.Add(contentLayout);
        shellPanel.Controls.Add(buttonBar);
        shellPanel.Controls.Add(descriptionLabel);
        shellPanel.Controls.Add(titleLabel);

        titleLabel.Dock = DockStyle.Top;
        titleLabel.Height = 36;
        titleLabel.Text = "Match Options";
        titleLabel.Font = UnoTheme.HeadingFont;
        titleLabel.ForeColor = UnoTheme.Ink;

        descriptionLabel.Dock = DockStyle.Top;
        descriptionLabel.Height = 42;
        descriptionLabel.Text = "Tune card count, scoring, automation, and optional rules for this match.";
        descriptionLabel.Font = UnoTheme.BodyFont;
        descriptionLabel.ForeColor = UnoTheme.MutedInk;

        contentLayout.Dock = DockStyle.Fill;
        contentLayout.ColumnCount = 2;
        contentLayout.Padding = new Padding(0, 10, 0, 10);
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));

        cardsPerPlayerNumericUpDown.Minimum = 1;
        cardsPerPlayerNumericUpDown.Maximum = 15;
        cardsPerPlayerNumericUpDown.Font = UnoTheme.BodyFont;
        cardsPerPlayerNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        computerDelayNumericUpDown.Minimum = 100;
        computerDelayNumericUpDown.Maximum = 5000;
        computerDelayNumericUpDown.Increment = 100;
        computerDelayNumericUpDown.Font = UnoTheme.BodyFont;
        computerDelayNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        scoringSystemComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        scoringSystemComboBox.Font = UnoTheme.BodyFont;
        scoringSystemComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        ConfigureCheckBox(highlightPlayableCheckBox, "Highlight playable cards");
        ConfigureCheckBox(useAnimationsCheckBox, "Enable animations");
        ConfigureCheckBox(stopAfterFirstWinnerCheckBox, "Stop after first winner");
        ConfigureCheckBox(allowDrawFourCheckBox, "Allow Wild Draw 4 anytime");
        ConfigureCheckBox(zeroRotatesHandsCheckBox, "0 rotates active hands");

        AddField(0, "Cards per player", cardsPerPlayerNumericUpDown);
        AddField(1, "Computer delay (ms)", computerDelayNumericUpDown);
        AddField(2, "Scoring system", scoringSystemComboBox);
        AddFullWidth(3, highlightPlayableCheckBox);
        AddFullWidth(4, useAnimationsCheckBox);
        AddFullWidth(5, stopAfterFirstWinnerCheckBox);
        AddFullWidth(6, allowDrawFourCheckBox);
        AddFullWidth(7, zeroRotatesHandsCheckBox);

        buttonBar.Dock = DockStyle.Bottom;
        buttonBar.Height = 48;
        buttonBar.FlowDirection = FlowDirection.RightToLeft;
        buttonBar.BackColor = Color.Transparent;
        buttonBar.Controls.Add(saveButton);
        buttonBar.Controls.Add(cancelButton);

        saveButton.Size = new Size(110, 36);
        saveButton.Text = "Save";
        UnoTheme.ApplyPrimaryButton(saveButton);
        saveButton.Click += saveButton_Click;

        cancelButton.Size = new Size(110, 36);
        cancelButton.Text = "Cancel";
        UnoTheme.ApplySecondaryButton(cancelButton);
        cancelButton.Click += cancelButton_Click;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UnoTheme.AppBackground;
        ClientSize = new Size(560, 470);
        Controls.Add(shellPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Game Options";
        ((System.ComponentModel.ISupportInitialize)cardsPerPlayerNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)computerDelayNumericUpDown).EndInit();
        shellPanel.ResumeLayout(false);
        contentLayout.ResumeLayout(false);
        contentLayout.PerformLayout();
        buttonBar.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void ConfigureCheckBox(CheckBox checkBox, string text)
    {
        checkBox.Text = text;
        checkBox.Font = UnoTheme.BodyFont;
        checkBox.ForeColor = UnoTheme.Ink;
        checkBox.AutoSize = true;
    }

    private void AddField(int row, string labelText, Control control)
    {
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Font = UnoTheme.BodyFont,
            ForeColor = UnoTheme.Ink
        };
        contentLayout.Controls.Add(label, 0, row);
        contentLayout.Controls.Add(control, 1, row);
    }

    private void AddFullWidth(int row, Control control)
    {
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        contentLayout.Controls.Add(control, 0, row);
        contentLayout.SetColumnSpan(control, 2);
    }
}
