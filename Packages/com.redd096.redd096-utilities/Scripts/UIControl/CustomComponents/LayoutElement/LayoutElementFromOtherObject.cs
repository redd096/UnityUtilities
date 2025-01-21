using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace redd096.UIControl
{
    /// <summary>
    /// Add this component to a GameObject to make it into a layout element or override values on an existing layout element. 
    /// Every value will be used as a percentage from 0f to 1f
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Layout Element/Layout Element From Other Object")]
    public class LayoutElementFromOtherObject : UIBehaviour, ILayoutElement
    {
        [SerializeField] RectTransform otherObject;

        public float minWidth => LayoutUtility.GetMinWidth(otherObject);

        public float preferredWidth => LayoutUtility.GetPreferredWidth(otherObject);

        public float flexibleWidth => LayoutUtility.GetFlexibleWidth(otherObject);

        public float minHeight => LayoutUtility.GetMinHeight(otherObject);

        public float preferredHeight => LayoutUtility.GetPreferredHeight(otherObject);

        public float flexibleHeight => LayoutUtility.GetFlexibleHeight(otherObject);

        public int layoutPriority => 1;

        public virtual void CalculateLayoutInputHorizontal()
        {

        }

        public virtual void CalculateLayoutInputVertical()
        {

        }
    }
}