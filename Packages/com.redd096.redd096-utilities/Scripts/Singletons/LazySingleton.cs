using UnityEngine;

namespace redd096
{
    /// <summary>
    /// This is the same as Singleton (literally copy-paste). 
    /// The only difference is this auto instantiate the instance if it's not in scene (instance get)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                //if null, try find it
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    //if not in scene, auto instantiate
                    if (_instance == null)
                        _instance = new GameObject(typeof(T).Name + " (AutoInstantiated)", typeof(T)).GetComponent<T>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        /// <summary>
        /// Set DontDeestroyOnLoad when set this element as Instance
        /// </summary>
        protected virtual bool isDontDestroyOnLoad => true;
        /// <summary>
        /// Unparent this element when set DontDestroyOnLoad (NB DontDestroyOnLoad works only when there aren't parents)
        /// </summary>
        protected virtual bool automaticallyUnparentOnSetDontDestroyOnLoad => true;
        /// <summary>
        /// If there is already an instance, but there are other objects with this class, destroy them
        /// </summary>
        protected virtual bool destroyCopies => true;

        protected virtual void Awake()
        {
            //set instance and initialize it
            CheckInstance();

            if (_instance == this)
                InitializeInstance();
        }

        protected void CheckInstance()
        {
            if (_instance && _instance != this && destroyCopies) //check also != this, if someone set instance with FindFirstObjectByType
            {
                //if there is already an instance, destroy this one (if destroyCopies is true)
                Destroy(gameObject);
            }
            else
            {
                //else, set this as unique instance and set don't destroy on load
                _instance = (T)this;
                if (isDontDestroyOnLoad)
                {
                    //unparent
                    if (automaticallyUnparentOnSetDontDestroyOnLoad)
                        transform.SetParent(null);
                    
                    DontDestroyOnLoad(this);
                }
            }
        }

        /// <summary>
        /// Called one time in Awake, only if this is the correct instance
        /// </summary>
        protected virtual void InitializeInstance()
        {

        }
    }
}