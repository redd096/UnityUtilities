using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PredictionSlider : MonoBehaviour
{
    [Header("Slider - if null use GetComponent")]
    [SerializeField] Slider slider;
    [SerializeField] bool autoRegisterToEvents = true;

    [Header("Predict")]
    [SerializeField] RectTransform predictionRect = default;
    [Tooltip("For how much time prediction will remain still")][SerializeField] float durationPrediction = 0.5f;
    [Tooltip("Duration animation from start value to end value")][SerializeField] float durationAnimation = 0.5f;
    [SerializeField] Color colorOnIncrease = Color.green;
    [SerializeField] Color colorOnDecrease = Color.red;

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
        if (slider.fillRect)
        {
            predictionRect.SetParent(slider.fillRect.parent);
            predictionRect.SetSiblingIndex(slider.fillRect.GetSiblingIndex()); //set this to fill index, and fill will be moved forward
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
        predictionImage.color = increase ? colorOnIncrease : colorOnDecrease;

        //if increase move prediction first (so reset slider value), else move slider normally
        if (increase)
        {
            slider.SetValueWithoutNotify(lastValue);
            UpdatePredictionVisual(valueToReach);
        }

        //wait duration
        yield return new WaitForSeconds(durationPrediction);

        //then start animation
        float delta = 0;
        float startValue = increase ? lastValue : predictionCurrentValue;
        while (delta < 1)
        {
            delta += Time.deltaTime / durationAnimation;

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
}
