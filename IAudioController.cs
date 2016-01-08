using System;
using UnityEngine;

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

    public interface IAudioPlayer
    {
        /// <summary>
        /// plays audio clip if sound enabled.
        /// </summary>
        /// <param name="clip">Audio clip to play.</param>
        /// <param name="volumeProportion">volume in range 1 - 0, when plays its also affected by global volume setting.</param>
        /// <param name="looped">should clip play be looped</param>
        /// <returns> returns code for this sound call to control playback for concrete clip played.</returns>
        int PlayAudioClip2D(AudioClip clip, float volumeProportion = 1f, bool looped = false);

        /// <summary>
        /// Plays audio clip in concrete 3d position
        /// </summary>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="position">world position of audio source.</param>
        /// <param name="maxSoundDistance">parameter seted to audioSource.MaxDistance</param>
        /// <param name="volumeProportion">volume in range 1 - 0, when plays its also affected by global volume setting.</param>
        /// <param name="looped">should clip play be looped</param>
        /// <returns></returns>
        int PlayAudioClip3D(AudioClip clip, Vector3 position, float maxSoundDistance, float volumeProportion = 1f, bool looped = false);
        /// <summary>
        /// stop playing concrete clip.
        /// </summary>
        /// <param name="audioCode">code, recived from methods PlayAudioClip2D or PlayAudioClip3D</param>
        void StopPlayingClip(int audioCode);
        /// <summary>
        /// Returns true if audio code contains in player and can be controlled.
        /// </summary>
        /// <param name="audioCode">audio code</param>
        /// <returns></returns>
        bool IsAudioClipCodePlaying(int audioCode);
        /// <summary>
        /// Sets global audio listener to concrete position
        /// </summary>
        /// <param name="position">v3 in world coordinates</param>
        void SetAudioListenerToPosition(Vector3 position);
        /// <summary>
        /// Set position of source if source exist.
        /// </summary>
        /// <param name="audioCode">code of source</param>
        /// <param name="destinationPos">target position in world coordinates</param>
        void SetSourcePositionTo(int audioCode, Vector3 destinationPos);
    }

    public interface IMusicPlayer
    {
        /// <summary>
        /// plays music clip as 2d sound with concrete volume padding.
        /// </summary>
        /// <param name="clip">music clip</param>
        /// <param name="volumeProportion">volume proportions of sound in range of 1 - 0. Its also affected by global music volume settings</param>
        /// <returns>concrete music playback code for future control</returns>
        int PlayMusicClip(AudioClip clip, float volumeProportion = 1f);
        /// <summary>
        /// stops playing music clip and clear data for this code.
        /// </summary>
        /// <param name="audioCode">audio code to find audio clip playback</param>
        void StopPlayingMusicClip(int audioCode);
        /// <summary>
        /// Pauses concrete music clip play, it could be resumed.
        /// </summary>
        /// <param name="audioCode"></param>
        void PausePlayingClip(int audioCode);
        /// <summary>
        /// Resumes concrete music clip play if it was paused before.
        /// </summary>
        /// <param name="audioCode"></param>
        void ResumeClipIfInPause(int audioCode);
        /// <summary>
        /// Returns true if audio code contains in player and can be controlled.
        /// </summary>
        /// <param name="audioCode">audio code</param>
        /// <returns></returns>
        bool IsMusicClipCodePlaying(int audioCode);
    }
}
