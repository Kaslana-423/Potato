using System;

public interface IPlayerWeaponLoadout
{
    int WeaponCount { get; }
    ShopWeaponDefinition GetWeapon(int index);
    event Action Changed;
}
