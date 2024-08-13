using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Create a variable for every input and attach the component to your PlayerController. 
    /// Then your player will use probably a statemachine to know when read these inputs
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example Input Manager")]
    public class ExampleInputManager : MonoBehaviour
    {
        //inputs
        public Vector2 Movement;
        //public bool InteractWasPressedThisFrame;

        #region playerInput

#if ENABLE_INPUT_SYSTEM
        //player input variables
        public int playerIndex => PlayerInput.playerIndex;

        PlayerInput _playerInput;
        public PlayerInput PlayerInput { get { if (_playerInput == null) _playerInput = GetComponent<PlayerInput>(); return _playerInput; } }

        /// <summary>
        /// Shortcut to call FindAction on PlayerInput
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public InputAction FindAction(string action)
        {
            return PlayerInput.actions.FindAction(action);
        }
#endif

        #endregion

        protected virtual void Update()
        {
#if ENABLE_INPUT_SYSTEM
            //read inputs
            Movement = FindAction("Move").ReadValue<Vector2>();
            //InteractWasPressedThisFrame = FindAction("Interact").WasPressedThisFrame();
#else
            Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //InteractWasPressedThisFrame = Input.GetButtonDown("Fire1");
#endif
        }
    }
}