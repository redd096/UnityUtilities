using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// An example of PlayerPawn and ICharacter to use in game
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example Player")]
    public class ExamplePlayer : PlayerPawn, IObject
    {
        //declare every component, and add in SetComponents
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;
        [SerializeField] MovementComponentRigidbody movementComponent;
        [SerializeField] InteractComponent2D interactComponent;

        public IObjectComponent[] Components { get; set; }

        public IObjectComponent[] SetComponents()
        {
            return new IObjectComponent[] { stateMachineComponent, movementComponent, interactComponent };
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            GetComponent<IObject>().OnDrawGizmosSelected();
        }

#endif

        void Awake()
        {
            GetComponent<IObject>().AwakeFunction();
        }

        void Start()
        {
            GetComponent<IObject>().StartFunction();
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