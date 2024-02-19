using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Item", menuName = "Scriptable Object/Items/New Passive Item")]
public class Item_Passive : Item
{
    public WeaponData weaponData;

    public override bool Equip(Player player)
    {
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }
}
