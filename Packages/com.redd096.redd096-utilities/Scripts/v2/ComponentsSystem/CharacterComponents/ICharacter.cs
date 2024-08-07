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
            if (Components == null)
                Components = SetComponents();

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
            if (Components == null)
                Components = SetComponents();

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

        ICharacterComponent[] SetComponents();
        void Awake();
        void Start();
        void Update();
        void FixedUpdate();

        virtual void AwakeFunction()
        {
            if (Components == null)
                Components = SetComponents();

            foreach (var component in Components)
            {
                component.Init(this);
            }
            foreach (var component in Components)
            {
                component.Awake();
            }
        }

        virtual void StartFunction()
        {
            if (Components == null)
                Components = SetComponents();

            foreach (var component in Components)
            {
                component.Start();
            }
        }

        virtual void UpdateFunction()
        {
            if (Components == null)
                Components = SetComponents();

            foreach (var component in Components)
            {
                component.Update();
            }
        }

        virtual void FixedUpdateFunction()
        {
            if (Components == null)
                Components = SetComponents();

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