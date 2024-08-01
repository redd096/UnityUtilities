using UnityEngine;
using redd096.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This is a PlayerPawn with every CharacterComponent
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Example Player")]
    public class ExamplePlayer : PlayerPawn, ICharacter<ExamplePlayer>
    {
        //declare every component, and add in OnValidate
        [SerializeField] MovementComponent2D movementComponent;
        [SerializeField] InspectorStateMachineComponent stateMachineComponent;

        public ICharacterComponent[] Components { get; private set; }
        public ExamplePlayer Character => this;

        private void OnValidate()
        {
            Components = new ICharacterComponent[] { movementComponent, stateMachineComponent };
        }

        public void Awake()
        {
            GetComponent<ICharacter>().AwakeFunction();
        }

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