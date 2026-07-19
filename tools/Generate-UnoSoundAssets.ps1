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

function New-CardStyleWav {
    param(
        [string]$Path,
        [int]$DurationMs,
        [double]$Impact,
        [double]$Rustle,
        [double]$BodyFrequency,
        [double]$BodyAmount
    )

    $sampleRate = 22050
    $channels = 1
    $bitsPerSample = 16
    $samples = [int]($sampleRate * $DurationMs / 1000)
    $dataSize = $samples * $channels * ($bitsPerSample / 8)
    $random = [System.Random]::new(84)

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

        $impactEnv = [Math]::Exp(-38 * $progress)
        $rustleEnv = [Math]::Exp(-11 * $progress)
        $bodyEnv = [Math]::Exp(-18 * $progress)
        $attack = [Math]::Min(1.0, $i / 16.0)

        $impactNoise = (($random.NextDouble() * 2.0) - 1.0) * $Impact * $impactEnv
        $rustleNoise = (($random.NextDouble() * 2.0) - 1.0) * $Rustle * $rustleEnv
        $body = ([Math]::Sin(2 * [Math]::PI * $BodyFrequency * $time) + (0.22 * [Math]::Sin(2 * [Math]::PI * ($BodyFrequency * 2.1) * $time))) * $BodyAmount * $bodyEnv

        $sample = ($impactNoise + $rustleNoise + $body) * $attack
        $sample = [Math]::Max(-0.8, [Math]::Min(0.8, $sample))
        $writer.Write([Int16]($sample * 32767))
    }

    $writer.Dispose()
    $fileStream.Dispose()
}

function New-SoftLoopWav {
    param(
        [string]$Path,
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
        $padA = [Math]::Sin(2 * [Math]::PI * 196 * $time)
        $padB = [Math]::Sin(2 * [Math]::PI * 246.94 * $time)
        $padC = [Math]::Sin(2 * [Math]::PI * 293.66 * $time)
        $sparkle = [Math]::Sin(2 * [Math]::PI * 659.25 * $time)
        $lfoA = 0.5 + (0.5 * [Math]::Sin(2 * [Math]::PI * 0.08 * $time))
        $lfoB = 0.5 + (0.5 * [Math]::Sin(2 * [Math]::PI * 0.06 * $time + 1.4))
        $lfoC = 0.5 + (0.5 * [Math]::Sin(2 * [Math]::PI * 0.05 * $time + 2.2))
        $pulse = [Math]::Max(0.0, [Math]::Sin(2 * [Math]::PI * 1.2 * $time))

        $sample = (0.018 * $padA * $lfoA) + (0.014 * $padB * $lfoB) + (0.011 * $padC * $lfoC) + (0.004 * $sparkle * $pulse)
        $sample = [Math]::Max(-0.16, [Math]::Min(0.16, $sample))
        $writer.Write([Int16]($sample * 32767))
    }

    $writer.Dispose()
    $fileStream.Dispose()
}

New-Wav -Path (Join-Path $soundDir 'button_click.wav') -Frequency 880 -DurationMs 70
New-CardStyleWav -Path (Join-Path $soundDir 'card_play.wav') -DurationMs 82 -Impact 0.40 -Rustle 0.12 -BodyFrequency 175 -BodyAmount 0.08
New-CardStyleWav -Path (Join-Path $soundDir 'card_draw.wav') -DurationMs 108 -Impact 0.20 -Rustle 0.18 -BodyFrequency 145 -BodyAmount 0.06
New-Wav -Path (Join-Path $soundDir 'error.wav') -Frequency 240 -DurationMs 150
New-Wav -Path (Join-Path $soundDir 'win.wav') -Frequency 1040 -DurationMs 180
New-Wav -Path (Join-Path $soundDir 'lose.wav') -Frequency 180 -DurationMs 220
New-SoftLoopWav -Path (Join-Path $soundDir 'background_loop.wav') -DurationMs 10000
