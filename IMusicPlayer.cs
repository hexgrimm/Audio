using UnityEngine;

namespace HexGrimmDev.Audio
{
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