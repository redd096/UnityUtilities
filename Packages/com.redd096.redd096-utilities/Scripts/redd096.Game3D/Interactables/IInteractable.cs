
using UnityEngine;

namespace redd096.Game3D
{
    /// <summary>
    /// Interface for every interactable
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Called when user interact with this object. Return true if user have to call OnDismiss before interact again with other objects
        /// </summary>
        /// <param name="interactor">The user that interact with this object</param>
        /// <param name="hitCollider">The collider user interact with</param>
        /// <param name="args">Optional parameters, for example the Camera transform for draggable objects</param>
        /// <returns></returns>
        bool OnInteract(InteractComponent interactor, Collider hitCollider, params object[] args);

        /// <summary>
        /// Called when user try dismiss from an object. Return true if now user can interact again with other objects
        /// </summary>
        /// <param name="interactor">The user that interact with this object</param>
        /// <param name="args">Optional parameters, for example the Camera transform for draggable objects</param>
        /// <returns></returns>
        bool OnDismiss(InteractComponent interactor, params object[] args);
    }
}