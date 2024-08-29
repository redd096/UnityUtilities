using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// An example of PlayerPawn and ICharacter to use in game
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example Player")]
    public class ExamplePlayer : PlayerPawn, IGameObjectRD
    {
        //declare every component, and add in SetComponents
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;
        [SerializeField] MovementComponentRigidbody movementComponent;
        [SerializeField] InteractComponentRadius interactComponent;

        public IComponentRD[] Components { get; set; }

        public IComponentRD[] SetComponents()
        {
            return new IComponentRD[] { stateMachineComponent, movementComponent, interactComponent };
        }

#if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            GetComponent<IGameObjectRD>().OnDrawGizmosSelectedFunction();
        }

#endif

        void Awake()
        {
            GetComponent<IGameObjectRD>().AwakeFunction();
        }

        void Start()
        {
            GetComponent<IGameObjectRD>().StartFunction();
        }

        void Update()
        {
            foreach (var component in Components)
            {
                component.UpdateRD();
            }
        }

        void FixedUpdate()
        {
            foreach (var component in Components)
            {
                component.FixedUpdateRD();
            }
        }

        void LateUpdate()
        {
            foreach (var component in Components)
            {
                component.LateUpdateRD();
            }
        }
    }
}