using redd096.Attributes;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Use this instead of AudioClip in inspector, to get it from data and keep order in the project
    /// </summary>
    [System.Serializable]
    public class AudioClass
    {
        //inspector
        [SerializeField] AudioData data;
        [Dropdown(nameof(GetNames))][SerializeField] string elementName;

        private AudioData.Element element;

        /// <summary>
        /// Get element
        /// </summary>
        public AudioData.Element Element => GetElement(showErrors: true);

        //public
        public string Name => Element.Name;
        public float Volume => Element.Volume;
        public AudioData.PresetAudio Preset => Element.Preset;
        public AudioClip Clip => Element.RandomClip;

        #region public API

        /// <summary>
        /// This constructor is used to create the class without set data and element name, in case you need it in code but you don't need to set it in inspector
        /// </summary>
        /// <param name="element"></param>
        public AudioClass(AudioData.Element element)
        {
            this.element = element;
        }

        /// <summary>
        /// Check if this object exists and has a element
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return this != null && GetElement(showErrors: false) != null;
        }

        /// <summary>
        /// Force refresh also if Element is already != null. 
        /// NB if data is null, this return null
        /// </summary>
        public void RefreshElement()
        {
            element = data != null ? data.GetElement(elementName) : null;
        }

        #endregion

        #region private API

        private AudioData.Element GetElement(bool showErrors)
        {
            //update if element isn't already setted
            if (element == null || string.IsNullOrEmpty(element.Name))
            {
                //check data isn't null
                if (data != null)
                    element = data.GetElement(elementName, showErrors);
                else if (showErrors)
                    Debug.LogError($"Missing data on {GetType().Name}");
            }

            return element;
        }

        #endregion

        #region editor

        string[] GetNames()
        {
            if (data == null)
                return new string[0];

            string[] s = new string[data.Elements.Length];
            for (int i = 0; i < s.Length; i++)
                s[i] = data.Elements[i].Name;

            return s;
        }

        #endregion
    }
}