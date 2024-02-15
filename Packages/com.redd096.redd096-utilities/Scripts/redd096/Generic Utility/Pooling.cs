using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    public class Pooling<T> where T : Object
    {
        #region variables

        /// <summary>
        /// Limit of objects in the list. When reach this limit it can't instantiate other objects. If you call to instantiate, it "instantiate" only if there is an object deactivated in the list. 
        /// If you prefer to deactivate the oldest object and instantiate a new one, you can call Pooling Destroy on the first element (0) of the list before call Instantiate
        /// </summary>
        public int Limit = -1;

        /// <summary>
        /// List of objects in the list
        /// </summary>
        public List<T> PooledObjects = new List<T>();

        #endregion

        /// <summary>
        /// Set list limit when Instantiate (-1 is no limit)
        /// </summary>
        public Pooling(int limit = -1)
        {
            Limit = limit;
        }

        /// <summary>
        /// Get element in the list at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return PooledObjects[index]; }
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
            //return GameObject (cast obj as GameObject or Component)
            if (obj is GameObject)
                return obj as GameObject;
            else
                return (obj as Component).gameObject;
        }

        Transform GetTransform(Object obj)
        {
            //return Transform (cast obj as GameObject or Component)
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

        /// <summary>
        /// If not enough objects in the pool, instantiate necessary to reach the cycleAmount. 
        /// This is necessary for cycles, because SetActive doesn't work in the same frame, 
        /// so if we exceed the amount of object in the list it will try to reactivate a precedent object instead of instantiate new one
        /// </summary>
        public void InitCycle(T prefab, int cycleAmount)
        {
            //add if there are not enough buttons in pool
            if (cycleAmount > PooledObjects.Count)
            {
                Init(prefab, cycleAmount - PooledObjects.Count);
            }
        }

        #region instantiate

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if not reached limit, instantiate new one. 
        /// NB SetActive doesn't work in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab)
        {
            //get the first inactive and return
            foreach (T obj in PooledObjects)
            {
                //if found obj null, remove from the list and go to next object
                if (obj == null)
                {
                    PooledObjects.Remove(obj);
                    return Instantiate(prefab);
                }

                if (GetGameObject(obj).activeSelf == false)
                {
                    //set active
                    GetGameObject(obj).SetActive(true);

                    //move to the end of the list
                    PooledObjects.Remove(obj);
                    PooledObjects.Add(obj);

                    return obj;
                }
            }

            //else if didn't reach limit, create new one and return it
            if (Limit < 0 || PooledObjects.Count < Limit)
            {
                return Spawn(prefab);
            }

            return null;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if not reached limit, instantiate new one. 
        /// Then set position and rotation. 
        /// NB SetActive doesn't work in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
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
        /// Active first inactive in the list. If everything is already active, if not reached limit, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive doesn't work in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Transform parent)
        {
            //return obj, then set parent
            T obj = Instantiate(prefab);

            if (obj != null)
            {
                //by default Unity Object.Instantiate(T, transform) set worldPositionsStays at false
                GetTransform(obj).SetParent(parent, worldPositionStays: false);
            }

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if not reached limit, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive doesn't work in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
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
        /// Clear the list (doesn't destroy objects. To destroy them call DestroyAll)
        /// </summary>
        public void Clear()
        {
            PooledObjects.Clear();
        }
    }

    public static class Pooling
    {
        /// <summary>
        /// Simple deactive function
        /// </summary>
        public static void Destroy(GameObject objToDestroy)
        {
            objToDestroy.SetActive(false);
        }
    }
}