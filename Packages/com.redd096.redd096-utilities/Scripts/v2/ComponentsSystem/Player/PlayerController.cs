using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Generally you want to calculate inputs inside InputManager attached to this same GameObject, because this is always instantiate for every player, even online. 
    /// Then your Pawn in scene will read the inputs. Or in a multiplayer online, your pawn on the server will read inputs and syncronize position, rotation, etc... with client
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Player/Player Controller")]
    public class PlayerController : MonoBehaviour
    {
        //pawn
        private PlayerPawn _currentPawn;
        public PlayerPawn CurrentPawn => _currentPawn;

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