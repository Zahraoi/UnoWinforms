using Uno.WinForms.Controls;
using Uno.WinForms.Ui;

namespace Uno.WinForms.Forms;

#nullable disable

partial class AboutForm
{
    private System.ComponentModel.IContainer components = null;
    private RoundedPanel shellPanel = null!;
    private Label titleLabel = null!;
    private Label bodyLabel = null!;
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
        bodyLabel = new Label();
        closeButton = new ModernButton();
        titleLabel = new Label();
        shellPanel.SuspendLayout();
        SuspendLayout();
        // 
        // shellPanel
        // 
        shellPanel.BorderColor = UnoTheme.Border;
        shellPanel.CornerRadius = 20;
        shellPanel.Dock = DockStyle.Fill;
        shellPanel.FillColor = Color.White;
        shellPanel.Controls.Add(bodyLabel);
        shellPanel.Controls.Add(closeButton);
        shellPanel.Controls.Add(titleLabel);
        shellPanel.Padding = new Padding(24);
        shellPanel.Name = "shellPanel";
        shellPanel.TabIndex = 0;
        // 
        // bodyLabel
        // 
        bodyLabel.Dock = DockStyle.Fill;
        bodyLabel.Font = UnoTheme.BodyFont;
        bodyLabel.ForeColor = UnoTheme.MutedInk;
        bodyLabel.Text = "A modern C# WinForms UNO project inspired by the DouglasHeriot/Uno reference game.\r\n\r\nThis version includes local game logic, SQL Server/LocalDB support for saved match data, and a refreshed desktop interface.";
        bodyLabel.Name = "bodyLabel";
        bodyLabel.TabIndex = 0;
        // 
        // closeButton
        // 
        closeButton.Dock = DockStyle.Bottom;
        closeButton.Height = 46;
        closeButton.IsGradient = true;
        closeButton.Name = "closeButton";
        closeButton.TabIndex = 1;
        closeButton.Text = "Close";
        closeButton.Click += CloseButton_Click;
        // 
        // titleLabel
        // 
        titleLabel.Dock = DockStyle.Top;
        titleLabel.Font = UnoTheme.TitleFont;
        titleLabel.ForeColor = UnoTheme.Ink;
        titleLabel.Height = 44;
        titleLabel.Name = "titleLabel";
        titleLabel.TabIndex = 2;
        titleLabel.Text = "UNO WinForms";
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UnoTheme.AppBackground;
        ClientSize = new Size(520, 320);
        Controls.Add(shellPanel);
        MinimumSize = new Size(460, 280);
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "About UNO WinForms";
        shellPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
