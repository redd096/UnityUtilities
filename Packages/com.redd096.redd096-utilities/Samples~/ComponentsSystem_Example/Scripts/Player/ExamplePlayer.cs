using UnityEngine;
using redd096.v2.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This is a PlayerPawn with every component
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example Player")]
    public class ExamplePlayer : PlayerPawn, IObject
    {
        //declare every component, and add in SetComponents
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;
        [SerializeField] MovementComponentRigidbody movementComponent;
        [SerializeField] InteractComponentRadius interactComponent;

        public IObjectComponent[] Components { get; set; }

        public IObjectComponent[] SetComponents()
        {
            return new IObjectComponent[] { stateMachineComponent, movementComponent, interactComponent };
        }

        public void Awake()
        {
            GetComponent<IObject>().AwakeFunction();
        }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        GetComponent<IObject>().OnDrawGizmosSelectedFunction();
    }

#endif

        public void Start()
        {
            GetComponent<IObject>().StartFunction();
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