using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

#nullable disable

partial class ResultsForm
{
    private System.ComponentModel.IContainer components = null;
    private RoundedPanel shellPanel = null!;
    private RoundedPanel bannerPanel = null!;
    private Label winnerTitleLabel = null!;
    private Label winnerSubLabel = null!;
    private DoubleBufferedFlowLayoutPanel rowsPanel = null!;
    private FlowLayoutPanel footerPanel = null!;
    private Button closeButton = null!;
    private Label persistenceLabel = null!;

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
        rowsPanel = new DoubleBufferedFlowLayoutPanel();
        footerPanel = new FlowLayoutPanel();
        closeButton = new Button();
        persistenceLabel = new Label();
        bannerPanel = new RoundedPanel();
        winnerSubLabel = new Label();
        winnerTitleLabel = new Label();
        shellPanel.SuspendLayout();
        footerPanel.SuspendLayout();
        bannerPanel.SuspendLayout();
        SuspendLayout();

        shellPanel.Dock = DockStyle.Fill;
        shellPanel.FillColor = UnoTheme.Surface;
        shellPanel.BorderColor = UnoTheme.Border;
        shellPanel.Padding = new Padding(24);
        shellPanel.Controls.Add(rowsPanel);
        shellPanel.Controls.Add(footerPanel);
        shellPanel.Controls.Add(bannerPanel);

        bannerPanel.Dock = DockStyle.Top;
        bannerPanel.Height = 116;
        bannerPanel.FillColor = UnoTheme.Accent;
        bannerPanel.BorderColor = UnoTheme.AccentDark;
        bannerPanel.Padding = new Padding(24);
        bannerPanel.Controls.Add(winnerSubLabel);
        bannerPanel.Controls.Add(winnerTitleLabel);

        winnerTitleLabel.Dock = DockStyle.Top;
        winnerTitleLabel.Height = 48;
        winnerTitleLabel.Font = UnoTheme.TitleFont;
        winnerTitleLabel.ForeColor = Color.White;

        winnerSubLabel.Dock = DockStyle.Top;
        winnerSubLabel.Height = 26;
        winnerSubLabel.Font = UnoTheme.BodyFont;
        winnerSubLabel.ForeColor = Color.FromArgb(255, 238, 238);

        rowsPanel.Dock = DockStyle.Fill;
        rowsPanel.FlowDirection = FlowDirection.TopDown;
        rowsPanel.WrapContents = false;
        rowsPanel.AutoScroll = true;
        rowsPanel.BackColor = Color.Transparent;
        rowsPanel.Padding = new Padding(2, 18, 2, 12);

        footerPanel.Dock = DockStyle.Bottom;
        footerPanel.Height = 48;
        footerPanel.FlowDirection = FlowDirection.RightToLeft;
        footerPanel.BackColor = Color.Transparent;
        footerPanel.Controls.Add(closeButton);
        footerPanel.Controls.Add(persistenceLabel);

        closeButton.Size = new Size(110, 36);
        closeButton.Text = "Close";
        UnoTheme.ApplyPrimaryButton(closeButton);
        closeButton.Click += closeButton_Click;

        persistenceLabel.AutoSize = true;
        persistenceLabel.Font = UnoTheme.SmallFont;
        persistenceLabel.ForeColor = UnoTheme.MutedInk;
        persistenceLabel.Padding = new Padding(0, 10, 14, 0);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UnoTheme.AppBackground;
        ClientSize = new Size(860, 640);
        Controls.Add(shellPanel);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Match Results";
        shellPanel.ResumeLayout(false);
        footerPanel.ResumeLayout(false);
        footerPanel.PerformLayout();
        bannerPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
