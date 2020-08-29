namespace redd096
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/MonoBehaviours/Player")]
    public class Player : StateMachine
    {
        void Start()
        {
            //SetState(new PlayerState(this));  // create new one, with constructor
            //                                      // or
            //SetState(playerState);            // use serialized state (use Awake instead of constructor)
        }

        // Update is called once per frame
        void Update()
        {
            state?.Execution();
        }
    }
}