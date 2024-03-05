using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PredictionSlider : MonoBehaviour
{
    [Header("Slider - if null use GetComponent")]
    [SerializeField] Slider slider;
    [SerializeField] bool autoRegisterToEvents = true;

    [Header("Predict")]
    [SerializeField] Image predictionImage = default;
    [Tooltip("For how much time prediction will remain still")][SerializeField] float durationPrediction = 0.5f;
    [Tooltip("Duration animation from start value to end value")][SerializeField] float durationAnimation = 0.5f;
    [SerializeField] Color colorOnIncrease = Color.green;
    [SerializeField] Color colorOnDecrease = Color.red;

    float lastValue;
    Coroutine predictionCoroutine;

    private void Start()
    {
        //get references
        if (slider == null) slider = GetComponent<Slider>();

        //set prediction above fill in hierarchy (to render it behind)
        if (slider.fillRect && predictionImage)
        {
            predictionImage.transform.SetParent(slider.fillRect.parent);
            predictionImage.transform.SetSiblingIndex(slider.fillRect.GetSiblingIndex() - 1);

            //set prediction image
            predictionImage.type = Image.Type.Filled;
            predictionImage.fillMethod = slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft ? Image.FillMethod.Horizontal : Image.FillMethod.Vertical;
            predictionImage.fillOrigin = slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.BottomToTop ? 0 : 1;
        }

        //set default value
        lastValue = slider.value;

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
        if (isActiveAndEnabled && predictionImage)
        {
            if (predictionCoroutine != null)
                StopCoroutine(predictionCoroutine);

            predictionCoroutine = StartCoroutine(PredictionCoroutine(value));
        }
    }

    IEnumerator PredictionCoroutine(float valueToReach)
    {
        //check if increase or decrease and set prediction color
        bool increase = valueToReach >= predictionImage.fillAmount;
        predictionImage.color = increase ? colorOnIncrease : colorOnDecrease;

        //if increase move prediction first (so reset slider value), else move slider normally
        if (increase)
        {
            slider.SetValueWithoutNotify(lastValue);
            predictionImage.fillAmount = valueToReach;
        }

        //wait duration
        yield return new WaitForSeconds(durationPrediction);

        //then start animation
        float startValue = increase ? lastValue : predictionImage.fillAmount;
        float delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime / durationAnimation;

            if (increase)
                slider.SetValueWithoutNotify(Mathf.Lerp(startValue, valueToReach, delta));
            else
                predictionImage.fillAmount = Mathf.Lerp(startValue, valueToReach, delta);

            yield return null;
        }

        lastValue = valueToReach;
    }
}
