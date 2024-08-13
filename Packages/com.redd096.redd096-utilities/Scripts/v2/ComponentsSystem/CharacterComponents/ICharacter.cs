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

        /// <summary>
        /// If Components is null, it's called this function to get every Component and call Init on them. 
        /// This function is called on Awake, Gizmos, or GetCharacterComponent
        /// </summary>
        /// <returns></returns>
        ICharacterComponent[] SetComponents();

        private void InitializeComponentsIfNull()
        {
            if (Components == null)
                Components = SetComponents();

            foreach (var component in Components)
            {
                component.Init(this);
            }
        }

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
    }

    public interface ICharacter<T> : ICharacter
    {
        /// <summary>
        /// This is used to have a reference to the GameObject
        /// </summary>
        T Character { get; }
    }
}