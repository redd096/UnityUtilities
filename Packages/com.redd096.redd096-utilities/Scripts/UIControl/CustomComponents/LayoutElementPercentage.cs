using UnityEngine;
using UnityEngine.UI;

namespace redd096.UIControl
{
    /// <summary>
    /// Add this component to a GameObject to make it into a layout element or override values on an existing layout element. 
    /// Every value will be used as a percentage from 0f to 1f
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Custom Components/Layout Element Percentage")]
    public class LayoutElementPercentage : LayoutElement
    {
        //parent rect transform
        [System.NonSerialized] private RectTransform m_parentRect;
        private RectTransform parentRectTransform
        {
            get
            {
                if (m_parentRect == null && transform.parent)
                    m_parentRect = transform.parent.GetComponent<RectTransform>();
                return m_parentRect;
            }
        }

        private float GetParentWidth()
        {
            Rect parentRect = parentRectTransform ? parentRectTransform.rect : Rect.zero;
            return parentRect.width;
        }

        private float GetParentHeight()
        {
            Rect parentRect = parentRectTransform ? parentRectTransform.rect : Rect.zero;
            return parentRect.height;
        }

        public override float minWidth { get => GetParentWidth() * base.minWidth; set => base.minWidth = value; }

        public override float minHeight { get => GetParentHeight() * base.minHeight; set => base.minHeight = value; }

        public override float preferredWidth { get => GetParentWidth() * base.preferredWidth; set => base.preferredWidth = value; }

        public override float preferredHeight { get => GetParentHeight() * base.preferredHeight; set => base.preferredHeight = value; }
    }
}