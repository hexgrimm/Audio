using System;

namespace HexGrimmDev.Audio
{
    public interface IAudioController : IDisposable
    {
        /// <summary>
        /// Enabled or disables all sounds in game. All music sources sets volume to = 0 and stops their playback;
        /// </summary>
        bool SoundEnabled { get; set; }
        /// <summary>
        /// Enables or disables all musics in game. All music sources sets volume to = 0 or MusicVolume value;
        /// </summary>
        bool MusicEnabled { get; set; }
        /// <summary>
        /// Sound volume range 1 - 0
        /// </summary>
        float SoundVolume { get; set; }
        /// <summary>
        /// Music volume in range 1 - 0
        /// </summary>
        float MusicVolume { get; set; }
    }
}
