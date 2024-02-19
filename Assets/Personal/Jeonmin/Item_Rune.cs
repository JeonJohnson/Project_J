using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Rune Item", menuName = "Scriptable Object/Items/New Rune Item")]
public class Item_Rune : Item
{
    public UnityAction RuneAction;
    public string RuneEffect_Name;
    public int RuneEffect_Value;

    public override bool Equip(Player player)
    {
        Debug.Log("¿¡Å¢");
        player.runeEffectHandler.LoadRuneEffect(RuneEffect_Name, RuneEffect_Value);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        Debug.Log("¾ð¿¡Å¢");
        player.runeEffectHandler.RemoveRuneEffect(RuneEffect_Name);
        return base.UnEquip(player);
    }
}
