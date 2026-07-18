$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$projectRoot = Split-Path -Parent $PSScriptRoot
$assetRoot = Join-Path $projectRoot 'Uno.WinForms\Assets\Cards'
New-Item -ItemType Directory -Force -Path $assetRoot | Out-Null

$width = 240
$height = 360

$colors = @{
    red = [System.Drawing.Color]::FromArgb(231, 59, 65)
    yellow = [System.Drawing.Color]::FromArgb(246, 197, 48)
    green = [System.Drawing.Color]::FromArgb(39, 170, 89)
    blue = [System.Drawing.Color]::FromArgb(46, 108, 226)
    wild = [System.Drawing.Color]::FromArgb(31, 31, 34)
}

$faceMap = @(
    @{ Name = '0'; Text = '0'; Subtitle = '' },
    @{ Name = '1'; Text = '1'; Subtitle = '' },
    @{ Name = '2'; Text = '2'; Subtitle = '' },
    @{ Name = '3'; Text = '3'; Subtitle = '' },
    @{ Name = '4'; Text = '4'; Subtitle = '' },
    @{ Name = '5'; Text = '5'; Subtitle = '' },
    @{ Name = '6'; Text = '6'; Subtitle = '' },
    @{ Name = '7'; Text = '7'; Subtitle = '' },
    @{ Name = '8'; Text = '8'; Subtitle = '' },
    @{ Name = '9'; Text = '9'; Subtitle = '' },
    @{ Name = 'skip'; Text = 'S'; Subtitle = 'SKIP' },
    @{ Name = 'reverse'; Text = 'R'; Subtitle = 'REVERSE' },
    @{ Name = 'drawtwo'; Text = '+2'; Subtitle = 'DRAW TWO' }
)

