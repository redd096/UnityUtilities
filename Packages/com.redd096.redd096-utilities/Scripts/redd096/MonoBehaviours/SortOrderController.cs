using redd096.Attributes;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Used to manage SortingLayer and OrderInLayer
    /// </summary>
    [AddComponentMenu("redd096/MonoBehaviours/Sort Order Controller")]
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
        private SortOrderData.Element _element;
        private SortOrderData.Element GetElement() { if (data == null) Debug.LogError("Missing Data!"); return data ? data.GetElement(elementName) : null; }
        public SortOrderData.Element Element { get { if (_element == null || string.IsNullOrEmpty(_element.Name)) _element = GetElement(); return _element; } }

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

            //refresh element
            _element = GetElement();
            if (Element == null)
            {
                return;
            }

            //update sort order
            if (canvas != null)
            {
                canvas.sortingLayerID = _element.SortingLayer;
                canvas.sortingOrder = _element.OrderInLayer;
            }
            if (rend != null)
            {
                rend.sortingLayerID = _element.SortingLayer;
                rend.sortingOrder = _element.OrderInLayer;
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
}