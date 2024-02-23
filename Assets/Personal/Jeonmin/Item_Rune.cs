using AYellowpaper.SerializedCollections;
using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Item_Rune : Item
{
    public UnityAction RuneAction;
    public string RuneEffect_Name;
    public RuneEffect effect;

    [SerializedDictionary("이펙트 변수 이름", "값")]
    public SerializedDictionary<string, int> RuneEffect_Value;


    public override bool Equip(Player player)
    {
        effect = player.runeEffectHandler.LoadRuneEffect(RuneEffect_Name, RuneEffect_Value);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        player.runeEffectHandler.RemoveRuneEffect(effect);
        return base.UnEquip(player);
    }
}