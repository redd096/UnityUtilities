namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// GameObject Pooling
    /// </summary>
    public class Pooling
    {
        #region variables

        bool canGrow = true;

        /// <summary>
        /// List of objects in the list
        /// </summary>
        public List<GameObject> PooledObjects = new List<GameObject>();

        #endregion

        /// <summary>
        /// Set if the list can grow when use Instantiate, or use only amount in the Init function
        /// </summary>
        public Pooling(bool canGrow = true)
        {
            this.canGrow = canGrow;
        }

        #region private API

        GameObject Spawn(GameObject prefab)
        {
            //instantiate and add to list
            GameObject obj = Object.Instantiate(prefab);
            PooledObjects.Add(obj);

            return obj;
        }

        #endregion

        /// <summary>
        /// Instantiate pooled amount and set inactive
        /// </summary>
        public void Init(GameObject prefab, int pooledAmount)
        {
            //spawn amount and deactive
            for (int i = 0; i < pooledAmount; i++)
            {
                GameObject obj = Spawn(prefab);

                obj.SetActive(false);
            }
        }

        #region cycle

        /// <summary>
        /// If not enough objects in the pool, instantiate necessary to reach the cycleAmount
        /// </summary>
        public void InitCycle(GameObject prefab, int cycleAmount)
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
            //only if there are really objects unused
            if (cycledAmount >= PooledObjects.Count)
                return;

            for (int i = 0; i < PooledObjects.Count - cycledAmount; i++)
            {
                GameObject obj = PooledObjects[i];

                //move to the end of the list
                PooledObjects.Remove(obj);
                PooledObjects.Add(obj);
            }
        }

        #endregion

        #region instantiate

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public GameObject Instantiate(GameObject prefab)
        {
            //get the first inactive and return
            foreach (GameObject obj in PooledObjects)
            {
                if (obj.activeInHierarchy == false)
                {
                    obj.SetActive(true);

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
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            //return obj but with position and rotation set
            GameObject obj = Instantiate(prefab);

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public GameObject Instantiate(GameObject prefab, Transform parent)
        {
            //return obj but with position and rotation set
            GameObject obj = Instantiate(prefab);

            obj.transform.SetParent(parent);

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public GameObject Instantiate(GameObject prefab, Transform parent, bool worldPositionStays)
        {
            //return obj but with position and rotation set
            GameObject obj = Instantiate(prefab);

            obj.transform.SetParent(parent, worldPositionStays);

            return obj;
        }

        #endregion

        /// <summary>
        /// Deactive every object in the list
        /// </summary>
        public void DeactiveAll()
        {
            for (int i = 0; i < PooledObjects.Count; i++)
            {
                PooledObjects[i].SetActive(false);
            }
        }

        /// <summary>
        /// Simple deactive function
        /// </summary>
        public static void Destroy(GameObject objToDestroy)
        {
            objToDestroy.SetActive(false);
        }
    }

    /// <summary>
    /// Component Pooling
    /// </summary>
    public class Pooling<T> where T : Component
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

                obj.gameObject.SetActive(false);
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
            //only if there are really objects unused
            if (cycledAmount >= PooledObjects.Count)
                return;

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
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab)
        {
            //get the first inactive and return
            foreach (T obj in PooledObjects)
            {
                if (obj.gameObject.activeInHierarchy == false)
                {
                    obj.gameObject.SetActive(true);

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
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Vector3 position, Quaternion rotation)
        {
            //return obj but with position and rotation set
            T obj = Instantiate(prefab);

            obj.transform.position = position;
            obj.transform.rotation = rotation;

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Transform parent)
        {
            //return obj but with position and rotation set
            T obj = Instantiate(prefab);

            obj.transform.SetParent(parent);

            return obj;
        }

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one. 
        /// Then set parent. 
        /// NB SetActive not works in the same frame, so if you are instantiating in a cycle consider to use InitCycle()
        /// </summary>
        public T Instantiate(T prefab, Transform parent, bool worldPositionStays)
        {
            //return obj but with position and rotation set
            T obj = Instantiate(prefab);

            obj.transform.SetParent(parent, worldPositionStays);

            return obj;
        }

        #endregion

        /// <summary>
        /// Deactive every object in the list
        /// </summary>
        public void DeactiveAll()
        {
            for (int i = 0; i < PooledObjects.Count; i++)
            {
                PooledObjects[i].gameObject.SetActive(false);
            }
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