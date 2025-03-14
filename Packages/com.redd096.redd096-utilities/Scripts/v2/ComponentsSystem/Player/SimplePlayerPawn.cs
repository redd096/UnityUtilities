using UnityEngine;

namespace redd096.v2.ComponentsSystem
{ 
    /// <summary>
    /// This is used just to have a reference to the PlayerController
    /// </summary>
    public abstract class SimplePlayerPawn : MonoBehaviour
    {
        //controller
        private SimplePlayerController _currentController;
        public SimplePlayerController CurrentController { get => _currentController; set => _currentController = value; }

        /// <summary>
        /// Call Possess on controller, to possess this pawn
        /// </summary>
        /// <param name="playerController"></param>
        public void Possess(SimplePlayerController playerController)
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
                //if CurrentController for some reason doesn't have this setted as pawn, force unpossess on this pawn (copy-paste from controller unpossess)
                if (_currentController.CurrentPawn != this)
                {
                    SimplePlayerController previousController = _currentController;
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
        public virtual void OnPossess(SimplePlayerController newController)
        {

        }

        /// <summary>
        /// Called when a controller Unpossess this Pawn
        /// </summary>
        /// <param name="previousController"></param>
        public virtual void OnUnpossess(SimplePlayerController previousController)
        {

        }
    }
}