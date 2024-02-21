using redd096.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace redd096
{
    /// <summary>
    /// Scriptable object used to set audios for the game
    /// </summary>
    [CreateAssetMenu(fileName = "AudioData", menuName = "redd096/Audio Data")]
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

        public Element GetElement(string elementName)
        {
            //find element in array by name
            if (Elements != null)
            {
                foreach (var element in Elements)
                    if (element.Name == elementName)
                        return element;
            }

            Debug.LogError("Impossible to find: " + elementName);
            return null;
        }

        //classes ----------------------------------------------------------------------------------------

        #region classes

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
            [Tooltip("If play new audio with same clip and volume, continue play or restart anyway?")] public bool ForceReplay;
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
}