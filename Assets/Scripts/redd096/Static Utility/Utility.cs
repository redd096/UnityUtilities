namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public static class Utility
    {
        #region general

        /// <summary>
        /// Set lockState, and visible only when not locked
        /// </summary>
        public static void LockMouse(CursorLockMode lockMode)
        {
            Cursor.lockState = lockMode;
            Cursor.visible = lockMode != CursorLockMode.Locked;
        }

        /// <summary>
        /// Remap a value min and max
        /// </summary>
        /// <param name="value">Value to remap</param>
        /// <param name="from_prev">Previous minimum value</param>
        /// <param name="to_prev">Previous maximum value</param>
        /// <param name="from_new">New minimum value</param>
        /// <param name="to_new">New max value</param>
        /// <returns></returns>
        public static float Remap(this float value, float from_prev, float to_prev, float from_new, float to_new)
        {
            return (value - from_prev) / (to_prev - from_prev) * (to_new - from_new) + from_new;
        }

        #endregion

        #region find nearest

        /// <summary>
        /// Find nearest to position
        /// </summary>
        public static T FindNearest<T>(this T[] collection, Vector3 position) where T : Object
        {
            T nearest = default;
            float distance = Mathf.Infinity;

            //foreach element in the collection
            foreach (T element in collection)
            {
                //only if there is element
                if (element == null)
                    continue;

                //check distance to find nearest
                float newDistance = Vector3.Distance(element.GetTransform().position, position);
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
        public static T FindNearest<T>(this List<T> collection, Vector3 position) where T : Object
        {
            T nearest = default;
            float distance = Mathf.Infinity;

            //foreach element in the collection
            foreach (T element in collection)
            {
                //only if there is element
                if (element == null)
                    continue;

                //check distance to find nearest
                float newDistance = Vector3.Distance(element.GetTransform().position, position);
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
        public static T FindNearest<K, T>(this Dictionary<K, T> collection, Vector3 position, out K key) where T : Object
        {
            K nearestKey = default;
            float distance = Mathf.Infinity;

            //foreach element in the collection
            foreach (K elementKey in collection.Keys)
            {
                //only if there is element
                if (collection[elementKey] == null)
                    continue;

                //check distance to find nearest
                float newDistance = Vector3.Distance(collection[elementKey].GetTransform().position, position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    nearestKey = elementKey;
                }
            }

            key = nearestKey;
            return collection[nearestKey];
        }

        #endregion
    }

    public static class Collections
    {
        #region create copy

        /// <summary>
        /// Create a copy of the collection
        /// </summary>
        public static T[] CreateCopy<T>(this T[] collection)
        {
            T[] newCollection = new T[collection.Length];

            //add every element in new collection
            for (int i = 0; i < collection.Length; i++)
            {
                newCollection[i] = collection[i];
            }

            return newCollection;
        }

        /// <summary>
        /// Create a copy of the collection
        /// </summary>
        public static List<T> CreateCopy<T>(this List<T> collection)
        {
            List<T> newCollection = new List<T>();

            //add every element in new collection
            foreach (T element in collection)
            {
                newCollection.Add(element);
            }

            return newCollection;
        }

        /// <summary>
        /// Create a copy of the collection
        /// </summary>
        public static Dictionary<K, T> CreateCopy<K, T>(this Dictionary<K, T> collection)
        {
            Dictionary<K, T> newCollection = new Dictionary<K, T>();

            //add every element in new collection
            foreach (K key in collection.Keys)
            {
                newCollection.Add(key, collection[key]);
            }

            return newCollection;
        }

        #endregion

        #region set parent

        /// <summary>
        /// Set parent for every element in the collection
        /// </summary>
        public static void SetParent<T>(this T[] collection, Transform parent, bool worldPositionStays = true) where T : Object
        {
            //set parent for every element in the collection
            foreach (T element in collection)
            {
                element.GetTransform().SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// Set parent for every element in the collection
        /// </summary>
        public static void SetParent<T>(this List<T> collection, Transform parent, bool worldPositionStays = true) where T : Object
        {
            //set parent for every element in the collection
            foreach (T element in collection)
            {
                element.GetTransform().SetParent(parent, worldPositionStays);
            }
        }

        /// <summary>
        /// Set parent for every element in the collection
        /// </summary>
        public static void SetParent<K, T>(this Dictionary<K, T> collection, Transform parent, bool worldPositionStays = true) where T : Object
        {
            //set parent for every element in the collection
            foreach (K key in collection.Keys)
            {
                collection[key].GetTransform().SetParent(parent, worldPositionStays);
            }
        }

        #endregion
    }

    public static class Extensions
    {
        #region object

        /// <summary>
        /// Return GameObject (cast obj as GameObject or Component)
        /// </summary>
        public static GameObject GetGameObject(this Object obj)
        {
            if (obj is GameObject)
                return obj as GameObject;
            else
                return (obj as Component).gameObject;
        }

        /// <summary>
        /// Return Transform (cast obj as GameObject or Component)
        /// </summary>
        public static Transform GetTransform(this Object obj)
        {
            if (obj is GameObject)
                return (obj as GameObject).transform;
            else
                return (obj as Component).transform;
        }

        #endregion

        #region vector2

        /// <summary>
        /// Sum return Vector2
        /// </summary>
        public static Vector2 SumVectors(this Vector2 first, Vector3 second)
        {
            return new Vector2(first.x + second.x, first.y + second.y);
        }

        /// <summary>
        /// Subtraction return Vector2
        /// </summary>
        public static Vector2 SubtractVectors(this Vector2 first, Vector3 second)
        {
            return new Vector2(first.x - second.x, first.y - second.y);
        }

        /// <summary>
        /// Multiplication return Vector2
        /// </summary>
        public static Vector2 MultiplyVectors(this Vector2 first, Vector3 second)
        {
            return new Vector2(first.x * second.x, first.y * second.y);
        }

        /// <summary>
        /// Division return Vector2
        /// </summary>
        public static Vector2 DivideVectors(this Vector2 first, Vector3 second)
        {
            return new Vector2(first.x / second.x, first.y / second.y);
        }

        #endregion

        #region vector3

        /// <summary>
        /// Sum return Vector3
        /// </summary>
        public static Vector3 SumVectors(this Vector3 first, Vector2 second)
        {
            return new Vector3(first.x + second.x, first.y + second.y, first.z);
        }

        /// <summary>
        /// Subtraction return Vector3
        /// </summary>
        public static Vector3 SubtractVectors(this Vector3 first, Vector2 second)
        {
            return new Vector3(first.x - second.x, first.y - second.y, first.z);
        }

        /// <summary>
        /// Multiplication return Vector3
        /// </summary>
        public static Vector3 MultiplyVectors(this Vector3 first, Vector2 second)
        {
            return new Vector3(first.x * second.x, first.y * second.y, first.z);
        }

        /// <summary>
        /// Division return Vector3
        /// </summary>
        public static Vector3 DivideVectors(this Vector3 first, Vector2 second)
        {
            return new Vector3(first.x / second.x, first.y / second.y, first.z);
        }

        #endregion

        #region image

        /// <summary>
        /// Fade an image - to use in a coroutine
        /// </summary>
        public static void Set_Fade(this Image image, float delta, float from, float to)
        {
            //set alpha from to
            float alpha = Mathf.Lerp(from, to, delta);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }

        /// <summary>
        /// Fade an image with fillAmount - to use in a coroutine
        /// </summary>
        public static void Set_FadeFill(this Image image, float delta, float from, float to)
        {
            //set fill amount
            image.fillAmount = Mathf.Lerp(from, to, delta);
        }

        #endregion
    }
}