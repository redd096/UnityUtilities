namespace redd096
{
    using UnityEngine;
    using System.Collections;

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Instantiate sound at point")]
        [SerializeField] AudioSource audioPrefab = default;
        [SerializeField] float timeBeforeDeactive = 2;

        private AudioSource backgroundAudioSource;
        AudioSource BackgroundAudioSource
        {
            get
            {
                //create audio source if null
                if (backgroundAudioSource == null)
                    backgroundAudioSource = gameObject.AddComponent<AudioSource>();

                //return audio source
                return backgroundAudioSource;
            }
        }

        Transform soundsParent;
        Transform SoundsParent
        {
            get
            {
                if (soundsParent == null)
                    soundsParent = new GameObject("Sounds Parent").transform;

                return soundsParent;
            }
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

            //change only if different clip (so we can have same music in different scenes without stop)
            if (forceReplay || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }

        #endregion

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public void PlayBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //start music from this audio source
            Play(BackgroundAudioSource, clip, false, volume, loop);
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public void Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return;

            //instantiate at position, set parent
            AudioSource audioSource = pool.Instantiate(audioPrefab, position, Quaternion.identity);
            audioSource.transform.SetParent(SoundsParent);

            //play and start coroutine to deactivate
            Play(audioSource, clip, true, volume);
            StartCoroutine(DeactiveSoundAtPointCoroutine(audioSource));
        }

        IEnumerator DeactiveSoundAtPointCoroutine(AudioSource audioToDeactivate)
        {
            //wait
            yield return new WaitForSeconds(timeBeforeDeactive);

            //and deactive
            audioToDeactivate.gameObject.SetActive(false);
        }
    }
}