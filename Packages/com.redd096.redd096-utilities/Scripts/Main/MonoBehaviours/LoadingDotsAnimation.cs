using TMPro;
using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Show a loading animation with dots (. .. ...)
    /// </summary>
    [AddComponentMenu("redd096/Main/MonoBehaviours/Loading Dots Animation")]
    public class LoadingDotsAnimation : MonoBehaviour
    {
        [SerializeField] TMP_Text textComponent;
        [SerializeField] float delay = 0.2f;

        private float time;

        //DoAnimation
        [Space]
        [TextArea]
        [SerializeField]
        string[] textAnimation = new string[] {
        "<color=#668FA2>.</color><color=#A8C0CA>.</color><color=#BBC3C5>.</color>",
        "<color=#BBC3C5>.</color><color=#668FA2>.</color><color=#A8C0CA>.</color>",
        "<color=#A8C0CA>.</color><color=#BBC3C5>.</color><color=#668FA2>.</color>" };
        private int animationIndex;

        //DoTransparency
        private string defaultText;

        private void Awake()
        {
            time = Time.time + delay;
            defaultText = textComponent.text;
        }

        private void Update()
        {
            DoAnimation();
        }

        /// <summary>
		/// Set text with textAnimation[index]
		/// </summary>
        void DoAnimation()
        {
            if (Time.time < time)
                return;

            time = Time.time + delay;

            //set text based on animation index
            animationIndex = (animationIndex + 1) % textAnimation.Length;
            textComponent.text = textAnimation[animationIndex];
        }

        /// <summary>
		/// Set text as you want and every few seconds remove transparency, then set again transparency
        /// e.g. Loading<color=#0000>.</color><color=#0000>.</color><color=#0000>.</color>
		/// </summary>
        void DoTransparency()
        {
            if (Time.time < time)
                return;

            time = Time.time + delay;

            if (textComponent.text.Contains("<color=#0000>"))
            {
                //remove transparency
                textComponent.text = textComponent.text.Remove(textComponent.text.IndexOf("<color=#0000>"), "<color=#0000>".Length);
                textComponent.text = textComponent.text.Remove(textComponent.text.IndexOf("</color>"), "</color>".Length);
            }
            else
            {
                //if every dot is visible, return transparency to every dot
                textComponent.text = defaultText;
            }
        }
    }
}