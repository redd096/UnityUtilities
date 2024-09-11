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

        /// <summary>
        /// Get element in array by name
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="showErrors"></param>
        /// <returns></returns>
        public Element GetElement(string elementName, bool showErrors = true)
        {
            if (Elements != null)
            {
                foreach (var element in Elements)
                    if (element.Name == elementName)
                        return element;
            }

            if (showErrors) Debug.LogError("Impossible to find: " + elementName);
            return null;
        }

        #region element classes

        [System.Serializable]
        public class Element
        {
            public string Name;
            [Range(0, 1)] public float Volume;
            [Rename("Preset ", nameof(_presetDetails))] public PresetAudio Preset;

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

            //editor
            private string _presetDetails => $"[{Preset.AudioType}]";
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
            public OtherSettings OtherSettings;

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

        [System.Serializable]
        public class OtherSettings
        {
            public bool BypassEffects;
            public bool BypassListenerEffects;
            public bool BypassReverbZones;
            [Range(0, 256)] public int Priority = 128;
            [Range(-3f, 3f)] public float Pitch = 1;
            [Range(-1f, 1f)] public float StereoPan = 0;
            [Range(0f, 1.1f)] public float ReverbZoneMix = 1;
        }

        #endregion
    }
}