namespace redd096
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    [AddComponentMenu("redd096/UI Control/Event Trigger redd096")]
    public class EventTriggerRedd096 : EventTrigger
    {
        /// <summary>
        /// Set UI Selected
        /// </summary>
        public void SelectThis()
        {
            //set selected this one
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        /// <summary>
        /// Set UI NOT Selected
        /// </summary>
        public void DeselectThis()
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            //if this one is selected, deselect
            if(gameObject == currentSelected)
                EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// Set transform.localScale
        /// </summary>
        public void SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        /// <summary>
        /// Set Color for Image UI
        /// </summary>
        public void SetImageColor(ColorValue colorValue)
        {
            GetComponent<Image>().color = colorValue.Color;
        }
    }
}