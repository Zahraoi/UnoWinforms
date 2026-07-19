using NAudio.Wave;

namespace Uno.WinForms.Services;

public static class SoundService
{
    private static readonly object SyncLock = new();
    private static readonly string SoundRoot = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds");
    private static WaveOutEvent? _backgroundOutput;
    private static AudioFileReader? _backgroundReader;
    private static LoopStream? _backgroundLoop;

    public static void PlayButtonClick() => PlayOneShot("button_click.wav");

    public static void PlayCardPlay() => PlayOneShot("soundc.wav");

    public static void PlayCardDraw() => PlayOneShot("card_draw.wav");

    public static void PlayError() => PlayOneShot("error.wav");

    public static void PlayWin() => PlayOneShot("win.wav");

    public static void PlayLose() => PlayOneShot("lose.wav");

    public static void StartBackgroundLoop()
    {
        lock (SyncLock)
        {
            if (_backgroundOutput is not null)
            {
                if (_backgroundOutput.PlaybackState != PlaybackState.Playing)
                {
                    _backgroundOutput.Play();
                }

                return;
            }

            var path = GetPath("backGame.wav");
            if (path is null)
            {
                return;
            }

            try
            {
                _backgroundReader = new AudioFileReader(path) { Volume = 0.18f };
                _backgroundLoop = new LoopStream(_backgroundReader);
                _backgroundOutput = new WaveOutEvent();
                _backgroundOutput.Init(_backgroundLoop);
                _backgroundOutput.Play();
            }
            catch
            {
                DisposeBackground();
            }
        }
    }

    public static void StopBackgroundLoop()
    {
        lock (SyncLock)
        {
            DisposeBackground();
        }
    }

    private static void PlayOneShot(string fileName)
    {
        var path = GetPath(fileName);
        if (path is null)
        {
            return;
        }

        _ = Task.Run(() =>
        {
            try
            {
                var reader = new AudioFileReader(path);
                var output = new WaveOutEvent();
                output.Init(reader);
                output.PlaybackStopped += (_, _) =>
                {
                    output.Dispose();
                    reader.Dispose();
                };
                output.Play();
            }
            catch
            {
            }
        });
    }

    private static string? GetPath(string fileName)
    {
        var path = Path.Combine(SoundRoot, fileName);
        return File.Exists(path) ? path : null;
    }

    private static void DisposeBackground()
    {
        try
        {
            _backgroundOutput?.Stop();
        }
        catch
        {
        }

        _backgroundOutput?.Dispose();
        _backgroundLoop?.Dispose();
        _backgroundReader?.Dispose();
        _backgroundOutput = null;
        _backgroundLoop = null;
        _backgroundReader = null;
    }

    private sealed class LoopStream : WaveStream
    {
        private readonly WaveStream _sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        public override long Length => long.MaxValue;

        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                var bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    _sourceStream.Position = 0;
                    bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}
