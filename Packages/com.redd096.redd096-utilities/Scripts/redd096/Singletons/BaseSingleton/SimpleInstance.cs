using UnityEngine;

namespace redd096
{
    /// <summary>
    /// This to have the instance static variable, but can't be DontDestroyOnLoad and can have multiple instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleInstance<T> : MonoBehaviour where T : SimpleInstance<T>
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

        protected virtual bool onlyOneInstance => false;

        protected virtual void Awake()
        {
            //set instance and initialize it
            CheckInstance();

            //if onlyOneInstance is true call this only on the correct instance, else call it always
            if (onlyOneInstance == false || _instance == this)
                InitializeInstance();
        }

        void CheckInstance()
        {
            if (_instance && _instance != this && onlyOneInstance) //check also != this, if someone set instance with FindObjectOfType
            {
                //if there is already an instance, destroy this one (if must be only one instance)
                Destroy(gameObject);
            }
            else
            {
                //else, set this as instance
                _instance = (T)this;
            }
        }

        /// <summary>
        /// Called one time in Awake, only if can have multiple instances or this is the correct one
        /// </summary>
        protected virtual void InitializeInstance()
        {

        }
    }
}