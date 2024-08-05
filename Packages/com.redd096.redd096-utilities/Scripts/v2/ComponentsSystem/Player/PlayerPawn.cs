using UnityEngine;

namespace redd096.v2.ComponentsSystem
{ 
    /// <summary>
    /// This is used just to have a reference to the PlayerController
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Player/Player Pawn")]
    public class PlayerPawn : MonoBehaviour
    {
        //controller
        private PlayerController _currentController;
        public PlayerController CurrentController { get => _currentController; set => _currentController = value; }

        /// <summary>
        /// Call Possess on controller, to possess this pawn
        /// </summary>
        /// <param name="playerController"></param>
        public void Possess(PlayerController playerController)
        {
            if (playerController)
                playerController.Possess(this);
        }

        /// <summary>
        /// Call Unpossess on current controller, to unpossess this pawn
        /// </summary>
        public void Unpossess()
        {
            if (_currentController)
            {
                //if for some reason CurrentController doesn't have this setted as pawn, force unpossess on this pawn
                if (_currentController.CurrentPawn != this)
                {
                    PlayerController previousController = _currentController;
                    _currentController = null;
                    OnUnpossess(previousController);
                    previousController.Unpossess(); //call anyway unpossess on PlayerController
                }
                //else call unpossess normally
                else
                {
                    _currentController.Unpossess();
                }
            }
        }

        /// <summary>
        /// Called when a controller Possess this Pawn
        /// </summary>
        /// <param name="newController"></param>
        public virtual void OnPossess(PlayerController newController)
        {

        }

        /// <summary>
        /// Called when a controller Unpossess this Pawn
        /// </summary>
        /// <param name="previousController"></param>
        public virtual void OnUnpossess(PlayerController previousController)
        {

        }
    }
}