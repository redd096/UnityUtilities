namespace redd096
{
    using System.Collections.Generic;
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
        /// Find collider nearest to position
        /// </summary>
        public static Collider FindNearest(Collider[] colliders, Vector3 position)
        {
            Collider nearest = null;
            float distance = Mathf.Infinity;

            //foreach collider
            foreach (Collider col in colliders)
            {
                //check distance to find nearest
                float newDistance = Vector3.Distance(col.transform.position, position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = col;
                }
            }

            return nearest;
        }
    }

    public static class Extensions
    {
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
        /// create a copy of the dictionary (N.B. a copy of dictionary, not elements neither keys)
        /// </summary>
        public static Dictionary<T, J> CreateCopy<T, J>(this Dictionary<T, J> dictionary)
        {
            Dictionary<T, J> newDictionary = new Dictionary<T, J>();

            //add every element in new dictionary
            foreach (T key in dictionary.Keys)
            {
                newDictionary.Add(key, dictionary[key]);
            }

            return newDictionary;
        }
    }

    public static class FadeImage
    {
        /// <summary>
        /// Fade an image
        /// </summary>
        public static void Fade(this Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            UtilityMonoBehaviour.instance.Fade(image, from, to, duration, onEndFade);
        }

        /// <summary>
        /// Fade an image with fillAmount
        /// </summary>
        public static void FadeFill(this Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            UtilityMonoBehaviour.instance.FadeFill(image, from, to, duration, onEndFade);
        }

        /// <summary>
        /// Fade an image - to use in a coroutine
        /// </summary>
        public static void Set_Fade(this Image image, ref float delta, float from, float to, float duration)
        {
            //speed based to duration
            delta += Time.deltaTime / duration;

            //set alpha from to
            float alpha = Mathf.Lerp(from, to, delta);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        /// <summary>
        /// Fade an image with fillAmount - to use in a coroutine
        /// </summary>
        public static void Set_FadeFill(this Image image, ref float delta, float from, float to, float duration)
        {
            //speed based to duration
            delta += Time.deltaTime / duration;

            //set fill amout
            image.fillAmount = Mathf.Lerp(from, to, delta);
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