using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace HexGrimmDev.Audio
{
    public class AudioController : IAudioController, IAudioPlayer, IMusicPlayer, IDisposable
    {
        private readonly SavableValue<bool> soundEnabled = new SavableValue<bool>("HexGrimmDev.Audio.AudioController.soundEnabled", true);
        private readonly SavableValue<bool> musicEnabled = new SavableValue<bool>("HexGrimmDev.Audio.AudioController.musicEnabled", true);
        private readonly SavableValue<float> soundVolume = new SavableValue<float>("HexGrimmDev.Audio.AudioController.soundVolume", 1f);
        private readonly SavableValue<float> musicVolume = new SavableValue<float>("HexGrimmDev.Audio.AudioController.musicVolume", 1f);
        private readonly Dictionary<int, AudioSourceData> sourceMedia = new Dictionary<int, AudioSourceData>();

        private AudioPresenter presenter;
        private int audioCodeIndex;

        bool IAudioController.SoundEnabled
        {
            get { return soundEnabled.Value; }
            set
            {
                if (soundEnabled.Value != value)
                {
                    foreach (var key in sourceMedia.Keys)
                    {
                        var sourceData = sourceMedia[key];
                        if (!sourceData.IsMusic)
                        {
                            sourceData.Source.volume = value ? sourceData.RequestedVolume * soundVolume.Value : 0;
                        }
                    }

                    soundEnabled.Value = value;
                }
            }
        }

        bool IAudioController.MusicEnabled
        {
            get { return musicEnabled.Value; }
            set
            {
                if (musicEnabled.Value != value)
                {
                    foreach (var key in sourceMedia.Keys)
                    {
                        var sourceData = sourceMedia[key];
                        if (sourceData.IsMusic)
                        {
                            sourceData.Source.volume = value ? sourceData.RequestedVolume * musicVolume.Value : 0;
                        }
                    }

                    musicEnabled.Value = value;
                }
            }
        }

        float IAudioController.SoundVolume
        {
            get { return soundVolume.Value; }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0; 

                soundVolume.Value = value;

                if (!soundEnabled.Value)
                    return;

                foreach (var key in sourceMedia.Keys)
                {
                    var sourceData = sourceMedia[key];
                    if (!sourceData.IsMusic)
                    {
                        sourceData.Source.volume = sourceData.RequestedVolume * soundVolume.Value;
                    }
                }
            }
        }

        float IAudioController.MusicVolume
        {
            get { return musicVolume.Value; }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < 0)
                    value = 0;

                musicVolume.Value = value;

                if (!musicEnabled.Value)
                    return;

                foreach (var key in sourceMedia.Keys)
                {
                    var sourceData = sourceMedia[key];
                    if (sourceData.IsMusic)
                    {
                        sourceData.Source.volume = sourceData.RequestedVolume * musicVolume.Value;
                    }
                }
            }
        }

        public AudioController()
        {
            Initialization();
        }

        void IDisposable.Dispose()
        {
            sourceMedia.Clear();
            var g = presenter.gameObject;
            presenter = null;
#if UNITY_EDITOR
            GameObject.DestroyImmediate(g);
#else
            GameObject.Destroy(g);
#endif
        }

        int IMusicPlayer.PlayMusicClip(AudioClip clip, float volumeProportion)
        {
            if (volumeProportion > 1)
                volumeProportion = 1;
            else if (volumeProportion < 0)
                volumeProportion = 0;

            ScanForEndedSources();
            audioCodeIndex++;

            var source = presenter.gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.priority = 0;
            source.spatialBlend = 0;
            source.minDistance = 0.06f;

            var data = new AudioSourceData()
            {
                Is3Dsound = false,
                IsMusic = true,
                OnPause = false,
                RequestedVolume = volumeProportion,
                Source = source,
                SourceRequestedPos = Vector3.one,
                AudioCode = audioCodeIndex,
            };

            source.volume = musicEnabled.Value ? volumeProportion * musicVolume.Value : 0;
            source.Play();
            sourceMedia.Add(data.AudioCode, data);
            return audioCodeIndex;
        }

        void IMusicPlayer.StopPlayingMusicClip(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return;

            var s = sourceMedia[audioCode];
            sourceMedia.Remove(audioCode);
            s.Source.Stop();
            SmartDestroy(s.Source);
        }

        void IMusicPlayer.PausePlayingClip(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return;

            var s = sourceMedia[audioCode];
            s.Source.Pause();
            s.OnPause = true;
        }

        void IMusicPlayer.ResumeClipIfInPause(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return;

            var s = sourceMedia[audioCode];
            s.Source.UnPause();
            s.OnPause = false;
        }

        bool IMusicPlayer.IsMusicClipCodePlaying(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return false;

            var s = sourceMedia[audioCode];
            return s.Source.isPlaying;
        }

        int IAudioPlayer.PlayAudioClip2D(AudioClip clip, float volumeProportion, bool looped)
        {
            if (volumeProportion > 1)
                volumeProportion = 1;
            else if (volumeProportion < 0)
                volumeProportion = 0; 

            ScanForEndedSources();
            audioCodeIndex++;

            var source = presenter.gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = looped;
            source.spatialBlend = 0;
            source.minDistance = 0.06f;
            var data = new AudioSourceData()
            {
                Is3Dsound = false,
                IsMusic = false,
                OnPause = false,
                RequestedVolume = volumeProportion,
                Source = source,
                SourceRequestedPos = Vector3.one,
                AudioCode = audioCodeIndex,
            };

            source.volume = soundEnabled.Value ? volumeProportion*soundVolume.Value : 0;
            source.Play();
            sourceMedia.Add(data.AudioCode, data);
            return audioCodeIndex;
        }

        int IAudioPlayer.PlayAudioClip3D(AudioClip clip, Vector3 position, float maxSoundDistance, float volumeProportion, bool looped)
        {
            if (volumeProportion > 1)
                volumeProportion = 1;
            else if (volumeProportion < 0)
                volumeProportion = 0; 

            ScanForEndedSources();
            audioCodeIndex++;

            var go = new GameObject("Audio 3D source");
            go.transform.SetParent(presenter.transform);
            go.transform.position = position;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = looped;
            source.spatialBlend = 1;
            source.dopplerLevel = 0;
            source.reverbZoneMix = 0;
            source.pitch = 1 + Random.Range(-0.1f, 0.1f);
            source.minDistance = 0.4f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;

            source.maxDistance = maxSoundDistance;

            var data = new AudioSourceData()
            {
                Is3Dsound = true,
                IsMusic = false,
                OnPause = false,
                RequestedVolume = volumeProportion,
                Source = source,
                SourceRequestedPos = position,
                AudioCode = audioCodeIndex,
                CachedTransform = source.transform,
            };

            source.volume = soundEnabled.Value ? volumeProportion * soundVolume.Value : 0;
            source.Play();
            sourceMedia.Add(data.AudioCode, data);
            return audioCodeIndex;
        }

        void IAudioPlayer.StopPlayingClip(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode)) 
                return;

            var s = sourceMedia[audioCode];
            sourceMedia.Remove(audioCode);
            s.Source.Stop();

            if (s.Is3Dsound)
            {
                SmartDestroy(s.Source.gameObject);
            }
            else
            {
                SmartDestroy(s.Source);
            }
        }

        bool IAudioPlayer.IsAudioClipCodePlaying(int audioCode)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return false;

            var s = sourceMedia[audioCode];
            return s.Source.isPlaying;
        }

        void IAudioPlayer.SetAudioListenerToPosition(Vector3 position)
        {
            presenter.AudioListener.transform.position = position;
        }

        void IAudioPlayer.SetSourcePositionTo(int audioCode, Vector3 destinationPos)
        {
            if (!sourceMedia.ContainsKey(audioCode))
                return;

            var data = sourceMedia[audioCode];
            if (!data.Is3Dsound)
            {
                Debug.LogError("try control 2d sound as 3d sound");
                return;
            }

            if (data.SourceRequestedPos == destinationPos)
                return;

            data.SourceRequestedPos = destinationPos;
            data.CachedTransform.position = destinationPos;
        }

        private void Initialization()
        {
            presenter = new GameObject("AUDIO").AddComponent<AudioPresenter>();
            var listener = new GameObject("LISTENER").AddComponent<AudioListener>();
            listener.transform.SetParent(presenter.transform);
            presenter.AudioListener = listener;
        }

        private void ScanForEndedSources()
        {
            var todel = new Dictionary<int, AudioSourceData>();

            foreach (var k in sourceMedia.Keys)
            {
                var source = sourceMedia[k];
                if (!source.OnPause && !source.Source.isPlaying)
                    todel.Add(k, source);
            }

            foreach (var k in todel.Keys)
            {
                var source = todel[k];
                sourceMedia.Remove(k);

                if (source.Is3Dsound && !source.IsMusic)
                    SmartDestroy(source.Source.gameObject);
                else
                    SmartDestroy(source.Source);
            }
        }
        
        private void SmartDestroy(Object obj)
        {
            #if UNITY_EDITOR
            Object.DestroyImmediate(obj);
            #else
            Object.Destroy(obj);
            #endif
        }

        internal class AudioSourceData
        {
            public AudioSource Source;
            public int AudioCode;
            public bool IsMusic;
            public bool Is3Dsound;
            public Vector3 SourceRequestedPos;
            public float RequestedVolume;
            public bool OnPause;
            public Transform CachedTransform;
        }
    }
}