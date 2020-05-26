namespace redd096
{
    using UnityEngine;

    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource audioSource;

        #region private API

        void CreateAudioSource()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        AudioSource GetAudioSource()
        {
            //create audio source if null
            if (audioSource == null)
                CreateAudioSource();

            //return audio source
            return audioSource;
        }

        #endregion

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public void StartMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            GetAudioSource();

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