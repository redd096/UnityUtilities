using UnityEngine;
using redd096.StateMachine.BasicStateMachineRedd096;

namespace redd096.Examples.BasicStateMachine
{
    [AddComponentMenu("redd096/Examples/BasicStateMachine/Example Player")]
    public class ExamplePlayer : StateMachineRedd096
    {
        [Header("State Serialized")]
        [SerializeField] ExamplePlayerState playerState = new ExamplePlayerState();

        [Header("Camera")]
        public CameraBaseControl cameraControl;

        private void OnDrawGizmos()
        {
            cameraControl.OnDrawGizmos(Camera.main.transform, transform);
        }

        void Start()
        {
            //set default camera
            cameraControl.StartDefault(Camera.main.transform, transform);

            //set state
            SetState(playerState);                  // use serialized state
            //SetState(new ExamplePlayerState());   // or create new one, with constructor
        }
    }
}