using UnityEngine;

namespace HexGrimmDev.Audio
{
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
}