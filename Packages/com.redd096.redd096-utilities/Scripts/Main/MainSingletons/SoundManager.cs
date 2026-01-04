using redd096.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace redd096
{
    [AddComponentMenu("redd096/Main/Singletons/Sound Manager")]
    public class SoundManager : LazySingleton<SoundManager>
    {
        [Header("Music Fade")]
        [Tooltip("From 0 to 1, where 0 is no volume and 1 is the volume to set")][SerializeField] AnimationCurve fadeInMusic = default;
        [Tooltip("From 1 to 0, where 1 is current volume and 0 is no volume")][SerializeField] AnimationCurve fadeOutMusic = default;
        [Button]
        void SetDefaultFades()
        {
            fadeInMusic = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            fadeOutMusic = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        }

        //sound parent (instantiate if null)
        private Transform soundsParent;
        public Transform SoundsParent
        {
            get
            {
                if (soundsParent == null) { soundsParent = new GameObject("Sounds Parent").transform; }
                return soundsParent;
            }
        }
        private Transform soundsParentPersistent;
        public Transform SoundsParentPersistent
        {
            get
            {
                if (soundsParentPersistent == null) { soundsParentPersistent = new GameObject("Sounds Parent Persistent").transform; DontDestroyOnLoad(soundsParentPersistent); }
                return soundsParentPersistent;
            }
        }

        //audio sources
        AudioSource musicAudioSource;
        Pooling<AudioSource> pooling = new Pooling<AudioSource>();
        AudioSource prefabAudioSource;

        //volumes
        Dictionary<AudioSource, AudioClass> savedAudios = new Dictionary<AudioSource, AudioClass>();
        float volumeMusic = 1;
        float volumeSFX = 1;
        float volumeUI = 1;

        //coroutines
        Dictionary<AudioSource, Coroutine> fadeCoroutines = new Dictionary<AudioSource, Coroutine>();
        Dictionary<AudioSource, Coroutine> deactivateAudioSourceCoroutines = new Dictionary<AudioSource, Coroutine>();

        //todo
        //fade and force replay also for sounds? (not only music)
        //to do it: in Play, instead of call Play_SoundManagerInternal, create one PlaySound same as PlayMusic to call, where if audioSource != null then check replay and fade

        /// <summary>
        /// Play sound with presets from AudioClass
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="position"></param>
        /// <param name="audioSource">You can specify an audioSource to play sound</param>
        /// <returns></returns>
        public AudioSource Play(AudioClass audio, Vector3 position = default, AudioSource audioSource = null)
        {
            if (audio.IsValid() == false)
                return null;

            //play music
            if (audio.Preset.AudioType == AudioData.EAudioType.Music)
            {
                PlayMusic(audio);
                return null;    //if play music, don't need to return audio source
            }
            //play sfx or ui
            else
            {
                return Play_SoundManagerInternal(audio, position, audioSource);
            }
        }

        /// <summary>
        /// Play sound. Instead of use AudioClass, set every parameter
        /// </summary>
        /// <param name="audioClips"></param>
        /// <param name="position"></param>
        /// <param name="audioSource">You can specify an audioSource to play sound</param>
        /// <returns></returns>
        public AudioSource Play(AudioClip[] audioClips, Vector3 position, float volume = 1f, AudioSource audioSource = null,
            AudioData.EAudioType audioType = AudioData.EAudioType.Sfx, bool loop = false, bool fade = false, bool forceReplay = false, AudioMixerGroup audioMixer = null,
            bool enable3D = false, float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1f, float maxDistance = 500f,
            bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128, float pitch = 1, float stereoPan = 0, float reverbZoneMix = 1)
        {
            //create audio class and element
            AudioClass audio = new AudioClass(new AudioData.Element()
            {
                Name = "Instantiated Audio",
                Volume = volume,
                AudioClips = audioClips,
                Preset = new AudioData.PresetAudio()
                {
                    AudioType = audioType,
                    Loop = loop,
                    Fade = fade,
                    ForceReplay = forceReplay,
                    AudioMixer = audioMixer,
                    SoundSettings3D = new AudioData.SoundSettings3D()
                    {
                        Enable3D = enable3D,
                        DopplerLevel = dopplerLevel,
                        Spread = spread,
                        RolloffMode = rolloffMode,
                        MinDistance = minDistance,
                        MaxDistance = maxDistance
                    },
                    OtherSettings = new AudioData.OtherSettings()
                    {
                        BypassEffects = bypassEffects,
                        BypassListenerEffects = bypassListenerEffects,
                        BypassReverbZones = bypassReverbZones,
                        Priority = priority,
                        Pitch = pitch,
                        StereoPan = stereoPan,
                        ReverbZoneMix = reverbZoneMix
                    }
                }
            });

            //and call normally PlaySound with audio class
            return Play(audio, position, audioSource);
        }

        #region generic

        /// <summary>
        /// Stop audio source
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="keepActiveInScene">When use specific audioSource, keep it active, so the pooling can't use it for other audios</param>
        public void StopSound(AudioSource audioSource, bool keepActiveInScene = false)
        {
            if (audioSource)
            {
                audioSource.Stop();
            }

            //stop also coroutines
            StopCoroutine_SoundManagerInternal(fadeCoroutines, audioSource);
            StopCoroutine_SoundManagerInternal(deactivateAudioSourceCoroutines, audioSource);

            //and deactive audio source
            if (keepActiveInScene == false)
            {
                if (audioSource)
                    audioSource.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Stop music audio source
        /// </summary>
        public void StopMusic()
        {
            StopSound(musicAudioSource, true);
        }

        /// <summary>
        /// Pauses audio source
        /// </summary>
        /// <param name="source"></param>
        public virtual void PauseSound(AudioSource audioSource)
        {
            if (audioSource)
                audioSource.Pause();
        }

        /// <summary>
        /// Resume audio source
        /// </summary>
        /// <param name="source"></param>
        public virtual void ResumeSound(AudioSource audioSource)
        {
            if (audioSource)
                audioSource.Play();
        }

        /// <summary>
        /// Update audio sources in scene
        /// </summary>
        public void UpdateAudioSourcesVolume()
        {
            foreach (AudioSource source in savedAudios.Keys)
            {
                if (source)
                    source.volume = GetCorrectVolume(savedAudios[source]);
            }
        }

        /// <summary>
        /// Multiply audio.Volume for the saved volume for this audioType. So if for example in your game you set VolumeSFX to 0.5f, you will receive half of audio.Volume
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        public float GetCorrectVolume(AudioClass audio)
        {
            switch (audio.Preset.AudioType)
            {
                case AudioData.EAudioType.Sfx:
                    return audio.Volume * volumeSFX;
                case AudioData.EAudioType.UI:
                    return audio.Volume * volumeUI;
                case AudioData.EAudioType.Music:
                    return audio.Volume * volumeMusic;
                default:
                    return audio.Volume;
            }
        }

        /// <summary>
        /// Multiply desiredVolume for the saved volume for this audioType. So if for example in your game you set VolumeSFX to 0.5f, you will receive half of desiredVolume
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="desiredVolume"></param>
        /// <returns></returns>
        public float GetCorrectVolume(AudioData.EAudioType audioType, float desiredVolume)
        {
            switch (audioType)
            {
                case AudioData.EAudioType.Sfx:
                    return desiredVolume * volumeSFX;
                case AudioData.EAudioType.UI:
                    return desiredVolume * volumeUI;
                case AudioData.EAudioType.Music:
                    return desiredVolume * volumeMusic;
                default:
                    return desiredVolume;
            }
        }

        /// <summary>
        /// Fade audio source volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="duration"></param>
        /// <param name="initialVolume"></param>
        /// <param name="finalVolume"></param>
        /// <returns></returns>
        public IEnumerator FadeCoroutineLinear(AudioSource source, float duration, float initialVolume, AudioClass finalVolume)
        {
            float delta = 0;
            while (delta < 1)
            {
                delta += Time.deltaTime / duration;
                source.volume = Mathf.Lerp(initialVolume, GetCorrectVolume(finalVolume), delta);
                yield return null;
            }
        }

        /// <summary>
        /// Fade audio source volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="curve">curve value must go from 0 to 1, where 0 is initialVolume and 1 is finalVolume</param>
        /// <param name="initialVolume"></param>
        /// <param name="finalVolume"></param>
        /// <returns></returns>
        public IEnumerator FadeCoroutine(AudioSource source, AnimationCurve curve, float initialVolume, AudioClass finalVolume)
        {
            //if there is no curve, set immediatly sound and stop coroutine
            if (curve == null || curve.keys.Length <= 0)
            {
                source.volume = GetCorrectVolume(finalVolume);
                yield break;
            }

            float startTime = Time.time;
            float duration = curve.keys[curve.keys.Length - 1].time;    //last keyframe time
            float elapsedTime;
            while (Time.time - startTime < duration)
            {
                elapsedTime = Time.time - startTime;
                source.volume = Mathf.Lerp(initialVolume, GetCorrectVolume(finalVolume), curve.Evaluate(elapsedTime));
                yield return null;
            }
            source.volume = GetCorrectVolume(finalVolume);
        }

        /// <summary>
        /// Deactivate audio source when it stops to play sound
        /// </summary>
        /// <param name="audioSource"></param>
        /// <returns></returns>
        public IEnumerator DeactivateAudioSourceCoroutine(AudioSource audioSource)
        {
            //wait to end the clip
            if (audioSource && audioSource.clip)
            {
                yield return new WaitForSeconds(audioSource.clip.length / Mathf.Abs(audioSource.pitch));
            }

            //if still not ended (for example if player paused the audio source), continue to wait
            while (audioSource && audioSource.clip && audioSource.time < audioSource.clip.length)
            {
                yield return null;

                //when complete to play the audio, it moves to time 0. So if after one frame is still at 0, then it's not moving, it's already completed
                if (audioSource && audioSource.time == 0)
                    break;
            }

            //and deactive
            if (audioSource)
                audioSource.gameObject.SetActive(false);
        }

        #endregion

        #region settings

        /// <summary>
        /// Set Master volume
        /// </summary>
        /// <param name="value"></param>
        public void SetMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        /// <summary>
        /// Set volume for this type of audios
        /// </summary>
        /// <param name="audioType"></param>
        /// <param name="value"></param>
        public void SetTypeVolume(AudioData.EAudioType audioType, float value)
        {
            switch (audioType)
            {
                case AudioData.EAudioType.Sfx:
                    volumeSFX = value;
                    break;
                case AudioData.EAudioType.UI:
                    volumeUI = value;
                    break;
                case AudioData.EAudioType.Music:
                    volumeMusic = value;
                    break;
            }
            UpdateAudioSourcesVolume();
        }

        #endregion

        #region private Music API

        /// <summary>
        /// Play music always with same AudioSource
        /// </summary>
        /// <param name="music"></param>
        private void PlayMusic(AudioClass music)
        {
            //if try to play music that is already playing, do not restart it
            if (musicAudioSource != null && musicAudioSource.isPlaying && musicAudioSource.clip == music.Clip)
            {
                //this only if it isn't forced to replay
                if (music.Preset.ForceReplay == false)
                    return;
            }

            //if not setted, set default values for fade
            if (fadeInMusic == null || fadeInMusic.keys == null || fadeInMusic.keys.Length <= 0)
                fadeInMusic = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            if (fadeOutMusic == null || fadeOutMusic.keys == null || fadeOutMusic.keys.Length <= 0)
                fadeOutMusic = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

            if (musicAudioSource == null)
                musicAudioSource = GenerateAudioSource(persistent: true);

            //else, play music
            StartCoroutine_SoundManagerInternal(fadeCoroutines, musicAudioSource, PlayMusicCoroutine(music));
        }

        private IEnumerator PlayMusicCoroutine(AudioClass music)
        {
            //fade out
            if (musicAudioSource != null)
            {
                if (music.Preset.Fade && musicAudioSource.isPlaying)
                    yield return FadeCoroutine(musicAudioSource, fadeOutMusic, 0f, savedAudios[musicAudioSource]);    //inverse volumes, because to make curve more user friendly we make it go from 1 to 0 instead of 0 to 1

                //stop to be sure. For example if call to replay same audio already playing, we want it to restart
                StopSound(musicAudioSource, true);
            }

            //play music with fade in (both play and replay same clip)
            musicAudioSource = Play_SoundManagerInternal(music, position: default, musicAudioSource, music.Preset.Fade, persistent: true);
        }

        #endregion

        #region private API

        private AudioSource Play_SoundManagerInternal(AudioClass audio, Vector3 position = default, AudioSource audioSource = null, bool fade = false, bool persistent = false)
        {
            //if audio source is null, get from pooling
            if (audioSource == null)
            {
                audioSource = GenerateAudioSource(persistent);
            }

            //set position
            audioSource.transform.position = position;

            //values
            audioSource.clip = audio.Clip;

            //preset
            audioSource.loop = audio.Preset.Loop;
            audioSource.outputAudioMixerGroup = audio.Preset.AudioMixer;

            //sound settings 3d
            audioSource.spatialBlend = audio.Preset.SpatialBlend;
            audioSource.dopplerLevel = audio.Preset.DopplerLevel;
            audioSource.spread = audio.Preset.Spread;
            audioSource.rolloffMode = audio.Preset.RolloffMode;
            audioSource.minDistance = audio.Preset.MinDistance;
            audioSource.maxDistance = audio.Preset.MaxDistance;

            //other settings
            audioSource.bypassEffects = audio.Preset.OtherSettings.BypassEffects;
            audioSource.bypassListenerEffects = audio.Preset.OtherSettings.BypassListenerEffects;
            audioSource.bypassReverbZones = audio.Preset.OtherSettings.BypassReverbZones;
            audioSource.priority = audio.Preset.OtherSettings.Priority;
            audioSource.pitch = audio.Preset.OtherSettings.Pitch;
            audioSource.panStereo = audio.Preset.OtherSettings.StereoPan;
            audioSource.reverbZoneMix = audio.Preset.OtherSettings.ReverbZoneMix;

            //volume
            if (fade)
            {
                StartCoroutine_SoundManagerInternal(fadeCoroutines, audioSource, FadeCoroutine(audioSource, fadeInMusic, 0f, audio));
            }
            else
            {
                audioSource.volume = GetCorrectVolume(audio);
            }

            //add to list (or edit AudioClass if re-use same audioSource)
            savedAudios[audioSource] = audio;

            //play audio in scene
            audioSource.Play();

            //start deactivate coroutine if loop is false
            if (audio.Preset.Loop == false)
                StartCoroutine_SoundManagerInternal(deactivateAudioSourceCoroutines, audioSource, (DeactivateAudioSourceCoroutine(audioSource)));

            //if user passed an audio source, return it to continue use it. Else, return found audio source
            return audioSource;
        }

        private void StartCoroutine_SoundManagerInternal(Dictionary<AudioSource, Coroutine> coroutines, AudioSource audioSource, IEnumerator coroutine)
        {
            //stop coroutine if already running
            StopCoroutine_SoundManagerInternal(coroutines, audioSource);

            //start new coroutine
            coroutines[audioSource] = StartCoroutine(coroutine);
        }

        private void StopCoroutine_SoundManagerInternal(Dictionary<AudioSource, Coroutine> coroutines, AudioSource audioSource)
        {
            //stop coroutine if already running
            if (coroutines.ContainsKey(audioSource))
            {
                if (coroutines[audioSource] != null) StopCoroutine(coroutines[audioSource]);
                coroutines.Remove(audioSource);
            }
        }

        private AudioSource GenerateAudioSource(bool persistent)
        {
            //if pooling prefab is null, create it
            if (prefabAudioSource == null)
            {
                prefabAudioSource = new GameObject("AudioSource Prefab", typeof(AudioSource)).GetComponent<AudioSource>();
                prefabAudioSource.transform.SetParent(transform);   //set child to not destroy when change scene
            }
            AudioSource audioSource = pooling.Instantiate(prefabAudioSource);

            //set parent when audio source is null (if user gives an audioSource as parameter, we don't touch its parent)
            audioSource.transform.SetParent(persistent ? SoundsParentPersistent : SoundsParent);    //persistent for music, else normal SoundsParent

            return audioSource;
        }

        #endregion
    }
}