using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Scriptable Object/Items/New Weapon Item")]
public class Item_Weapon : Item
{
    public WeaponData weaponData;

    public override bool Equip(Player player)
    {
        player.inventroy.AddItem(this);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }
}