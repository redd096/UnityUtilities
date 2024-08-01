
namespace redd096.ComponentsSystem
{
    public interface ICharacterComponent
    {
        ICharacter Owner { get; set; }

        /// <summary>
        /// Set owner of this component
        /// </summary>
        /// <param name="owner"></param>
        virtual void Init(ICharacter owner)
        {
            Owner = owner;
        }

        virtual void OnDrawGizmosSelected() { }
        virtual void Awake() { }
        virtual void Update() { }
        virtual void FixedUpdate() { }
    }
}