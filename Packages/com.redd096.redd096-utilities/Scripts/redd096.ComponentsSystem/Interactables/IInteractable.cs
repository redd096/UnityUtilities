
namespace redd096.ComponentsSystem
{
    /// <summary>
    /// This is the interface for every interactable. Use an InteractComponent to interact with it
    /// </summary>
    public interface IInteractable
    {
        void Interact(ICharacter interactor);
    }
}