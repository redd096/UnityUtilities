using redd096.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace redd096
{
    /// <summary>
    /// Scriptable object used to set audios for the game
    /// </summary>
    [CreateAssetMenu(fileName = "AudioData", menuName = "redd096/Datas/Audio Data")]
    public class AudioData : ScriptableObject
    {
        public Element[] Elements;

        private void OnValidate()
        {
            //if change volume in inspector, update in game
            if (Application.isPlaying)
            {
                if (SoundManager.instance)
                    SoundManager.instance.UpdateAudioSourcesVolume();
            }
        }

        public Element GetElement(string elementName, bool showErrors = true)
        {
            if (this == null)
            {
                if (showErrors) Debug.LogError("Missing Data!");
                return null;
            }

            //find element in array by name
            if (Elements != null)
            {
                foreach (var element in Elements)
                    if (element.Name == elementName)
                        return element;
            }

            if (showErrors) Debug.LogError("Impossible to find: " + elementName);
            return null;
        }

        //element ----------------------------------------------------------------------------------------

        #region element classes

        [System.Serializable]
        public class Element
        {
            public string Name;
            [Range(0, 1)] public float Volume;
            public PresetAudio Preset;

            [Space]
            public AudioClip[] AudioClips;

            public AudioClip RandomClip
            {
                get
                {
                    if (AudioClips != null && AudioClips.Length > 0)
                        return AudioClips[Random.Range(0, AudioClips.Length)];

                    return null;
                }
            }
        }

        [System.Serializable]
        public class PresetAudio
        {
            public EAudioType AudioType;
            public bool Loop;
            [EnableIf("AudioType", EAudioType.Music)] public bool Fade;
            [EnableIf("AudioType", EAudioType.Music)][Tooltip("If call to play this audio but it's already playing, continue play or restart it?")] public bool ForceReplay;
            public AudioMixerGroup AudioMixer;
            [Rename("", nameof(_enabled3D))] public SoundSettings3D SoundSettings3D;

            //editor
            private string _enabled3D => $"[{(SoundSettings3D.Enable3D ? "Enabled" : "Disabled")}] Sound Settings 3D";

            //check distance - if not enabled return default values (default on AudioSource when instantiated in scene)
            public float SpatialBlend => SoundSettings3D.Enable3D ? 1 : 0;
            public float DopplerLevel => SoundSettings3D.Enable3D ? SoundSettings3D.DopplerLevel : 1;
            public int Spread => SoundSettings3D.Enable3D ? SoundSettings3D.Spread : 0;
            public AudioRolloffMode RolloffMode => SoundSettings3D.Enable3D ? SoundSettings3D.RolloffMode : AudioRolloffMode.Logarithmic;
            public float MinDistance => SoundSettings3D.Enable3D ? SoundSettings3D.MinDistance : 1;
            public float MaxDistance => SoundSettings3D.Enable3D ? SoundSettings3D.MaxDistance : 500;
        }

        public enum EAudioType
        {
            Sfx, UI, Music
        }

        [System.Serializable]
        public class SoundSettings3D
        {
            [Tooltip("SpatialBlend on AudioSource: disabled is 0, enabled is 1")] public bool Enable3D;
            [Range(0f, 5f)] public float DopplerLevel;
            [Range(0, 360)] public int Spread;
            public AudioRolloffMode RolloffMode;
            public float MinDistance;
            public float MaxDistance;
        }

        #endregion
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
        private AudioData.Element GetElement(bool showErrors) { if (_element == null || string.IsNullOrEmpty(_element.Name)) _element = data.GetElement(elementName, showErrors); return _element; }
        public AudioData.Element Element => GetElement(true);
        public void RefreshElement() => _element = data.GetElement(elementName);    //force refresh also if _element is already != null

        //check if valid this class and element
        public bool IsValid() => this != null && GetElement(false) != null;
        public static implicit operator bool(AudioClass a) => a.IsValid();

        //public API
        public string Name => Element.Name;
        public float Volume => Element.Volume;
        public AudioData.PresetAudio Preset => Element.Preset;
        public AudioClip Clip => Element.RandomClip;

        /// <summary>
        /// This constructor is used to create the class without set data and element name, in case you need it in code but you don't need to set it in inspector
        /// </summary>
        /// <param name="element"></param>
        public AudioClass(AudioData.Element element)
        {
            _element = element;
        }

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