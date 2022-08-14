using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.OLD
{
    [AddComponentMenu("redd096/.OLD/Example Player/Player")]
    public class Player : StateMachine
    {
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
            SetState(new PlayerState());    // create new one, with constructor
                                            //SetState(playerState);        // use serialized state (use AwakeState instead of constructor)
        }
    }
}