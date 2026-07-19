$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot
$soundDir = Join-Path $projectRoot 'Uno.WinForms\Assets\Sounds'
New-Item -ItemType Directory -Force -Path $soundDir | Out-Null

function New-Wav {
    param(
        [string]$Path,
        [double]$Frequency,
        [int]$DurationMs
    )

    $sampleRate = 22050
    $channels = 1
    $bitsPerSample = 16
    $samples = [int]($sampleRate * $DurationMs / 1000)
    $dataSize = $samples * $channels * ($bitsPerSample / 8)

    $fileStream = [System.IO.File]::Create($Path)
    $writer = [System.IO.BinaryWriter]::new($fileStream)

    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('RIFF'))
    $writer.Write([int](36 + $dataSize))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('WAVE'))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('fmt '))
    $writer.Write([int]16)
    $writer.Write([Int16]1)
    $writer.Write([Int16]$channels)
    $writer.Write([int]$sampleRate)
    $writer.Write([int]($sampleRate * $channels * ($bitsPerSample / 8)))
    $writer.Write([Int16]($channels * ($bitsPerSample / 8)))
    $writer.Write([Int16]$bitsPerSample)
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('data'))
    $writer.Write([int]$dataSize)

    for ($i = 0; $i -lt $samples; $i++) {
        $time = $i / $sampleRate
        $envelope = [Math]::Exp(-4 * $i / $samples)
        $sample = [Math]::Sin(2 * [Math]::PI * $Frequency * $time) * 0.35 * $envelope
        $writer.Write([Int16]($sample * 32767))
    }

    $writer.Dispose()
    $fileStream.Dispose()
}

function New-CardWav {
    param(
        [string]$Path,
        [int]$DurationMs,
        [double]$NoiseAmount,
        [double]$BaseFrequency,
        [double]$ToneAmount
    )

    $sampleRate = 22050
    $channels = 1
    $bitsPerSample = 16
    $samples = [int]($sampleRate * $DurationMs / 1000)
    $dataSize = $samples * $channels * ($bitsPerSample / 8)
    $random = [System.Random]::new(42)

    $fileStream = [System.IO.File]::Create($Path)
    $writer = [System.IO.BinaryWriter]::new($fileStream)

    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('RIFF'))
    $writer.Write([int](36 + $dataSize))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('WAVE'))
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('fmt '))
    $writer.Write([int]16)
    $writer.Write([Int16]1)
    $writer.Write([Int16]$channels)
    $writer.Write([int]$sampleRate)
    $writer.Write([int]($sampleRate * $channels * ($bitsPerSample / 8)))
    $writer.Write([Int16]($channels * ($bitsPerSample / 8)))
    $writer.Write([Int16]$bitsPerSample)
    $writer.Write([System.Text.Encoding]::ASCII.GetBytes('data'))
    $writer.Write([int]$dataSize)

    for ($i = 0; $i -lt $samples; $i++) {
        $time = $i / $sampleRate
        $progress = $i / [double]$samples
        $fastEnvelope = [Math]::Exp(-10 * $progress)
        $bodyEnvelope = [Math]::Exp(-5 * $progress)
        $noise = (($random.NextDouble() * 2.0) - 1.0) * $NoiseAmount * $fastEnvelope
        $tone = ([Math]::Sin(2 * [Math]::PI * $BaseFrequency * $time) + (0.35 * [Math]::Sin(2 * [Math]::PI * ($BaseFrequency * 1.8) * $time))) * $ToneAmount * $bodyEnvelope
        $attack = [Math]::Min(1.0, $i / 40.0)
        $sample = ($noise + $tone) * $attack
        $sample = [Math]::Max(-0.95, [Math]::Min(0.95, $sample))
        $writer.Write([Int16]($sample * 32767))
    }

    $writer.Dispose()
    $fileStream.Dispose()
}

New-Wav -Path (Join-Path $soundDir 'button_click.wav') -Frequency 880 -DurationMs 70
New-CardWav -Path (Join-Path $soundDir 'card_play.wav') -DurationMs 95 -NoiseAmount 0.28 -BaseFrequency 420 -ToneAmount 0.22
New-CardWav -Path (Join-Path $soundDir 'card_draw.wav') -DurationMs 120 -NoiseAmount 0.20 -BaseFrequency 300 -ToneAmount 0.18
New-Wav -Path (Join-Path $soundDir 'error.wav') -Frequency 240 -DurationMs 150
New-Wav -Path (Join-Path $soundDir 'win.wav') -Frequency 1040 -DurationMs 180
New-Wav -Path (Join-Path $soundDir 'lose.wav') -Frequency 180 -DurationMs 220
