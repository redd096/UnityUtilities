using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.UIControl
{
    /// <summary>
    /// Add a fill Image, to show for example red or green when get or lose health
    /// </summary>
    [AddComponentMenu("redd096/UIControl/Prediction Slider")]
    public class PredictionSlider : MonoBehaviour
    {
        [Header("Slider - if null use GetComponent")]
        [SerializeField] Slider slider;
        [SerializeField] RectTransform predictionRect = default;
        [SerializeField] bool autoRegisterToEvents = true;

        [Header("Predict")]
        [SerializeField] PredictOptions predictOptions;

        float lastValue;
        Coroutine predictionCoroutine;
        Image predictionImage;
        float predictionCurrentValue;

        private void Start()
        {
            //get references
            if (slider == null) slider = GetComponent<Slider>();
            predictionImage = predictionRect.GetComponent<Image>();

            //set prediction above fill in hierarchy (to render it behind)
            if (predictOptions.autoUpdatePredictionSiblingPosition && slider.fillRect)
            {
                predictionRect.SetParent(slider.fillRect.parent);
                int siblingIndex = slider.fillRect.GetSiblingIndex();
                if (predictionRect.GetSiblingIndex() > siblingIndex)
                    predictionRect.SetSiblingIndex(siblingIndex);       //set this to fill index, and fill will be moved forward
                else
                    predictionRect.SetSiblingIndex(siblingIndex - 1);   //just stay behind fill
            }

            //set default value
            lastValue = slider.value;
            UpdatePredictionVisual(slider.value);

            //auto register to events
            if (autoRegisterToEvents && slider)
                slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            //auto unregister from events
            if (autoRegisterToEvents && slider)
                slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void OnValueChanged(float value)
        {
            //if prediction is enabled, start coroutine
            if (isActiveAndEnabled && predictionRect)
            {
                if (predictionCoroutine != null)
                    StopCoroutine(predictionCoroutine);

                predictionCoroutine = StartCoroutine(PredictionCoroutine(value));
            }
        }

        IEnumerator PredictionCoroutine(float valueToReach)
        {
            //check if increase or decrease and set prediction color
            bool increase = valueToReach >= predictionCurrentValue;
            predictionImage.color = increase ? predictOptions.colorOnIncrease : predictOptions.colorOnDecrease;

            //if increase move prediction first (so reset slider value), else move slider normally
            if (increase)
            {
                slider.SetValueWithoutNotify(lastValue);
                UpdatePredictionVisual(valueToReach);
            }

            //wait duration
            yield return new WaitForSeconds(predictOptions.durationPrediction);

            //then start animation
            float delta = 0;
            float startValue = increase ? lastValue : predictionCurrentValue;
            while (delta < 1)
            {
                delta += Time.deltaTime / predictOptions.durationAnimation;

                if (increase)
                    slider.SetValueWithoutNotify(Mathf.Lerp(startValue, valueToReach, delta));
                else
                    UpdatePredictionVisual(Mathf.Lerp(startValue, valueToReach, delta));

                yield return null;
            }

            lastValue = valueToReach;
        }

        /// <summary>
        /// Copy-paste from UpdateVisuals inside UnityEngine.UI.Slider
        /// </summary>
        /// <param name="value"></param>
        private void UpdatePredictionVisual(float value)
        {
            predictionCurrentValue = value;

            if (predictionRect != null)
            {
                float normalizedValue = value / slider.maxValue;

                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                int axis = (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft) ? 0 : 1;
                bool reverseValue = slider.direction == Slider.Direction.RightToLeft || slider.direction == Slider.Direction.TopToBottom;

                if (predictionImage != null && predictionImage.type == Image.Type.Filled)
                {
                    predictionImage.fillAmount = normalizedValue;
                }
                else
                {
                    if (reverseValue)
                        anchorMin[axis] = 1 - normalizedValue;
                    else
                        anchorMax[axis] = normalizedValue;
                }

                predictionRect.anchorMin = anchorMin;
                predictionRect.anchorMax = anchorMax;
            }
        }

        [System.Serializable]
        public class PredictOptions
        {
            public bool autoUpdatePredictionSiblingPosition = true;
            [Tooltip("For how much time prediction will remain still")] public float durationPrediction = 0.5f;
            [Tooltip("Duration animation from start value to end value")] public float durationAnimation = 0.5f;
            public Color colorOnIncrease = Color.green;
            public Color colorOnDecrease = Color.red;
        }
    }
}