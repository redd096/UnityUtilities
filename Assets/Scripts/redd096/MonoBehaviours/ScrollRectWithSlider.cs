﻿namespace redd096
{
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("redd096/MonoBehaviours/Scroll Rect With Slider")]
    public class ScrollRectWithSlider : MonoBehaviour
    {
        [Header("Important")]
        [SerializeField] ScrollRect scrollRect = default;
        [SerializeField] Slider verticalSlider = default;
        [SerializeField] Slider horizontalSlider = default;

        private void Start()
        {
            //set default positions
            verticalSlider?.SetValueWithoutNotify(1 - scrollRect.verticalNormalizedPosition);
            horizontalSlider?.SetValueWithoutNotify(1 - scrollRect.horizontalNormalizedPosition);

            //set events
            scrollRect.onValueChanged.AddListener(OnScrollRectChanged);
            verticalSlider?.onValueChanged.AddListener(OnVerticalSliderChanged);
            horizontalSlider?.onValueChanged.AddListener(OnHorizontalSliderChanged);
        }

        void OnScrollRectChanged(Vector2 value)
        {
            //if(verticalSlider && horizontalSlider)
            //    Debug.Log($"scroll rect change: vertical from {verticalSlider.value} to {1 - scrollRect.verticalNormalizedPosition}, horizontal from {horizontalSlider.value} to {1 - scrollRect.horizontalNormalizedPosition}");
            //else if(verticalSlider)
            //    Debug.Log($"scroll rect change: vertical from {verticalSlider.value} to {1 - scrollRect.verticalNormalizedPosition}");
            //else if(horizontalSlider)
            //    Debug.Log($"scroll rect change: horizontal from {horizontalSlider.value} to {1 - scrollRect.horizontalNormalizedPosition}");

            //scrollRect go from 1 to 0, slider from 0 to 1
            verticalSlider?.SetValueWithoutNotify(1 - scrollRect.verticalNormalizedPosition);
            horizontalSlider?.SetValueWithoutNotify(1 - scrollRect.horizontalNormalizedPosition);
        }

        void OnVerticalSliderChanged(float value)
        {
            //Debug.Log($"vertical slider change: from {scrollRect.verticalNormalizedPosition} to {1 - verticalSlider.value}");

            //scrollRect go from 1 to 0, slider from 0 to 1
            scrollRect.verticalNormalizedPosition = 1 - verticalSlider.value;
        }

        void OnHorizontalSliderChanged(float value)
        {
            //Debug.Log($"horizontal slider change: from {scrollRect.horizontalNormalizedPosition} to {1 - horizontalSlider.value}");

            //scrollRect go from 1 to 0, slider from 0 to 1
            scrollRect.horizontalNormalizedPosition = 1 - horizontalSlider.value;
        }
    }
}