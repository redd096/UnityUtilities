using redd096.Attributes;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Scriptable object used to set SortingLayer and OrderInLayer or SortOrder, in various Renderer and Canvas
/// </summary>
[CreateAssetMenu(fileName = "SortOrderData", menuName = "redd096/Sort Order Data")]
public class SortOrderData : ScriptableObject
{
    public Element[] Elements;

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

    [System.Serializable]
    public class Element
    {
        public string Name;
        [Tooltip("Used both on Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")][Dropdown("GetSortingLayers")] public int SortingLayer;
        [Tooltip("Sort order in Canvas overlay, OrderInLayer in Renderer (SpriteRenderer, MeshRenderer...) and Canvas world")] public int OrderInLayer;

#if UNITY_EDITOR
        DropdownList<int> GetSortingLayers()
        {
            //get sorting layer IDs
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            int[] sortIDs = (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);

            //set dropdown
            DropdownList<int> list = new DropdownList<int>();
            for (int i = 0; i < sortIDs.Length; i++)
            {
                list.Add(UnityEngine.SortingLayer.IDToName(sortIDs[i]), sortIDs[i]);
            }
            return list;
        }
#endif
    }
}
