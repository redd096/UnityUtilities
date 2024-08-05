using UnityEngine;
using redd096.v2.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This is a PlayerPawn with every CharacterComponent
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example Player")]
    public class ExamplePlayer : PlayerPawn, ICharacter
    {
        //declare every component, and add in OnValidate
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;
        [SerializeField] MovementComponent2D movementComponent;
        [SerializeField] InteractComponent2D interactComponent;

        public ICharacterComponent[] Components { get; private set; }

        public void Awake()
        {
            Components = new ICharacterComponent[] { stateMachineComponent, movementComponent, interactComponent };
            GetComponent<ICharacter>().AwakeFunction();
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            Awake();
            foreach (var component in Components)
            {
                component.OnDrawGizmosSelected();
            }
        }

#endif

        public void Start()
        {
            foreach (var component in Components)
            {
                component.Start();
            }
        }

        public void Update()
        {
            foreach (var component in Components)
            {
                component.Update();
            }
        }

        public void FixedUpdate()
        {
            foreach (var component in Components)
            {
                component.FixedUpdate();
            }
        }
    }
}