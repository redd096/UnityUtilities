using redd096.Attributes;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Scriptable object used to set SortingLayer and OrderInLayer or SortOrder, in various Renderer and Canvas
    /// </summary>
    [CreateAssetMenu(fileName = "SortOrderData", menuName = "redd096/Datas/Sort Order Data")]
    public class SortOrderData : ScriptableObject
    {
        public Element[] Elements;

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

            Debug.LogError("Impossible to find: " + elementName);
            return null;
        }

        //element ----------------------------------------------------------------------------------------

        #region element classes

        [System.Serializable]
        public class Element
        {
            public string Name;
            [Tooltip("Used both on Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")][SortingLayer] public int SortingLayer;
            [Tooltip("Sort order in Canvas overlay, OrderInLayer in Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")] public int OrderInLayer;
        }

        #endregion
    }

    #region sort order class

    /// <summary>
    /// Class used to manage SortOrderData and SortOrderController
    /// </summary>
    [System.Serializable]
    public class SortOrderClass
    {
        //inspector
        [SerializeField] SortOrderData data;
        [Dropdown("GetNames")][SerializeField] string elementName;

        //get from data
        private SortOrderData.Element _element;
        private SortOrderData.Element GetElement(bool showErrors) { if (_element == null || string.IsNullOrEmpty(_element.Name)) _element = data.GetElement(elementName, showErrors); return _element; }
        public SortOrderData.Element Element => GetElement(true);
        public void RefreshElement() => _element = data.GetElement(elementName);    //force refresh also if _element is already != null

        //check if valid this class and element
        public bool IsValid() => this != null && GetElement(false) != null;
        public static implicit operator bool(SortOrderClass a) => a.IsValid();

        /// <summary>
        /// This constructor is used to create the class without set data and element name, in case you need it in code but you don't need to set it in inspector
        /// </summary>
        /// <param name="element"></param>
        public SortOrderClass(SortOrderData.Element element)
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