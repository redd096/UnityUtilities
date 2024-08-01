using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace redd096.ComponentsSystem
{
    /// <summary>
    /// Inherit from this component to create a variable for every input. Attach the component to your PlayerController. 
    /// Then your player will use probably a statemachine to know when read these inputs
    /// </summary>
    [AddComponentMenu("redd096/.ComponentsSystem/Player/Input Manager")]
    public class InputManager : MonoBehaviour
    {
        //inputs
        public Vector2 Movement;

        #region playerInput

#if ENABLE_INPUT_SYSTEM
        //player input variables
        public int playerIndex => _playerInput.playerIndex;

        PlayerInput _playerInput;
        public PlayerInput PlayerInput { get { if (_playerInput == null) _playerInput = GetComponent<PlayerInput>(); return _playerInput; } }

        /// <summary>
        /// Shortcut to call FindAction and ReadValue on PlayerInput
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public T ReadValue<T>(string action) where T : struct
        {
            return PlayerInput.actions.FindAction(action).ReadValue<T>();
        }
#endif

        #endregion

        protected virtual void Update()
        {
#if ENABLE_INPUT_SYSTEM
            //read inputs
            Movement = ReadValue<Vector2>("Move");
#endif
        }
    }
}