namespace redd096
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class UtilityMonoBehaviour : Singleton<UtilityMonoBehaviour>
    {
        #region Write Letter By Letter

        #region private API

        /// <summary>
        /// When press to skip, accelerate speed
        /// </summary>
        IEnumerator WriteLetterByLetter_Coroutine(Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite, bool canSkip, bool wait)
        {
            textToSet.text = string.Empty;

            //foreach char in value
            for (int i = 0; i < value.Length; i++)
            {
                //set current text
                textToSet.text += value[i];

                //wait before new char
                float startTime = Time.time;
                float waitTime = startTime + timeBetweenChar;

                while (waitTime > Time.time)
                {
                    //if skip, wait skipSpeed instead of timeBetweenChar
                    if (canSkip && Input.anyKey)
                    {
                        waitTime = startTime + skipSpeed;
                    }

                    yield return null;
                }

            }

            //if wait
            if (wait)
            {
                //wait until press any key down
                while (!Input.anyKeyDown)
                {
                    yield return null;
                }
            }

            //call a function on end
            onEndWrite?.Invoke();
        }

        /// <summary>
        /// When press to skip, set immediatly all text
        /// </summary>
        IEnumerator WriteLetterByLetter_Coroutine(Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite, bool canSkip, bool wait)
        {
            bool skipped = false;
            textToSet.text = string.Empty;

            //foreach char in value
            for (int i = 0; i < value.Length; i++)
            {
                //set current text
                textToSet.text += value[i];

                //wait before new char
                float waitTime = Time.time + timeBetweenChar;

                while (waitTime > Time.time)
                {
                    //if skip, set immediatly all text
                    if (canSkip && Input.anyKeyDown)
                    {
                        textToSet.text = value;
                        skipped = true;
                        i = value.Length;                       //end for cycle
                        yield return new WaitForEndOfFrame();   //end of frame, so can wait again Input.anyKeyDown
                        break;
                    }

                    yield return null;
                }
            }

            //if wait or when player skip text
            if (wait || skipped)
            {
                //wait until press any key down
                while (!Input.anyKeyDown)
                {
                    yield return null;
                }
            }

            //call a function on end
            onEndWrite?.Invoke();
        }

        #endregion

        #region public API

        /// <summary>
        /// Write a text letter by letter, then wait input. When press to skip, accelerate speed
        /// </summary>
        public void WriteLetterByLetterAndWait(Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite = null, bool canSkip = true)
        {
            StartCoroutine(WriteLetterByLetter_Coroutine(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip, true));
        }

        /// <summary>
        /// Write a text letter by letter, then wait input. When press to skip, set immediatly all text
        /// </summary>
        public void WriteLetterByLetterAndWait(Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            StartCoroutine(WriteLetterByLetter_Coroutine(textToSet, value, timeBetweenChar, onEndWrite, canSkip, true));
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, accelerate speed
        /// </summary>
        public void WriteLetterByLetter(Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite = null, bool canSkip = true)
        {
            StartCoroutine(WriteLetterByLetter_Coroutine(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip, false));
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, set immediatly all text
        /// </summary>
        public void WriteLetterByLetter(Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            StartCoroutine(WriteLetterByLetter_Coroutine(textToSet, value, timeBetweenChar, onEndWrite, canSkip, false));
        }

        #endregion

        #endregion




        #region Fade

        #region private API

        IEnumerator Fade_Coroutine(Image image, float from, float to, float duration, System.Action onEndFade, bool isNormalFade)
        {
            float delta = 0;

            //from 0 to 1
            while(delta < 1)
            {
                //fade or fade_fill
                if(isNormalFade)
                {
                    image.Set_Fade(ref delta, from, to, duration);
                }
                else
                {
                    image.Set_FadeFill(ref delta, from, to, duration);
                }

                yield return null;
            }

            //call a function on end
            onEndFade?.Invoke();
        }

        #endregion

        #region public API

        /// <summary>
        /// Fade an image
        /// </summary>
        public void Fade(Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            StartCoroutine(Fade_Coroutine(image, from, to, duration, onEndFade, true));
        }

        /// <summary>
        /// Fade an image with fillAmount
        /// </summary>
        public void FadeFill(Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            StartCoroutine(Fade_Coroutine(image, from, to, duration, onEndFade, false));
        }

        #endregion

        #endregion
    }
}