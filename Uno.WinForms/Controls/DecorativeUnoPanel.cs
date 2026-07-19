using System.Drawing.Drawing2D;

namespace Uno.WinForms.Controls;

public sealed class DecorativeUnoPanel : Panel
{
    private readonly Image? _illustration;

    public DecorativeUnoPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.Transparent;

        var imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Cards", "uno_background.png");
        if (File.Exists(imagePath))
        {
            _illustration = Image.FromFile(imagePath);
        }
        else
        {
            var fallback = Path.Combine(AppContext.BaseDirectory, "Assets", "Cards", "card_back.png");
            if (File.Exists(fallback)) _illustration = Image.FromFile(fallback);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        if (_illustration is not null)
        {
            var maxWidth = Width;
            var maxHeight = Height;
            if (maxWidth > 0 && maxHeight > 0)
            {
                // Add equal padding (5%) on all sides so the image never touches edges
                var pad = Math.Min(maxWidth, maxHeight) * 0.05f;
                var availableWidth = maxWidth - pad * 2;
                var availableHeight = maxHeight - pad * 2;

                // "Contain" behavior — use Math.Min so the entire image always fits
                // without any cropping, occupying ~90-95% of the available area
                var targetWidth = availableWidth * 0.95f;
                var targetHeight = availableHeight * 0.95f;

                var scaleX = targetWidth / _illustration.Width;
                var scaleY = targetHeight / _illustration.Height;
                var scale = Math.Min(scaleX, scaleY); // Min = contain — full image always visible

                var imageWidth = (int)(_illustration.Width * scale);
                var imageHeight = (int)(_illustration.Height * scale);
                
                // Center perfectly within the panel
                var target = new Rectangle(
                    (maxWidth - imageWidth) / 2,
                    (maxHeight - imageHeight) / 2,
                    imageWidth,
                    imageHeight
                );
                
                e.Graphics.DrawImage(_illustration, target);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _illustration?.Dispose();
        base.Dispose(disposing);
    }
}
