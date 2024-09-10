using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is instantiated for every player, even online. 
    /// Generally you want to calculate inputs inside InputManager attached to this same GameObject, then your pawn in scene will read those inputs
    /// </summary>
    public abstract class SimplePlayerController : MonoBehaviour
    {
        //pawn
        private SimplePlayerPawn _currentPawn;
        public SimplePlayerPawn CurrentPawn => _currentPawn;

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
        public virtual void Possess(SimplePlayerPawn pawn)
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