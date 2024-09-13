using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using DG.Tweening;

namespace redd096.UIControl
{
    [AddComponentMenu("redd096/UIControl/Switch Toggle Coroutine")]
    public class SwitchToggleCoroutine : MonoBehaviour
    {
        [Tooltip("Used only if button. Else use Toggle.isOn")][SerializeField] bool defaultValue;
        [SerializeField] RectTransform backgroundRectTransform;
        [SerializeField] RectTransform uiHandleRectTransform;
        [SerializeField] Color backgroundActiveColor;
        [SerializeField] Color handleActiveColor;
        [SerializeField] UnityEvent<bool> OnValueChanged;

        Toggle toggle;
        Button button;
        bool isCurrentlyOn;

        Vector2 handleDefaultPosition, handleActivePosition;
        Image backgroundImage, handleImage;
        Color backgroundDefaultColor, handleDefaultColor;

        void Awake()
        {
            //can you as toggle or button
            toggle = GetComponent<Toggle>();
            button = GetComponent<Button>();

            //save positions
            handleDefaultPosition = uiHandleRectTransform.anchoredPosition;
            handleActivePosition = new Vector2(backgroundRectTransform.rect.width - uiHandleRectTransform.rect.width - handleDefaultPosition.x, handleDefaultPosition.y);

            //get images
            backgroundImage = backgroundRectTransform.GetComponent<Image>();
            handleImage = uiHandleRectTransform.GetComponent<Image>();

            //save colors
            backgroundDefaultColor = backgroundImage.color;
            handleDefaultColor = handleImage.color;

            //add events
            if (toggle) toggle.onValueChanged.AddListener(OnSwitch);
            if (button) button.onClick.AddListener(ToggleSwitch);

            if (toggle && toggle.isOn)
                OnSwitch(true);
            else if (toggle == null && button && defaultValue)
                OnSwitch(true);
        }

        void OnDestroy()
        {
            if (toggle) toggle.onValueChanged.RemoveListener(OnSwitch);
            if (button) button.onClick.RemoveListener(ToggleSwitch);
        }

        public void OnSwitch(bool on)
        {
            isCurrentlyOn = on;
            OnValueChanged?.Invoke(isCurrentlyOn);

            //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;                           //no anim
            //uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);    //dotween
            StartCoroutine(DOAnchorPos(uiHandleRectTransform, on ? handleActivePosition : handleDefaultPosition, 0.4f));

            //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;                                  //no anim
            //backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);                            //dotween
            StartCoroutine(DOColor(backgroundImage, on ? backgroundActiveColor : backgroundDefaultColor, 0.6f));

            //handleImage.color = on ? handleActiveColor : handleDefaultColor;                                              //no anim
            //handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .4f);                                        //dotween
            StartCoroutine(DOColor(handleImage, on ? handleActiveColor : handleDefaultColor, 0.4f));
        }

        public void ToggleSwitch()
        {
            OnSwitch(!isCurrentlyOn);
        }

        #region coroutines

        private IEnumerator DOAnchorPos(RectTransform rect, Vector2 endPosition, float time)
        {
            float delta = 0;
            Vector2 startPosition = rect ? rect.anchoredPosition : default;
            while (delta < 1)
            {
                delta += Time.deltaTime / time;
                if (rect) rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, delta);
                yield return null;
            }
        }

        private IEnumerator DOColor(Image image, Color endColor, float time)
        {
            float delta = 0;
            Color startColor = image ? image.color : default;
            while (delta < 1)
            {
                delta += Time.deltaTime / time;
                if (image) image.color = Color.Lerp(startColor, endColor, delta);
                yield return null;
            }
        }

        #endregion
    }
}