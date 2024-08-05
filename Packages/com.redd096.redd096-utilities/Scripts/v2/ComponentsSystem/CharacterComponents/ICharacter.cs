using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    public interface ICharacter
    {
        /// <summary>
        /// Every component on this character
        /// </summary>
        ICharacterComponent[] Components { get; }

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

        void Awake();
        void Start();
        void Update();
        void FixedUpdate();

        virtual void AwakeFunction()
        {
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
            foreach (var component in Components)
            {
                component.Start();
            }
        }

        virtual void UpdateFunction()
        {
            foreach (var component in Components)
            {
                component.Update();
            }
        }

        virtual void FixedUpdateFunction()
        {
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