function New-RoundedRectPath {
    param(
        [int]$X,
        [int]$Y,
        [int]$W,
        [int]$H,
        [int]$R
    )

    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $R * 2
    $path.AddArc($X, $Y, $d, $d, 180, 90)
    $path.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $path.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $path.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-Card {
    param(
        [string]$Path,
        [System.Drawing.Color]$Accent,
        [string]$MainText,
        [string]$Subtitle,
        [switch]$Wild,
        [switch]$Back
    )

    $bitmap = New-Object System.Drawing.Bitmap $width, $height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)

    $shadowPath = New-RoundedRectPath 18 20 198 314 26
    $shadowBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(40, 18, 18, 18))
    $graphics.FillPath($shadowBrush, $shadowPath)

    $cardPath = New-RoundedRectPath 10 10 198 314 26
    $whiteBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::White)
    $whitePen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(225, 225, 225), 3)
    $graphics.FillPath($whiteBrush, $cardPath)
    $graphics.DrawPath($whitePen, $cardPath)

    $innerPath = New-RoundedRectPath 22 22 174 290 22

    if ($Back) {
        $backTop = [System.Drawing.Color]::FromArgb(244, 88, 69)
        $backBottom = [System.Drawing.Color]::FromArgb(193, 35, 37)
        $backGradient = New-Object System.Drawing.Drawing2D.LinearGradientBrush ([System.Drawing.Rectangle]::FromLTRB(22, 22, 196, 312)), $backTop, $backBottom, 90
        $graphics.FillPath($backGradient, $innerPath)

        $stripeBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(50, 255, 255, 255))
        for ($i = 0; $i -lt 6; $i++) {
            $graphics.FillEllipse($stripeBrush, 32 + ($i * 10), 32 + ($i * 20), 154 - ($i * 10), 16)
        }

        $graphics.TranslateTransform(110, 166)
        $graphics.RotateTransform(-18)
        $goldBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(247, 199, 42))
        $graphics.FillEllipse($goldBrush, -54, -30, 108, 60)
        $unoFont = [System.Drawing.Font]::new('Segoe UI', 26.0, ([System.Drawing.FontStyle]::Bold -bor [System.Drawing.FontStyle]::Italic))
        $stringFormat = New-Object System.Drawing.StringFormat
        $stringFormat.Alignment = [System.Drawing.StringAlignment]::Center
        $stringFormat.LineAlignment = [System.Drawing.StringAlignment]::Center
        $graphics.DrawString('UNO', $unoFont, [System.Drawing.Brushes]::White, ([System.Drawing.RectangleF]::new(-52, -24, 104, 48)), $stringFormat)
        $graphics.ResetTransform()
    }
    else {
        if ($Wild) {
            $segments = @(
                @{ Brush = New-Object System.Drawing.SolidBrush $colors.red; Start = 180 },
                @{ Brush = New-Object System.Drawing.SolidBrush $colors.blue; Start = 270 },
                @{ Brush = New-Object System.Drawing.SolidBrush $colors.green; Start = 0 },
                @{ Brush = New-Object System.Drawing.SolidBrush $colors.yellow; Start = 90 }
            )

            foreach ($segment in $segments) {
                $graphics.FillPie($segment.Brush, 22, 22, 174, 290, $segment.Start, 90)
            }
        }
        else {
            $accentTop = [System.Drawing.Color]::FromArgb([Math]::Min(255, $Accent.R + 18), [Math]::Min(255, $Accent.G + 18), [Math]::Min(255, $Accent.B + 18))
            $accentGradient = New-Object System.Drawing.Drawing2D.LinearGradientBrush ([System.Drawing.Rectangle]::FromLTRB(22, 22, 196, 312)), $accentTop, $Accent, 90
            $graphics.FillPath($accentGradient, $innerPath)
        }

        $graphics.TranslateTransform(110, 166)
        $graphics.RotateTransform(-20)
        $centerBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(236, 255, 255, 255))
        $graphics.FillEllipse($centerBrush, -60, -88, 120, 176)
        $graphics.ResetTransform()

        $textBrush = if ($Accent -eq $colors.yellow) { [System.Drawing.Brushes]::Black } else { [System.Drawing.Brushes]::White }
        $cornerBrush = if ($Accent -eq $colors.yellow) { [System.Drawing.Brushes]::Black } else { [System.Drawing.Brushes]::White }
        $mainFontSize = if ($MainText.Length -gt 1) { 58 } else { 74 }
        if ($Wild) { $mainFontSize = 44 }

        $cornerFont = [System.Drawing.Font]::new('Segoe UI', 22.0, [System.Drawing.FontStyle]::Bold)
        $mainFont = [System.Drawing.Font]::new('Segoe UI', [float]$mainFontSize, [System.Drawing.FontStyle]::Bold)
        $subFont = [System.Drawing.Font]::new('Segoe UI', 12.0, [System.Drawing.FontStyle]::Bold)
        $centerFormat = New-Object System.Drawing.StringFormat
        $centerFormat.Alignment = [System.Drawing.StringAlignment]::Center
        $centerFormat.LineAlignment = [System.Drawing.StringAlignment]::Center

        $graphics.DrawString($MainText, $cornerFont, $cornerBrush, 34, 26)
        $graphics.DrawString($MainText, $cornerFont, $cornerBrush, ([System.Drawing.RectangleF]::new(154, 270, 28, 28)), $centerFormat)
        $graphics.DrawString($MainText, $mainFont, $textBrush, ([System.Drawing.RectangleF]::new(26, 118, 168, 88)), $centerFormat)

        if ($Subtitle) {
            $graphics.DrawString($Subtitle, $subFont, $cornerBrush, ([System.Drawing.RectangleF]::new(28, 266, 162, 18)), $centerFormat)
        }
    }

    $bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)

    $graphics.Dispose()
    $bitmap.Dispose()
}

foreach ($colorKey in @('red', 'yellow', 'green', 'blue')) {
    foreach ($face in $faceMap) {
        $fileName = "card_{0}_{1}.png" -f $colorKey, $face.Name
        $filePath = Join-Path $assetRoot $fileName
        Draw-Card -Path $filePath -Accent $colors[$colorKey] -MainText $face.Text -Subtitle $face.Subtitle
    }
}

Draw-Card -Path (Join-Path $assetRoot 'card_wild.png') -Accent $colors.wild -MainText 'WILD' -Subtitle 'WILD' -Wild
Draw-Card -Path (Join-Path $assetRoot 'card_wild_drawfour.png') -Accent $colors.wild -MainText '+4' -Subtitle 'WILD DRAW 4' -Wild
Draw-Card -Path (Join-Path $assetRoot 'card_back.png') -Accent $colors.red -MainText '' -Subtitle '' -Back
