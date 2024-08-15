using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// An example of PlayerPawn and ICharacter to use in game
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example Player")]
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

        void Awake()
        {
            GetComponent<ICharacter>().AwakeFunction();
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            GetComponent<ICharacter>().OnDrawGizmosSelected();
        }

#endif

        void Start()
        {
            GetComponent<ICharacter>().StartFunction();
        }

        void Update()
        {
            foreach (var component in Components)
            {
                component.Update();
            }
        }

        void FixedUpdate()
        {
            foreach (var component in Components)
            {
                component.FixedUpdate();
            }
        }

        void LateUpdate()
        {
            foreach (var component in Components)
            {
                component.LateUpdate();
            }
        }
    }
}