using UnityEngine;

namespace redd096.v1.Game3D
{
    /// <summary>
    /// Interface for every interactable
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// When OnInteract return true, interactor can't interact with other objects until it call OnDismiss. This is used to tell to interactor when call OnDismiss
        /// </summary>
        EDismissType DismissType => EDismissType.InteractAgain;

        /// <summary>
        /// Called when user interact with this object. Return true if user have to call OnDismiss before interact again with other objects
        /// </summary>
        /// <param name="interactor">The user that interact with this object</param>
        /// <param name="hit">The Raycast used to interact with</param>
        /// <param name="args">Optional parameters, for example the Camera transform for draggable objects</param>
        /// <returns></returns>
        bool OnInteract(InteractComponent interactor, RaycastHit hit, params object[] args);

        /// <summary>
        /// Called when user try dismiss from an object. Return true if now user can interact again with other objects
        /// </summary>
        /// <param name="interactor">The user that interact with this object</param>
        /// <param name="args">Optional parameters, for example the Camera transform for draggable objects</param>
        /// <returns></returns>
        bool OnDismiss(InteractComponent interactor, params object[] args);
    }
}