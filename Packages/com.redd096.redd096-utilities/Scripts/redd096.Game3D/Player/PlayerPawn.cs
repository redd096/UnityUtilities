using UnityEngine;

namespace redd096.Game3D
{
    [AddComponentMenu("redd096/.Game3D/Player/Player Pawn")]
    public class PlayerPawn : MonoBehaviour
    {
        //controller
        private PlayerController _currentController;
        public PlayerController CurrentController { get => _currentController; set => _currentController = value; }

        /// <summary>
        /// Make this pawn possessed by a controller
        /// </summary>
        /// <param name="playerController"></param>
        public void Possess(PlayerController playerController)
        {
            if (playerController)
                playerController.Possess(this);
        }

        /// <summary>
        /// Make this pawn unpossessed by controller
        /// </summary>
        public void Unpossess()
        {
            if (_currentController)
            {
                _currentController.Unpossess();
                _currentController = null;   //set null to be sure, if for some reason CurrentController doesn't have this setted as pawn
            }
        }
    }
}