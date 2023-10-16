using UnityEngine;

namespace redd096
{
    /// <summary>
    /// This is the same as Singleton. The only difference is this auto instantiate the instance if it's not in scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DefaultExecutionOrder(-10)]
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
                    _instance = FindObjectOfType<T>();

                    //if not in scene, auto instantiate
                    if (_instance == null)
                        _instance = new GameObject(nameof(T) + " (AutoInstantiated)", typeof(T)).GetComponent<T>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        protected virtual bool isDontDestroyOnLoad => true;
        protected virtual bool automaticallyUnparentOnAwake => true;

        protected virtual void Awake()
        {
            //unparent
            if (automaticallyUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            //set instance and initialize it
            CheckInstance();

            if (_instance == this)
                InitializeSingleton();

            //call set defaults in the instance
            _instance.SetDefaults();
        }

        void CheckInstance()
        {
            if (_instance && _instance != this) //check also != this, if someone set instance with FindObjectOfType or instantiate it
            {
                //if there is already an instance, destroy this one
                Destroy(gameObject);
            }
            else
            {
                //else, set this as unique instance and set don't destroy on load
                _instance = (T)this;
                if (isDontDestroyOnLoad) DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Called one time in Awake, only if this is the correct instance. Called before SetDefaults
        /// </summary>
        protected virtual void InitializeSingleton()
        {

        }

        /// <summary>
        /// Called on Awake, after InitializeSingleton. 
        /// This will be called every time is loaded a new scene, if in the new scene there is another object of this type
        /// </summary>
        protected virtual void SetDefaults()
        {
            //things you must to call on every awake 
            //(every change of scene where there is another instance of this object)
        }
    }
}