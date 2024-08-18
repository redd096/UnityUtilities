
namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Use this interface on every object that can receive damage (characters, breakable walls, etc...)
    /// </summary>
    public interface IDamageableExample
    {
        void ApplyDamage(FDamageInfoExample damageInfo);
    }
}