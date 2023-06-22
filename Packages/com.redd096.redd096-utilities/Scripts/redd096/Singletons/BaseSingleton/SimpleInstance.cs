using UnityEngine;

namespace redd096
{
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
            if (onlyOneInstance == false || instance == this)
                InitializeInstance();
        }

        void CheckInstance()
        {
            if (instance && onlyOneInstance)
            {
                //if there is already an instance, destroy this one (if must be only one instance)
                Destroy(gameObject);
            }
            else
            {
                //else, set this as instance
                instance = (T)this;
            }
        }

        /// <summary>
        /// Called one time in Awake
        /// </summary>
        protected virtual void InitializeInstance()
        {

        }
    }
}