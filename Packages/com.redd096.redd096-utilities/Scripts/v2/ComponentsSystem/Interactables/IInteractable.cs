
namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the interface for every interactable. Use an InteractComponent to interact with it
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// When user interact with this
        /// </summary>
        /// <param name="interactor"></param>
        void Interact(IObject interactor);

        /// <summary>
        /// Can user interact with this object?
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns></returns>
        virtual bool CanInteract(IObject interactor)
        {
            return true;
        }
    }
}