using redd096.v1.Game3D;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using Input = redd096.InputNew;
#endif

namespace redd096.Examples.BasicPlayer
{
    /// <summary>
    /// Player controller is used to Possess a PlayerPawn in scene. I use this structure to help me in multiplayer games. 
    /// Normally instead of the input wrapper, I attach unity Player Input class with InputAction, so it manages also multiple controllers for local multiplayer
    /// </summary>
    [AddComponentMenu("redd096/Examples/BasicPlayer/Basic PlayerController")]
    public class BasicPlayerController : PlayerController
    {
        [SerializeField] BasicPlayerPawn playerPawn;

        public Vector2 Move { get; private set; }
        public Vector2 Rotate { get; private set; }
        public bool InteractPressedThisFrame { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            //possess pawn in scene
            playerPawn.Possess(this);
        }

        private void Update()
        {
            //calculate inputs. The pawn will use inputs when necessary
            Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Rotate = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            InteractPressedThisFrame = Input.GetButtonDown("Fire1");

            //we calculate every input here, so when we have to change inputs for some porting, or for example for PhotonFusion, we have to change only this script
            //myInputs.Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //myInputs.Rotate = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            //if (Input.GetButtonDown("Fire1"))
            //    myInputs.buttons.Set(MyButtons.InteractPressedThisFrame, true);
        }

        //then PhotonFusion has its method to send inputs online (only if Object.HasInputAuthority)
        //public void OnInput(NetworkRunner runner, NetworkInput input)
        //{
        //    input.Set(myInputs);
        //
        //    // Reset the input struct to start with a clean slate
        //    // when polling for the next tick
        //    myInputs = default;
        //}

        //and its method to receive inputs (only if Runner.IsServer)
        //public override void OnFixedUpdateNetworkTask()
        //{
        //    base.OnFixedUpdateNetworkTask();
        //
        //    if (GetInput(out NetworkInputData input))
        //    {
        //        Move = input.Move;
        //        Rotate = input.Rotate;
        //        InteractPressedThisFrame = input.buttons.IsSet(MyButtons.InteractPressedThisFrame);
        //        //probably for ButtonDown like InteractPressedThisFrame, we don't want to set immediatly it
        //        //but we can set a temp bool, then in Update we always set our input based on this temp bool and reset it.
        //        //so we are sure every script read our input down only for one frame
        //    }
        //}
    }
}