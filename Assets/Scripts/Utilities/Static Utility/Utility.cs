namespace redd096
{
    using UnityEngine;
    using UnityEngine.UI;

    public static class Utility
    {
        /// <summary>
        /// set lockState, and visible only when not locked
        /// </summary>
        public static void LockMouse(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = lockMode != CursorLockMode.Locked;
        }

        /// <summary>
        /// create a copy of the list
        /// </summary>
        public static List<T> CreateCopy<T>(this List<T> list)
        {
            List<T> newList = new List<T>();

            //add every element in new list
            foreach (T element in list)
            {
                newList.Add(element);
            }

            return newList;
        }

        /// <summary>
        /// create a copy of the array
        /// </summary>
        public static T[] CreateCopy<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];

            //add every element in new array
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }
    }

    public static class Fade
    {
        #region private API

        static void Fade(this Image image, ref float delta, float from, float to, float duration)
        {
            //speed based to duration
            delta += Time.deltaTime / duration;

            //set alpha from to
            float alpha = Mathf.Lerp(from, to, delta);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        static void Fade_Fill(this Image image, ref float delta, float from, float to, float duration)
        {
            //speed based to duration
            delta += Time.deltaTime / duration;

            //set fill amout
            image.fillAmount = Mathf.Lerp(from, to, delta);
        }

        #endregion

        /// <summary>
        /// Use inside a coroutine (delta 0 to 1), for fade in (alpha from 0 to 1)
        /// </summary>
        public static void FadeIn(this Image image, ref float delta, float duration)
        {
            image.Fade(ref delta, 0, 1, duration);
        }

        /// <summary>
        /// Use inside a coroutine (delta 0 to 1), for fade out (alpha from 1 to 0)
        /// </summary>
        public static void FadeOut(this Image image, ref float delta, float duration)
        {
            image.Fade(ref delta, 1, 0, duration);
        }

        /// <summary>
        /// Use inside a coroutine (delta 0 to 1), for fade in (alpha from 0 to 1) with fillAmount
        /// </summary>
        public static void FadeIn_Fill(this Image image, ref float delta, float duration)
        {
            image.Fade_Fill(ref delta, 0, 1, duration);
        }

        /// <summary>
        /// Use inside a coroutine (delta 0 to 1), for fade out (alpha from 1 to 0) with fillAmount
        /// </summary>
        public static void FadeOut_Fill(this Image image, ref float delta, float duration)
        {
            image.Fade_Fill(ref delta, 1, 0, duration);
        }
    }

    public static class TextLetterByLetter
    {
        /// <summary>
        /// Write a text letter by letter, then wait input. When press to skip, accelerate speed
        /// </summary>
        public static void WriteLetterByLetterAndWait(this Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilityMonoBehaviour.instance.WriteLetterByLetterAndWait(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter, then wait input. When press to skip, set immediatly all text
        /// </summary>
        public static void WriteLetterByLetterAndWait(this Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilityMonoBehaviour.instance.WriteLetterByLetterAndWait(textToSet, value, timeBetweenChar, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, accelerate speed
        /// </summary>
        public static void WriteLetterByLetter(this Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilityMonoBehaviour.instance.WriteLetterByLetter(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, set immediatly all text
        /// </summary>
        public static void WriteLetterByLetter(this Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilityMonoBehaviour.instance.WriteLetterByLetter(textToSet, value, timeBetweenChar, onEndWrite, canSkip);
        }
    }
}