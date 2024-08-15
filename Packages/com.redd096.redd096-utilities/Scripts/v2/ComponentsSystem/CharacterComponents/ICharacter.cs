using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    public interface ICharacter
    {
        /// <summary>
        /// Every component on this character
        /// </summary>
        ICharacterComponent[] Components { get; set; }

        /// <summary>
        /// Get this character transform
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// If Components is null, it's called this function to get every Component and call Init on them. 
        /// This function is called on Awake, Gizmos, or GetCharacterComponent
        /// </summary>
        /// <returns></returns>
        ICharacterComponent[] SetComponents();

        /// <summary>
        /// If components is null, call SetComponents and initialize every component
        /// </summary>
        private void InitializeComponentsIfNull()
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
        virtual T GetCharacterComponent<T>() where T : ICharacterComponent
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
        virtual bool TryGetCharacterComponent<T>(out T foundComponent) where T : ICharacterComponent
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

        virtual void OnDrawGizmosSelected()
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

        virtual void UpdateFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.Update();
            }
        }

        virtual void FixedUpdateFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.FixedUpdate();
            }
        }

        virtual void LateUpdateFunction()
        {
            InitializeComponentsIfNull();

            foreach (var component in Components)
            {
                component.LateUpdate();
            }
        }

        #endregion
    }

    public interface ICharacter<T> : ICharacter
    {
        /// <summary>
        /// This is used to have a reference to the GameObject
        /// </summary>
        T Character { get; }
    }
}