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
            [ShowIf("AudioType", EAudioType.Music)] public bool Fade;
            public AudioMixerGroup AudioMixer;
            public CheckDistance_AudioPreset CheckDistance;

            //check distance
            public float SpatialBlend => CheckDistance.EnableCheckDistance ? 1 : 0;
            public float DopplerLevel => CheckDistance.EnableCheckDistance ? CheckDistance.DopplerLevel : 1;
            public int Spread => CheckDistance.EnableCheckDistance ? CheckDistance.Spread : 0;
            public AudioRolloffMode RolloffMode => CheckDistance.EnableCheckDistance ? CheckDistance.RollofMode : AudioRolloffMode.Logarithmic;
            public float MinDistance => CheckDistance.EnableCheckDistance ? CheckDistance.MinDistance : 1;
            public float MaxDistance => CheckDistance.EnableCheckDistance ? CheckDistance.MaxDistance : 500;
        }

        public enum EAudioType
        {
            Sfx, UI, Music
        }

        [System.Serializable]
        public class CheckDistance_AudioPreset
        {
            public bool EnableCheckDistance;
            public float DopplerLevel;
            public int Spread;
            public AudioRolloffMode RollofMode;
            public float MinDistance;
            public float MaxDistance;
        }
    }
}