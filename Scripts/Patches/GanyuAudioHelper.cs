using Godot;

public static class NovaAudioHelper
{    // 用于加载和播放本地音频文件
    private static AudioStreamPlayer? _player;

    public static void PlayOneShot(string path, float volume = 1.0f)
    {
        if (!ResourceLoader.Exists(path)) return;
        if (_player == null || !_player.IsInsideTree())
        {
            _player = new AudioStreamPlayer();
            if (Engine.GetMainLoop() is SceneTree tree && tree.CurrentScene != null)
            {
                tree.CurrentScene.AddChild(_player);
            }
            else
            {
                return;
            }
        }

        var stream = ResourceLoader.Load<AudioStream>(path);
        if (stream != null)
        {
            _player.Stream = stream;
            _player.VolumeDb = Mathf.LinearToDb(volume);
            _player.Play();
        }
    }
}