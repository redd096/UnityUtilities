using redd096.Attributes;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Used to manage SortingLayer and OrderInLayer
    /// </summary>
    [AddComponentMenu("redd096/Main/MonoBehaviours/Sort Order Controller")]
    public class SortOrderController : MonoBehaviour
    {
        //inspector
        [SerializeField] SortOrderClass sortOrderClass;
        [SerializeField] FSortOrderDetails details;

        private void Awake()
        {
            if (details.updateOnAwake)
                UpdateSortOrder();
        }

        [Button]
        void UpdateSortOrder()
        {
            //get ref
            if (details.canvas == null && details.rend == null)
            {
                details.canvas = GetComponentInChildren<Canvas>();
                if (details.canvas == null) details.rend = GetComponentInChildren<Renderer>();
            }

            //refresh always element, ignore if _element is already setted. This is necessary if call this function with the button in inspector
            sortOrderClass.RefreshElement();
            if (sortOrderClass.IsValid() == false)
            {
                return;
            }

            //update sort order
            if (details.canvas != null)
            {
                details.canvas.sortingLayerID = sortOrderClass.SortingLayer;
                details.canvas.sortingOrder = sortOrderClass.SortOrder;
            }
            if (details.rend != null)
            {
                details.rend.sortingLayerID = sortOrderClass.SortingLayer;
                details.rend.sortingOrder = sortOrderClass.SortOrder;
            }

        }

        [System.Serializable]
        public class FSortOrderDetails
        {
            [Header("Canvas or Renderer - if both null, try get component in children")]
            public bool updateOnAwake;
            public Canvas canvas;
            public Renderer rend;

            //default updateOnAwake true
            public FSortOrderDetails() { updateOnAwake = true; canvas = null; rend = null; }
        }
    }
}