
namespace redd096.v2.ComponentsSystem
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
        virtual void Start() { }
        virtual void Update() { }
        virtual void FixedUpdate() { }
        virtual void LateUpdate() { }
    }
}