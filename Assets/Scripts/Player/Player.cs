using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : StateMachine
{
    [Header("Camera")]
    public CameraBaseControl cameraControl;

    [SerializeField] float duration = 10;

    void Start()
    {
        //set default camera
        cameraControl.StartDefault(Camera.main.transform, transform);

        //set state
        SetState(new PlayerState());    // create new one, with constructor
        //SetState(playerState);        // use serialized state (use AwakeState instead of constructor)
        StartCoroutine(TestCoroutine());
    }

    IEnumerator TestCoroutine()
    {
        float delta = 0;
        float timerDelta = Time.time + duration;
        while(Time.time < 10)
        {
            float remainingTime = timerDelta - Time.time;
            float from1To0 = remainingTime / duration;
            float from0To1 = 1 - from1To0;

            delta += Time.deltaTime / duration;

            Debug.Log($"normale: {delta} - nuovo test: {from0To1} ---- Time: {Time.time}");
            yield return null;
        }
    }
}