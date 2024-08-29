using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the object with its components. Use this interface if you want to use the ComponentsSystem
    /// </summary>
    public interface IGameObjectRD
    {
        /// <summary>
        /// Every component on this object
        /// </summary>
        IComponentRD[] Components { get; set; }

        /// <summary>
        /// Get this object transform
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// If Components is null, it's called this function to get every Component and call Init on them. 
        /// This function is called by InitializeComponentsIfNull, on Awake, Gizmos, or GetComponentRD
        /// </summary>
        /// <returns></returns>
        IComponentRD[] SetComponents();

        /// <summary>
        /// If components is null, call SetComponents and initialize every component
        /// </summary>
        virtual void InitializeComponentsIfNull()
        {
            if (Components != null)
                return;

            Components = SetComponents();

            foreach (var component in Components)
            {
                if (component == null)
                {
                    Debug.LogError("Component is null, please update your SetComponents function. " +
                        "Maybe your component doesn't have [System.Serializable] or it isn't declared as a [SerializeField]. " +
                        "Or if you are creating it, be sure to call the constructor (new Component()) when you put it in your Components array");
                    continue;
                }
                component.InitRD(this);
            }
        }

        /// <summary>
        /// Get component from Components list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        virtual T GetComponentRD<T>()
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
        virtual bool TryGetComponentRD<T>(out T foundComponent)
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
                component.OnDrawGizmosSelectedRD();
            }
        }

        virtual void AwakeFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.AwakeRD();
            }
        }

        virtual void StartFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.StartRD();
            }
        }

        #endregion
    }
}