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
        List<GameObject> pooledObjects = new List<GameObject>();

        /// <summary>
        /// List of objects in the list
        /// </summary>
        public List<GameObject> PooledObjects { get { return pooledObjects; } }

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
            pooledObjects.Add(obj);

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

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one
        /// </summary>
        public GameObject Instantiate(GameObject prefab)
        {
            //get the first inactive and return
            foreach (GameObject obj in pooledObjects)
            {
                if (obj.activeInHierarchy == false)
                {
                    obj.SetActive(true);
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
        /// Then set position and rotation
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
        /// Deactive every object in the list
        /// </summary>
        public void DeactiveAll()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                pooledObjects[i].SetActive(false);
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
        List<T> pooledObjects = new List<T>();

        /// <summary>
        /// List of objects in the list
        /// </summary>
        public List<T> PooledObjects { get { return pooledObjects; } }

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
            pooledObjects.Add(obj);

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

        /// <summary>
        /// Active first inactive in the list. If everything is already active, if can grow, instantiate new one
        /// </summary>
        public T Instantiate(T prefab)
        {
            //get the first inactive and return
            foreach (T obj in pooledObjects)
            {
                if (obj.gameObject.activeInHierarchy == false)
                {
                    obj.gameObject.SetActive(true);
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
        /// Then set position and rotation
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
        /// Deactive every object in the list
        /// </summary>
        public void DeactiveAll()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                pooledObjects[i].gameObject.SetActive(false);
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