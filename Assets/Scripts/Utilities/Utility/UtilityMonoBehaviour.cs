namespace redd096
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class UtilityMonoBehaviour : Singleton<UtilityMonoBehaviour>
    {
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

            //if wait or when player skip text
            if (wait)
            {
                //wait until press any key down
                while (!Input.anyKeyDown)
                {
                    yield return null;
                }
            }

            //call a function on end write
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

            //call a function on end write
            onEndWrite?.Invoke();
        }

        #endregion

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
    }
}