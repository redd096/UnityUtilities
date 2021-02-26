using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : StateMachine
{
    [Header("Camera")]
    public CameraBaseControl cameraControl;

    [SerializeField] float duration = 10;

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