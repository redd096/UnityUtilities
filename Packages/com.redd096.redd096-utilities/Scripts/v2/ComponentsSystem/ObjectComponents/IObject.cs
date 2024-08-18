using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the object with its components. Use this interface if you want to use the ComponentsSystem
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// Every component on this object
        /// </summary>
        IObjectComponent[] Components { get; set; }

        /// <summary>
        /// Get this object transform
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// If Components is null, it's called this function to get every Component and call Init on them. 
        /// This function is called on Awake, Gizmos, or GetCharacterComponent
        /// </summary>
        /// <returns></returns>
        IObjectComponent[] SetComponents();

        /// <summary>
        /// If components is null, call SetComponents and initialize every component
        /// </summary>
        virtual void InitializeComponentsIfNull()
        {
            if (Components == null)
                Components = SetComponents();

            foreach (var component in Components)
            {
                if (component == null)
                {
                    Debug.LogError("Component is null, please update your SetComponents function. " +
                        "Maybe your component doesn't have [System.Serializable] or it isn't declared as a [SerializeField]. " +
                        "Or if you are creating it, be sure to call the constructor (new Component()) when you put it in your Components array");
                }
                component.Init(this);
            }
        }

        /// <summary>
        /// Get component from Components list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        virtual T GetObjectComponent<T>()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                if (component is T)
                    return (T)component;
            }
            return default;
        }

        /// <summary>
        /// Try Get component from Components list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        virtual bool TryGetObjectComponent<T>(out T foundComponent)
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                if (component is T)
                {
                    foundComponent = (T)component;
                    return true;
                }
            }
            foundComponent = default;
            return false;
        }

        #region example functions

        virtual void OnDrawGizmosSelectedFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.OnDrawGizmosSelected();
            }
        }

        virtual void AwakeFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.Awake();
            }
        }

        virtual void StartFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.Start();
            }
        }

        #endregion
    }
}