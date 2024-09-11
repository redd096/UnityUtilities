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

        [System.Serializable]
        public class Element
        {
            public string Name;
            [Tooltip("Used both on Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")][SortingLayer] public int SortingLayer;
            [Tooltip("Sort order in Canvas overlay, OrderInLayer in Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")] public int OrderInLayer;
        }
    }
}