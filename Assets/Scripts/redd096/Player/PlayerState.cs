namespace redd096
{
    using UnityEngine;

    public class PlayerState : State
    {
        protected Player player;
        protected Transform transform;
        protected Rigidbody rb;
        protected CameraBaseControl cameraControl;

        public override void AwakeState(StateMachine stateMachine)
        {
            base.AwakeState(stateMachine);
        
            //get references
            player = stateMachine as Player;
            transform = player.transform;
            rb = transform.GetComponent<Rigidbody>();
            cameraControl = player.cameraControl;
        }

        public override void Update()
        {
            base.Update();

            //camera update
            MoveCamera();
            Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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