using TMPro;
using UnityEngine;

public class WaitingResponseChatMessage : MonoBehaviour
{
    [SerializeField] TMP_Text textComponent;
    [SerializeField] float delay = 0.2f;
    [Space]
    [TextArea]
    [SerializeField]
    string[] textAnimation = new string[] {
        "<color=#668FA2>.</color><color=#A8C0CA>.</color><color=#BBC3C5>.</color>",
        "<color=#BBC3C5>.</color><color=#668FA2>.</color><color=#A8C0CA>.</color>",
        "<color=#A8C0CA>.</color><color=#BBC3C5>.</color><color=#668FA2>.</color>" };

    float time;
    string defaultText;
    int animationIndex;

    private void Awake()
    {
        time = Time.time + delay;
        defaultText = textComponent.text;
    }

    private void Update()
    {
        DoAnimation();
    }

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

    void DoAnimation()
    {
        if (Time.time < time)
            return;

        time = Time.time + delay;

        //set text based on animation index
        animationIndex = (animationIndex + 1) % textAnimation.Length;
        textComponent.text = textAnimation[animationIndex];
    }
}
