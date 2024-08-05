
namespace redd096.v1.GameTopDown2D
{
    public interface IInteractable
    {
        /// <summary>
        /// When someone interact with this object
        /// </summary>
        /// <param name="whoInteract"></param>
        void Interact(InteractComponent whoInteract);
    }
}