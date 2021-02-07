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

        [Header("Fade In")]
        [Min(0)]
        [SerializeField] float waitBeforeStartFadeIn = 1;
        [Min(0)]
        [SerializeField] float timeToFadeIn = 1;

        [Header("Press to continue")]
        [SerializeField] bool pressToContinue = false;

        [Header("Fade Out")]
        [Min(0)]
        [SerializeField] float waitBeforeStartFadeOut = 2;
        [Min(0)]
        [SerializeField] float timeToFadeOut = 1;

        [Header("Load New Scene")]
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
                    delta += Time.deltaTime / timeToFadeIn;

                    float alpha = Mathf.Lerp(0, 1, delta);
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

                    yield return null;
                }

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
                    delta += Time.deltaTime / timeToFadeOut;

                    float alpha = Mathf.Lerp(1, 0, delta);
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

                    yield return null;
                }
            }

            //load new scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}