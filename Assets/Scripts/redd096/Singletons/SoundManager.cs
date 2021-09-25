namespace redd096
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [System.Serializable]
    public struct AudioStruct
    {
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
    }

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        #region variables

        [Header("Instantiate background music")]
        [SerializeField] bool stopBackgroundMusicThisScene = false;
        [SerializeField] AudioSource musicPrefab = default;
        [SerializeField] AudioStruct musicThisScene = default;
        [SerializeField] bool loopMusicThisScene = true;
        [Tooltip("From 0 to 1, where 0 is 0 and 1 is volume to set")] [SerializeField] AnimationCurve fadeInMusic = default;
        [Tooltip("From 1 to 0, where 1 is current volume and 0 is 0")] [SerializeField] AnimationCurve fadeOutMusic = default;

        [Header("Instantiate sound at point")]
        [SerializeField] AudioSource sfxPrefab = default;

        [Header("Sounds On Click Button")]
        [SerializeField] AudioStruct[] soundsOnClick = default;

        //background music
        AudioSource musicBackgroundAudioSource;

        //sound at point
        private Transform soundsParent;
        Transform SoundsParent
        {
            get
            {
                if (soundsParent == null)
                    soundsParent = new GameObject("Sounds Parent").transform;

                return soundsParent;
            }
        }

        Pooling<AudioSource> poolingSounds = new Pooling<AudioSource>();

        //fade in and fade out coroutines
        Dictionary<AudioSource, Coroutine> fadeCoroutines = new Dictionary<AudioSource, Coroutine>();

        #endregion

        protected override void Awake()
        {
            base.Awake();

            //stop background music if playing
            if (stopBackgroundMusicThisScene)
            {
                instance.StopBackgroundMusic(true);
            }
            //else, on the instance, play new background music
            else
            {
                instance.PlayBackgroundMusic(musicThisScene.audioClip, true, musicThisScene.volume, loopMusicThisScene);
            }
        }

        /// <summary>
        /// Called by buttons in UI - play random sound on click
        /// </summary>
        public void PlayOnClick()
        {
            instance.Play(soundsOnClick, Vector3.zero);
        }

        #region static Play

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public static void Play(AudioSource audioSource, AudioClip clip, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different (so we can have same music in different scenes without stop) - or if set forceReplay or audioSource is not playing
            if (forceReplay || audioSource.isPlaying == false || audioSource.clip != clip || audioSource.volume != volume || audioSource.loop != loop)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }

        /// <summary>
        /// Start audio clip. Can set volume and loop. It use animation curve to fade out previous audio clip and fade in new one
        /// </summary>
        public static void PlayWithFade(AudioSource audioSource, AudioClip clip, AnimationCurve fadeIn, AnimationCurve fadeOut, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different (so we can have same music in different scenes without stop) - or if set forceReplay or audioSource is not playing
            if (forceReplay || audioSource.isPlaying == false || audioSource.clip != clip || audioSource.volume != volume || audioSource.loop != loop)
            {
                //if already running fade coroutine for this audiosource, stop it
                if (instance.fadeCoroutines.ContainsKey(audioSource))
                {
                    instance.StopCoroutine(instance.fadeCoroutines[audioSource]);
                    instance.fadeCoroutines.Remove(audioSource);
                }

                //start coroutine
                instance.fadeCoroutines.Add(audioSource, instance.StartCoroutine(instance.FadeAudioCoroutine(audioSource, clip, fadeIn, fadeOut, volume, loop)));
            }
        }

        IEnumerator FadeAudioCoroutine(AudioSource audioSource, AudioClip clip, AnimationCurve fadeIn, AnimationCurve fadeOut, float volume = 1, bool loop = false)
        {
            //if playing, do fade out (only if there is an animation curve and volume is not already at 0)
            if (audioSource.isPlaying && fadeOut.keys.Length > 0 && audioSource.volume > 0)
            {
                yield return FadeCoroutine(audioSource, audioSource.volume, fadeOut);
            }

            //set new clip, volume and loop - then be sure to play
            audioSource.clip = clip;
            audioSource.volume = fadeIn.keys.Length > 0 ? 0 : volume;       //if there is animation curve, set volume at 0, else set necessary volume
            audioSource.loop = loop;
            audioSource.Play();

            //start fade in (only if there is an animation curve)
            if (fadeIn.keys.Length > 0)
            {
                yield return FadeCoroutine(audioSource, volume, fadeIn);
            }
        }

        IEnumerator FadeCoroutine(AudioSource audioSource, float volume, AnimationCurve fadeCurve)
        {
            float currentTime = 0;

            //do fade
            while (currentTime < fadeCurve.keys[fadeCurve.length - 1].time)
            {
                currentTime += Time.deltaTime;

                //set volume using animation curve
                audioSource.volume = Mathf.Lerp(0, volume, fadeCurve.Evaluate(currentTime));

                yield return null;
            }
        }

        #endregion

        #region music background

        /// <summary>
        /// Start audio clip for background music. Can set volume and loop
        /// </summary>
        public AudioSource PlayBackgroundMusic(AudioClip clip, bool doFade, float volume = 1, bool loop = false)
        {
            if (clip == null)
                return null;

            //if there is no music audioSource, instantiate it
            if (musicBackgroundAudioSource == null)
            {
                if (musicPrefab == null)
                    return null;

                musicBackgroundAudioSource = Instantiate(musicPrefab, transform);       //child of this
                musicBackgroundAudioSource.transform.localPosition = Vector3.zero;      //and same position
            }

            //play it (with fade or not)
            if (doFade)
                PlayWithFade(musicBackgroundAudioSource, clip, fadeInMusic, fadeOutMusic, false, volume, loop);
            else
                Play(musicBackgroundAudioSource, clip, false, volume, loop);

            return musicBackgroundAudioSource;
        }

        /// <summary>
        /// Start background music with clip setted in sound manager
        /// </summary>
        /// <param name="doFade"></param>
        public AudioSource PlayBackgroundMusic(bool doFade)
        {
            return PlayBackgroundMusic(musicThisScene.audioClip, doFade, musicThisScene.volume, loopMusicThisScene);
        }

        /// <summary>
        /// Stop background music if playing
        /// </summary>
        public AudioSource StopBackgroundMusic(bool doFade)
        {
            //stop old background music
            if (musicBackgroundAudioSource)
            {
                //with fade
                if (doFade)
                {
                    //if already running fade coroutine for this audiosource, stop it
                    if (fadeCoroutines.ContainsKey(musicBackgroundAudioSource))
                    {
                        StopCoroutine(fadeCoroutines[musicBackgroundAudioSource]);
                        fadeCoroutines.Remove(musicBackgroundAudioSource);
                    }

                    //start coroutine
                    fadeCoroutines.Add(musicBackgroundAudioSource, StartCoroutine(FadeCoroutine(musicBackgroundAudioSource, musicBackgroundAudioSource.volume, fadeOutMusic)));
                }
                //or immediatly
                else
                {
                    musicBackgroundAudioSource.Stop();
                }
            }

            return musicBackgroundAudioSource;
        }

        #endregion

        #region sound at point

        /// <summary>
        /// Start audio clip at point. Can set volume. Use specific pooling
        /// </summary>
        public AudioSource Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return null;

            //instantiate (if didn't find deactivated, take first one in the pool)
            AudioSource audioSource = pool.Instantiate(sfxPrefab);
            if (audioSource == null && pool.PooledObjects.Count > 0)
                audioSource = pool.PooledObjects[0];

            //if still null, return
            if (audioSource == null)
                return null;

            //set position, rotation and parent
            audioSource.transform.position = position;
            audioSource.transform.SetParent(SoundsParent);

            //play and start coroutine to deactivate
            Play(audioSource, clip, true, volume);
            StartCoroutine(DeactiveSoundAtPointCoroutine(audioSource));

            return audioSource;
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public AudioSource Play(AudioClip clip, Vector3 position, float volume = 1)
        {
            //use this manager's pooling, instead of a specific one
            return Play(poolingSounds, clip, position, volume);
        }

        /// <summary>
        /// Start audio clip at point, with selected volume
        /// </summary>
        public AudioSource Play(AudioStruct audio, Vector3 position)
        {
            //use this manager's pooling, instead of a specific one
            return Play(poolingSounds, audio.audioClip, position, audio.volume);
        }

        IEnumerator DeactiveSoundAtPointCoroutine(AudioSource audioToDeactivate)
        {
            //wait to end the clip
            if (audioToDeactivate)
                yield return new WaitForSeconds(audioToDeactivate.clip.length);

            //and deactive
            if (audioToDeactivate)
                audioToDeactivate.gameObject.SetActive(false);
        }

        /// <summary>
        /// Start audio clip at point. Can set volume. Get clip random from the array
        /// </summary>
        public AudioSource Play(AudioClip[] clips, Vector3 position, float volume = 1)
        {
            //do only if there are elements in the array
            if (clips.Length > 0)
            {
                return Play(clips[Random.Range(0, clips.Length)], position, volume);
            }

            return null;
        }

        /// <summary>
        /// Start audio clip at point. Get clip and volume random from the array
        /// </summary>
        public AudioSource Play(AudioStruct[] audios, Vector3 position)
        {
            //do only if there are elements in the array
            if (audios.Length > 0)
            {
                return Play(audios[Random.Range(0, audios.Length)], position);
            }

            return null;
        }

        #endregion
    }
}