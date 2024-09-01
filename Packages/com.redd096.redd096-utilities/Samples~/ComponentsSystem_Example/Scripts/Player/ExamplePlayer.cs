using UnityEngine;
using redd096.v2.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This is a PlayerPawn with every component
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example Player")]
    public class ExamplePlayer : SimplePlayerPawn, IGameObjectRD
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

        public void Awake()
        {
            GetComponent<IGameObjectRD>().AwakeFunction();
        }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        GetComponent<IGameObjectRD>().OnDrawGizmosSelectedFunction();
    }

#endif

        public void Start()
        {
            GetComponent<IGameObjectRD>().StartFunction();
        }

        public void Update()
        {
            foreach (var component in Components)
            {
                component.UpdateRD();
            }
        }

        public void FixedUpdate()
        {
            foreach (var component in Components)
            {
                component.FixedUpdateRD();
            }
        }
    }
}