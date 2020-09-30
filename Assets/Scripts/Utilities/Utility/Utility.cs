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

        #region find nearest

        /// <summary>
        /// Find nearest to position
        /// </summary>
        public static T FindNearest<T>(this T[] array, Vector3 position) where T : Component
        {
            T nearest = default;
            float distance = Mathf.Infinity;

            //foreach element in the array
            foreach (T element in array)
            {
                //only if there is element
                if (element == null)
                    continue;

                //check distance to find nearest
                float newDistance = Vector3.Distance(element.transform.position, position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = element;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Find nearest to position
        /// </summary>
        public static GameObject FindNearest(this GameObject[] array, Vector3 position)
        {
            GameObject nearest = default;
            float distance = Mathf.Infinity;

            //foreach element in the array
            foreach (GameObject element in array)
            {
                //only if there is element
                if (element == null)
                    continue;

                //check distance to find nearest
                float newDistance = Vector3.Distance(element.transform.position, position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearest = element;
                }
            }

            return nearest;
        }

        #endregion
    }

    public static class Collections
    {
        #region create copy

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

        #endregion

        #region set parent

        /// <summary>
        /// set parent for every element in the array
        /// </summary>
        public static void SetParent<T>(this T[] array, Transform parent, bool worldPositionStays = true) where T : Component
        {
            foreach (T c in array)
            {
                c.transform.SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// set parent for every element in the array
        /// </summary>
        public static void SetParent(this GameObject[] array, Transform parent, bool worldPositionStays = true)
        {
            foreach (GameObject c in array)
            {
                c.transform.SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// set parent for every element in the list
        /// </summary>
        public static void SetParent<T>(this List<T> list, Transform parent, bool worldPositionStays = true) where T : Component
        {
            foreach (T c in list)
            {
                c.transform.SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// set parent for every element in the list
        /// </summary>
        public static void SetParent(this List<GameObject> list, Transform parent, bool worldPositionStays = true)
        {
            foreach (GameObject c in list)
            {
                c.transform.SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// set parent for every element in the dictionary
        /// </summary>
        public static void SetParent<T, J>(this Dictionary<T, J> dictionary, Transform parent, bool worldPositionStays = true) where J : Component
        {
            foreach (T key in dictionary.Keys)
            {
                dictionary[key].transform.SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// set parent for every element in the dictionary
        /// </summary>
        public static void SetParent<T>(this Dictionary<T, GameObject> dictionary, Transform parent, bool worldPositionStays = true)
        {
            foreach (T key in dictionary.Keys)
            {
                dictionary[key].transform.SetParent(parent, worldPositionStays);
            }
        }

        #endregion
    }

    public static class FadeImage
    {
        /// <summary>
        /// Fade an image
        /// </summary>
        public static void Fade(this Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            UtilitySingleton.instance.Fade(image, from, to, duration, onEndFade);
        }

        /// <summary>
        /// Fade an image with fillAmount
        /// </summary>
        public static void FadeFill(this Image image, float from, float to, float duration, System.Action onEndFade = null)
        {
            UtilitySingleton.instance.FadeFill(image, from, to, duration, onEndFade);
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
            UtilitySingleton.instance.WriteLetterByLetterAndWait(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter, then wait input. When press to skip, set immediatly all text
        /// </summary>
        public static void WriteLetterByLetterAndWait(this Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilitySingleton.instance.WriteLetterByLetterAndWait(textToSet, value, timeBetweenChar, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, accelerate speed
        /// </summary>
        public static void WriteLetterByLetter(this Text textToSet, string value, float timeBetweenChar, float skipSpeed, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilitySingleton.instance.WriteLetterByLetter(textToSet, value, timeBetweenChar, skipSpeed, onEndWrite, canSkip);
        }

        /// <summary>
        /// Write a text letter by letter. When press to skip, set immediatly all text
        /// </summary>
        public static void WriteLetterByLetter(this Text textToSet, string value, float timeBetweenChar, System.Action onEndWrite = null, bool canSkip = true)
        {
            UtilitySingleton.instance.WriteLetterByLetter(textToSet, value, timeBetweenChar, onEndWrite, canSkip);
        }
    }
}