using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Rune Item", menuName = "Scriptable Object/Items/New Rune Item")]
public class Item_Rune : Item
{
    public UnityAction RuneAction;
    public RuneEffect RuneEffect;

    public override bool Equip(Player player)
    {
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }
}
