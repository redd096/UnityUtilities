using UnityEngine;
using redd096.BasicStateMachine;

namespace redd096.Examples.BasicStateMachine
{
    [System.Serializable]
    [AddComponentMenu("redd096/Examples/BasicStateMachine/Example Player State")]
    public class ExamplePlayerState : State
    {
        [SerializeField] VarOrBlackboard<Transform> targetBlackboard = new VarOrBlackboard<Transform>("Target");
        [SerializeField] VarOrBlackboard<Transform> targetInspector = default;

        protected ExamplePlayer player;
        protected Rigidbody rb;
        protected CameraBaseControl cameraControl;

        protected override void OnInit()
        {
            base.OnInit();

            //get references
            player = GetStateMachine<ExamplePlayer>();
            rb = GetStateMachineComponent<Rigidbody>();
            cameraControl = player.cameraControl;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            //camera update
            MoveCamera();
            //Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            StateMachine.SetBlackboardElement(targetBlackboard.GetName(), transformState);
            Debug.Log(GetValue(targetBlackboard));
            Debug.Log(targetInspector);
        }

        #region private API

        void MoveCamera()
        {
            //move camera
            cameraControl.UpdateCameraPosition();
        }

        void Rotate(float inputX, float inputY)
        {
            //rotate player and camera
            cameraControl.UpdateRotation(inputX, inputY);
        }

        #endregion
    }
}