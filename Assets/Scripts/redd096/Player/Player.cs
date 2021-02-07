namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Player")]
    public class Player : StateMachine
    {
        [Header("Camera")]
        public CameraBaseControl cameraControl;

        void Start()
        {
            //set default camera
            cameraControl.StartDefault(Camera.main.transform, transform);

            //set state
            SetState(new PlayerState());    // create new one, with constructor
            //SetState(playerState);        // use serialized state (use AwakeState instead of constructor)
        }
    }
}