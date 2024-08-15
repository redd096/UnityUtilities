
namespace redd096.v2.ComponentsSystem
{
    public interface IObjectComponent
    {
        IObject Owner { get; set; }

        /// <summary>
        /// Set owner of this component
        /// </summary>
        /// <param name="owner"></param>
        virtual void Init(IObject owner)
        {
            Owner = owner;
        }

        virtual void OnDrawGizmosSelected() { }
        virtual void Awake() { }
        virtual void OnEnable() { }
        virtual void Start() { }
        virtual void Update() { }
        virtual void FixedUpdate() { }
        virtual void LateUpdate() { }
        virtual void OnDisable() { }
        virtual void OnDestroy() { }
    }
}