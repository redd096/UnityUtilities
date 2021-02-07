namespace redd096
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("redd096/MonoBehaviours/Splash Screen")]
    public class SplashScreen : MonoBehaviour
    {
        [Header("Image and Sprites")]
        [SerializeField] Image image = default;
        [SerializeField] Sprite[] spritesToUse = default;

        [Header("Fade In and Out")]
        [Min(0)]
        [SerializeField] float waitBeforeStartFadeIn = 1;
        [Min(0)]
        [SerializeField] float timeToFadeIn = 1;

        [SerializeField] bool pressToContinue = false;

        [Min(0)]
        [SerializeField] float waitBeforeStartFadeOut = 2;
        [Min(0)]
        [SerializeField] float timeToFadeOut = 1;
        [SerializeField] string nextSceneName = "Main Scene";

        void Start()
        {
            //if image is null, stop here
            if (image == null)
            {
                Debug.LogError("Missing Image UI");
                return;
            }

            //start splash screen
            StartCoroutine(FadeInAndOut());
        }

        IEnumerator FadeInAndOut()
        {
            //foreach sprite
            foreach (Sprite sprite in spritesToUse)
            {
                //set sprite
                image.sprite = sprite;

                //start alpha to 0
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

                //wait before start fade in
                yield return new WaitForSeconds(waitBeforeStartFadeIn);

                //fade in
                float delta = 0;
                while (delta < 1)
                {
                    image.Set_Fade(ref delta, 0, 1, timeToFadeIn);

                    yield return null;
                }

                //final alpha to 1
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

                //wait until press any key down
                if (pressToContinue)
                {
                    while (!Input.anyKeyDown)
                    {
                        yield return null;
                    }
                }

                //wait before start fade out
                yield return new WaitForSeconds(waitBeforeStartFadeOut);

                //fade out
                delta = 0;
                while (delta < 1)
                {
                    image.Set_Fade(ref delta, 1, 0, timeToFadeOut);

                    yield return null;
                }

                //final alpha to 0
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            //load new scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}