using UnityEngine;

//can use this for a Shop, where can sell Weapons and other things. Just add this interface to WeaponBASE
public interface ISellable
{
    string SellName { get; }
    int SellPrice { get; }
    Sprite SellSprite { get; }
}
