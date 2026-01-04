using System.Collections.Generic;
using UnityEngine;

namespace redd096.Singletons
{
    /// <summary>
    /// Instead of inerhiting from Singleton, call StaticSingleton.instance<T> to get any object as it has a static instance variable
    /// </summary>
    public class StaticSingleton : MonoBehaviour
    {
        private static Dictionary<System.Type, Component> instances = new Dictionary<System.Type, Component>();

        /// <summary>
        /// If this is the instance (or there aren't instances and this is set now as instance), return true (and can set DontDestroyOnLoad). 
        /// If there is already another instance, destroy this object if destroyCopies is true. 
        /// Normally this function is called in Awake() for every singleton script
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="setDontDestroyOnLoad">Set DontDeestroyOnLoad when set this element as Instance. Unparent automatically if this has parent</param>
        /// <param name="destroyCopies">If there is already an instance and it's not this one, destroy this object</param>
        /// <returns></returns>
        public static bool CheckInstance<T>(T obj, bool setDontDestroyOnLoad = true, bool destroyCopies = true) where T : Component
        {
            //get current instance or find in scene
            T currentInstance = instance<T>(false);

            //if this is the instance, return true
            if (currentInstance != null && currentInstance == obj)
            {
                //and set DontDestroyOnLoad
                if (setDontDestroyOnLoad)
                {
                    //and automatically unparent
                    if (obj.transform.parent)
                        obj.transform.SetParent(null);

                    DontDestroyOnLoad(obj);
                }

                return true;
            }
            //else, destroy and return false
            else
            {
                if (destroyCopies)
                    Destroy(obj);

                return false;
            }
        }
        
        /// <summary>
        /// Return instance for this type. If there isn't, find in scene or instantiate it. 
        /// e.g. StaticSingleton.instance<Player>(true) to look for Player in scene, else instantiate it 
        /// e.g. StaticSingleton.instance<Player>(false) to look for Player in scene, else return null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="autoInstantiate"></param>
        /// <returns></returns>
        public static T instance<T>(bool autoInstantiate = false) where T : Component
        {
            //if there is an instance and it's still valid, return it
            System.Type type = typeof(T);
            if (instances.ContainsKey(type))
            {
                if (instances[type] != null)
                    return instances[type] as T;
            }

            //if there isn't an instance ot it's not valid, try find in scene
            T obj_instance = FindFirstObjectByType<T>();            
            //or instantiate it
            if (obj_instance == null && autoInstantiate)
                obj_instance = new GameObject(type.Name + " (AutoInstantiated)", type).GetComponent<T>();

            //then register it
            if (obj_instance != null)
                instances[type] = obj_instance;

            return obj_instance;
        }
    }
}