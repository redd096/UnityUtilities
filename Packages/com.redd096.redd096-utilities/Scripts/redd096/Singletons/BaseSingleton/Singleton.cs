using UnityEngine;

namespace redd096
{
    /// <summary>
    /// Singleton with FindObjectOfType instance if null and automatically unparent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                //if null, try find it
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

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
                InitializeInstance();
        }

        void CheckInstance()
        {
            if (_instance && _instance != this) //check also != this, if someone set instance with FindObjectOfType
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
        /// Called one time in Awake, only if this is the correct instance
        /// </summary>
        protected virtual void InitializeInstance()
        {

        }
    }
}