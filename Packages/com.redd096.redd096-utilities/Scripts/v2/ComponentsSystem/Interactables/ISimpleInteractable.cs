
namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the interface for every interactable. Use an InteractComponent to interact with it
    /// </summary>
    public interface ISimpleInteractable
    {
        /// <summary>
        /// When user interact with this
        /// </summary>
        /// <param name="interactor"></param>
        void Interact(IGameObjectRD interactor);

        /// <summary>
        /// Can user interact with this object?
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns></returns>
        virtual bool CanInteract(IGameObjectRD interactor)
        {
            return true;
        }
    }
}