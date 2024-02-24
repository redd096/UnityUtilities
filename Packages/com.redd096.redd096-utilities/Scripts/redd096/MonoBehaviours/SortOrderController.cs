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
        [SerializeField] SortOrderClass sortOrderClass;

        [Header("Canvas or Renderer - if both null, try get component in children")]
        [SerializeField] bool updateOnAwake = true;
        [SerializeField] Canvas canvas;
        [SerializeField] Renderer rend;

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

            //refresh always element, ignore if _element is already setted. This is necessary if call this function with the button in inspector
            sortOrderClass.RefreshElement();
            if (sortOrderClass.IsValid() == false)
            {
                return;
            }

            //update sort order
            if (canvas != null)
            {
                canvas.sortingLayerID = sortOrderClass.Element.SortingLayer;
                canvas.sortingOrder = sortOrderClass.Element.OrderInLayer;
            }
            if (rend != null)
            {
                rend.sortingLayerID = sortOrderClass.Element.SortingLayer;
                rend.sortingOrder = sortOrderClass.Element.OrderInLayer;
            }

        }
    }
}