using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace redd096.v1.GameTopDown2D
{
    /// <summary>
    /// Generally you want to calculate inputs inside PlayerController (or InputManager attached to this same GameObject), always instantiated for every player also online, 
    /// then there is the Pawn with its StateMachine that knows when to read the inputs from pawn's player controller
    /// </summary>
    [AddComponentMenu("redd096/v1/GameTopDown2D/Player/Player Controller")]
    public class PlayerController : MonoBehaviour
    {
        //pawn
        private PlayerPawn _currentPawn;
        public PlayerPawn CurrentPawn => _currentPawn;

#if ENABLE_INPUT_SYSTEM
        //player input variables
        public int playerIndex => _playerInput.playerIndex;

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

        protected virtual void Awake()
        {
            //be sure is unparent - set DontDestroyOnLoad
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Possess pawn - if already possessing a pawn, it will be unpossessed
        /// </summary>
        /// <param name="pawn"></param>
        public virtual void Possess(PlayerPawn pawn)
        {
            //if this controller has already a pawn, unpossess it
            Unpossess();

            if (pawn)
            {
                //if the new pawn ahs already a controller, call unpossess on it
                pawn.Unpossess();

                //then set pawn and controller 
                _currentPawn = pawn;
                _currentPawn.CurrentController = this;
                _currentPawn.OnPossess(this);
            }
        }

        /// <summary>
        /// Unpossess current pawn
        /// </summary>
        public virtual void Unpossess()
        {
            if (_currentPawn)
            {
                //remove pawn and controller
                _currentPawn.CurrentController = null;
                _currentPawn.OnUnpossess(this);
                _currentPawn = null;
            }
        }
    }
}