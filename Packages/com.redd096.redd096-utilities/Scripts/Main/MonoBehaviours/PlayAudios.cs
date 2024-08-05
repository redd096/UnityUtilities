using UnityEngine;

namespace redd096
{
    /// <summary>
    /// This can be used to change Music in scene or can be called to play a sound from array
    /// </summary>
    [AddComponentMenu("redd096/Main/MonoBehaviours/Play Audios")]
    public class PlayAudios : MonoBehaviour
    {
        [SerializeField] bool playOnAwake;
        [SerializeField] bool stopMusicOnAwake;
        [SerializeField] AudioClass[] audios;

        private void Start()
        {
            //play on awake
            if (playOnAwake)
                PlayEveryAudio();

            //stop music for this scene
            if (stopMusicOnAwake)
                SoundManager.instance.StopMusic();
        }

        /// <summary>
        /// Play every audio in inspector
        /// </summary>
        public void PlayEveryAudio()
        {
            foreach (AudioClass audio in audios)
                SoundManager.instance.Play(audio, transform.position);
        }

        /// <summary>
        /// From the array in inspector, play audio with this name
        /// </summary>
        /// <param name="audioName"></param>
        public void PlayAudioByName(string audioName)
        {
            audioName = audioName.Trim();
            foreach (AudioClass audio in audios)
            {
                if (audio.IsValid() && audio.Name.Trim().Equals(audioName))
                {
                    SoundManager.instance.Play(audio, transform.position);
                    break;
                }
            }
        }

        /// <summary>
        /// From the array in inspector, play audio at this index
        /// </summary>
        /// <param name="index"></param>
        public void PlayAudioByIndex(int index)
        {
            if (index < audios.Length)
                SoundManager.instance.Play(audios[index], transform.position);
        }
    }
}