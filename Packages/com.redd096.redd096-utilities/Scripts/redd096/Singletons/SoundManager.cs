using redd096.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Music")]
        [Tooltip("From 0 to 1, where 0 is no volume and 1 is volume to set")][SerializeField] AnimationCurve fadeInMusic = default;
        [Tooltip("From 1 to 0, where 1 is current volume and 0 is no volume")][SerializeField] AnimationCurve fadeOutMusic = default;

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

        AudioSource musicAudioSource;
        Dictionary<AudioSource, AudioClass> poolAudioSources = new Dictionary<AudioSource, AudioClass>();
        float volumeMusic = 1;
        float volumeSFX = 1;
        float volumeUI = 1;

        //we don't need AudioSource prefab and we don't need different prefab for different situations, but we can instantiate a single default audio source.
        //we create the functions for AudioData, where everything in the audio source (2d, 3d, etc..) is setted by AudioData
        //and we create functions where user can pass AudioClip instead of AudioData, but must set also other variables, like in MoreMountains with infinite optional variables

        //we have to edit FeedbackRedd096 probably
        //and also OptionsManager have calls on OldSoundManager
        //we can edit also ParticlesManager and InstantiateGameObjectManager ??

        public void UpdateAudioSourcesVolume()
        {

        }

        public void PlaySound(AudioClass audio, Vector3 position)
        {

        }
    }

    #region audio class

    /// <summary>
    /// Class used to manage AudioData and SoundManager
    /// </summary>
    [System.Serializable]
    public class AudioClass
    {
        //inspector
        [SerializeField] AudioData data;
        [Dropdown("GetNames")][SerializeField] string elementName;

        //get from data
        private AudioData.Element _element;
        private AudioData.Element GetElement() { if (data == null) Debug.LogError("Missing Data!"); return data ? data.GetElement(elementName) : null; }
        public AudioData.Element Element { get { if (_element == null || string.IsNullOrEmpty(_element.Name)) _element = GetElement(); return _element; } }

        //check if null: this class, data and element
        public bool IsValid() => this != null && data != null && Element != null;
        public static implicit operator bool(AudioClass a) => a.IsValid();

#if UNITY_EDITOR
        string[] GetNames()
        {
            if (data == null)
                return new string[0];

            string[] s = new string[data.Elements.Length];
            for (int i = 0; i < s.Length; i++)
                s[i] = data.Elements[i].Name;

            return s;
        }
#endif
    }

    #endregion
}