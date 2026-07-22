using NAudio.Wave;

namespace Uno.WinForms.Services;

public static class SoundService
{
    private static readonly object SyncLock = new();
    private static readonly string SoundRoot = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds");
    private static readonly float BackgroundBaseVolume = 0.58f;
    private static readonly float BackgroundDuckVolume = 0.18f;
    private static WaveOutEvent? _backgroundOutput;
    private static AudioFileReader? _backgroundReader;
    private static LoopStream? _backgroundLoop;
    private static int _duckToken;

    public static void PlayButtonClick() => PlayOneShot("button_click.wav", 0.75f);

    public static void PlayCardPlay() => PlayOneShot("soundc.wav", 0.95f);

    public static void PlayCardDraw() => PlayOneShot("card_draw.wav", 0.88f);

    public static void PlayError() => PlayOneShot("error.wav", 0.90f);

    public static void PlayWin()
    {
        DuckBackgroundTemporarily(900);
        PlayOneShot("win.wav", 1.00f);
    }

    public static void PlayLose()
    {
        DuckBackgroundTemporarily(900);
        PlayOneShot("lose.wav", 0.98f);
    }

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
                _backgroundReader = new AudioFileReader(path) { Volume = BackgroundBaseVolume };
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

    private static void PlayOneShot(string fileName, float volume)
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
                var reader = new AudioFileReader(path) { Volume = volume };
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

    private static void DuckBackgroundTemporarily(int milliseconds)
    {
        AudioFileReader? reader;
        int token;

        lock (SyncLock)
        {
            if (_backgroundReader is null)
            {
                return;
            }

            _duckToken++;
            token = _duckToken;
            _backgroundReader.Volume = BackgroundDuckVolume;
            reader = _backgroundReader;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(milliseconds);
                lock (SyncLock)
                {
                    if (_backgroundReader is not null && ReferenceEquals(_backgroundReader, reader) && token == _duckToken)
                    {
                        _backgroundReader.Volume = BackgroundBaseVolume;
                    }
                }
            }
            catch
            {
            }
        });
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
