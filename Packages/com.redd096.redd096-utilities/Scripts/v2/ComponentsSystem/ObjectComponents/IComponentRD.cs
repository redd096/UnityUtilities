
namespace redd096.v2.ComponentsSystem
{
    public interface IComponentRD
    {
        IGameObjectRD Owner { get; set; }

        /// <summary>
        /// Set owner of this component
        /// </summary>
        /// <param name="owner"></param>
        virtual void InitRD(IGameObjectRD owner)
        {
            Owner = owner;
        }

        virtual void OnDrawGizmosRD() { }
        virtual void OnDrawGizmosSelectedRD() { }

        virtual void AwakeRD() { }
        virtual void OnEnableRD() { }
        virtual void ResetRD() { }
        virtual void StartRD() { }

        virtual void FixedUpdateRD() { }
        virtual void UpdateRD() { }
        virtual void LateUpdateRD() { }

        virtual void OnDisableRD() { }
        virtual void OnDestroyRD() { }
    }
}