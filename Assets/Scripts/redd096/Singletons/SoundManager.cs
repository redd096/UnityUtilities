namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource backgroundAudioSource;

        #region private API

        void CreateAudioSource()
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        }

        AudioSource GetAudioSource()
        {
            //create audio source if null
            if (backgroundAudioSource == null)
                CreateAudioSource();

            //return audio source
            return backgroundAudioSource;
        }

        #endregion

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public void StartBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            GetAudioSource();

            //start music from this audio source
            StartMusic(backgroundAudioSource, clip, volume, loop);
        }

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public static void StartMusic(AudioSource audioSource, AudioClip clip, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different clip (so we can have same music in different scenes without stop)
            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }
    }
}