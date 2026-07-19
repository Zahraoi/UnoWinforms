using System.Media;

namespace Uno.WinForms.Services;

public static class SoundService
{
    private static readonly object SyncLock = new();
    private static SoundPlayer? _buttonPlayer;
    private static SoundPlayer? _cardPlayer;
    private static SoundPlayer? _drawPlayer;
    private static SoundPlayer? _errorPlayer;
    private static SoundPlayer? _winPlayer;
    private static SoundPlayer? _losePlayer;
    private static SoundPlayer? _backgroundPlayer;
    private static bool _initialized;

    public static void PlayButtonClick()
    {
        EnsureInitialized();
        Play(_buttonPlayer, SystemSounds.Asterisk);
    }

    public static void PlayCardPlay()
    {
        EnsureInitialized();
        Play(_cardPlayer, SystemSounds.Beep);
    }

    public static void PlayCardDraw()
    {
        EnsureInitialized();
        Play(_drawPlayer, SystemSounds.Asterisk);
    }

    public static void PlayError()
    {
        EnsureInitialized();
        Play(_errorPlayer, SystemSounds.Hand);
    }

    public static void PlayWin()
    {
        EnsureInitialized();
        Play(_winPlayer, SystemSounds.Exclamation);
    }

    public static void PlayLose()
    {
        EnsureInitialized();
        Play(_losePlayer, SystemSounds.Question);
    }

    public static void StartBackgroundLoop()
    {
        EnsureInitialized();
        try
        {
            _backgroundPlayer?.PlayLooping();
        }
        catch
        {
        }
    }

    public static void StopBackgroundLoop()
    {
        try
        {
            _backgroundPlayer?.Stop();
        }
        catch
        {
        }
    }

    private static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (SyncLock)
        {
            if (_initialized)
            {
                return;
            }

            _buttonPlayer = TryLoad("button_click.wav");
            _cardPlayer = TryLoad("card_play.wav");
            _drawPlayer = TryLoad("card_draw.wav");
            _errorPlayer = TryLoad("error.wav");
            _winPlayer = TryLoad("win.wav");
            _losePlayer = TryLoad("lose.wav");
            _backgroundPlayer = TryLoad("background_loop.wav");
            _initialized = true;
        }
    }

    private static SoundPlayer? TryLoad(string fileName)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            var player = new SoundPlayer(path);
            player.LoadAsync();
            return player;
        }
        catch
        {
            return null;
        }
    }

    private static void Play(SoundPlayer? player, SystemSound fallback)
    {
        try
        {
            if (player is not null)
            {
                player.Play();
                return;
            }

            fallback.Play();
        }
        catch
        {
        }
    }
}
