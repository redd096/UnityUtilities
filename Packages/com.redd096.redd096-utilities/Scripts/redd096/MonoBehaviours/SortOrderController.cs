using redd096.Attributes;
using UnityEngine;

/// <summary>
/// Used to manage SortingLayer and OrderInLayer
/// </summary>
public class SortOrderController : MonoBehaviour
{
    //inspector
    [SerializeField] SortOrderData data;
    [Dropdown("GetNames")][SerializeField] string elementName;

    [Header("Canvas or Renderer - if both null, try get component in children")]
    [SerializeField] bool updateOnAwake = true;
    [SerializeField] Canvas canvas;
    [SerializeField] Renderer rend;

    //get from data
    private SortOrderData.Element element;
    private SortOrderData.Element GetElement() { if (data == null) Debug.LogError("Missing Data!"); return data ? data.GetElement(elementName) : null; }

    private void Awake()
    {
        if (updateOnAwake)
            UpdateSortOrder();
    }

    [Button]
    void UpdateSortOrder()
    {
        //get ref
        if (canvas == null && rend == null)
        {
            canvas = GetComponentInChildren<Canvas>();
            if (canvas == null) rend = GetComponentInChildren<Renderer>();
        }

        //update element
        element = GetElement();
        if (element == null)
        {
            return;
        }

        //update sort order
        if (canvas != null)
        {
            canvas.sortingLayerID = element.SortingLayer;
            canvas.sortingOrder = element.OrderInLayer;
        }
        if (rend != null)
        {
            rend.sortingLayerID = element.SortingLayer;
            rend.sortingOrder = element.OrderInLayer;
        }

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
