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
        //declare every component, and add in SetComponents
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;
        [SerializeField] MovementComponentRigidbody movementComponent;
        [SerializeField] InteractComponent2D interactComponent;

        public ICharacterComponent[] Components { get; set; }

        public ICharacterComponent[] SetComponents()
        {
            return new ICharacterComponent[] { stateMachineComponent, movementComponent, interactComponent };
        }

        public void Awake()
        {
            GetComponent<ICharacter>().AwakeFunction();
        }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        GetComponent<ICharacter>().OnDrawGizmosSelected();
    }

#endif

        public void Start()
        {
            GetComponent<ICharacter>().StartFunction();
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