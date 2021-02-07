﻿namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Pooling<T> where T : Object
    {
        #region variables

        bool canGrow = true;

        /// <summary>
        /// List of objects in the list
        /// </summary>
        public List<T> PooledObjects = new List<T>();

        #endregion

        /// <summary>
        /// Set if the list can grow when use Instantiate, or use only amount in the Init function
        /// </summary>
        public Pooling(bool canGrow = true)
        {
            this.canGrow = canGrow;
        }

        #region private API

        T Spawn(T prefab)
        {
            //instantiate and add to list
            T obj = Object.Instantiate(prefab);
            PooledObjects.Add(obj);

            return obj;
        }

        GameObject GetGameObject(Object obj)
        {
            //return obj as GameObject, or return gameObject variable of obj as Component
            if (obj is GameObject)
                return obj as GameObject;
            else
                return (obj as Component).gameObject;
        }

        Transform GetTransform(Object obj)
        {
            //return obj transform, casting to GameObject or Component
            if (obj is GameObject)
                return (obj as GameObject).transform;
            else
                return (obj as Component).transform;
        }

        #endregion

        /// <summary>
        /// Instantiate pooled amount and set inactive
        /// </summary>
        public void Init(T prefab, int pooledAmount)
        {
            //spawn amount and deactive
            for (int i = 0; i < pooledAmount; i++)
            {
                T obj = Spawn(prefab);

                GetGameObject(obj).SetActive(false);
            }
        }

        #region cycle

        /// <summary>
        /// If not enough objects in the pool, instantiate necessary to reach the cycleAmount
        /// </summary>
        public void InitCycle(T prefab, int cycleAmount)
        {
            //add if there are not enough buttons in pool
            if (cycleAmount > PooledObjects.Count)
            {
                Init(prefab, cycleAmount - PooledObjects.Count);
            }
        }

        /// <summary>
        /// Move to the end of the list every object unused in the cycle
        /// </summary>
        /// <param name="cycledAmount">The number of objects used in the cycle</param>
        public void EndCycle(int cycledAmount)
        {
            for (int i = 0; i < PooledObjects.Count - cycledAmount; i++)
            {
                T obj = PooledObjects[i];

                //move to the end of the list
                PooledObjects.Remove(obj);
                PooledObjects.Add(obj);
            }
        }

        #endregion

        #region instantiate

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// NB GameObject.SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab)
        {
            //get the first inactive and return
            foreach (T obj in PooledObjects)
            {
                if (obj == null)
                {
                    Debug.LogWarning("Pool object is destroyed");
                    continue;
                }

                if (GetGameObject(obj).activeInHierarchy == false)
                {
                    //set active
                    GetGameObject(obj).SetActive(true);

                    //move to the end of the list
                    PooledObjects.Remove(obj);
                    PooledObjects.Add(obj);

                    return obj;
                }
            }

            //else if can grow, create new one and return it
            if (canGrow)
            {
                return Spawn(prefab);
            }

            return null;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set position and rotation. 
        /// NB GameObject.SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Vector3 position, Quaternion rotation)
        {
            //return obj but with position and rotation set
            T obj = Instantiate(prefab);

            if (obj != null)
            {
                GetTransform(obj).position = position;
                GetTransform(obj).rotation = rotation;
            }

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB GameObject.SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Transform parent)
        {
            //return obj, then set parent
            T obj = Instantiate(prefab);

            if (obj != null)
            {
                GetTransform(obj).SetParent(parent);
            }

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB GameObject.SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Transform parent, bool worldPositionStays)
        {
            //return obj, then set parent
            T obj = Instantiate(prefab);

            if (obj != null)
            {
                GetTransform(obj).SetParent(parent, worldPositionStays);
            }

            return obj;
        }

        #endregion

        /// <summary>
        /// Deactive every object in the list
        /// </summary>
        public void DestroyAll()
        {
            for (int i = 0; i < PooledObjects.Count; i++)
            {
                GetGameObject(PooledObjects[i]).SetActive(false);
            }
        }

        /// <summary>
        /// Clear all the list
        /// </summary>
        public void Clear()
        {
            PooledObjects.Clear();
        }

        /// <summary>
        /// Simple deactive function
        /// </summary>
        public static void Destroy(GameObject objToDestroy)
        {
            objToDestroy.SetActive(false);
        }
    }
}