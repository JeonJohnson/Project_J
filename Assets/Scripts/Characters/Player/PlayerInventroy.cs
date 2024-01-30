using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;

//using static UnityEditor.Timeline.Actions.MenuPriority;

public class PlayerInventroy : MonoBehaviour
{
    private Player player;
    public Item_Weapon curWeaponItem;
    public Item_Weapon defWeaponItem;

    public Item curThrowItem;

    public Item_Rune[] equipedRuneList = new Item_Rune[6];
    public Item_Rune[] runeList = new Item_Rune[20];

    public Data<int> bulletCount;
    public Data<int> ejectRemainBulletCount;

    public BonusStatus runeBonusStatus;

    private void Awake()
    {
        player = GetComponent<Player>();
        bulletCount = new Data<int>();
        ejectRemainBulletCount = new Data<int>();
    }

    public void EquipWeapon(Item_Weapon item_Weapon)
    {
        curWeaponItem = item_Weapon;
        //player.curWeapon. ���־� ������Ʈ

        UiController_Proto.Instance?.UpdateWeaponImage(curWeaponItem.item_sprite);
        SoundManager.Instance.PlaySound("Player_WeaponSwap", gameObject);
    }

    public void AddRune(Item_Rune rune)
    {
        if (Funcs.IsArrayFull(runeList))
        {
            Debug.Log("�κ� ����. ���Ͻ�Ŵ");
            return; 
        }

        Funcs.ArrayAdd(runeList, rune);
        //�� ���� UI ����
    }

    public void RemoveRune(Item_Rune rune)
    {
        Funcs.ArrayRemove(runeList, rune);
        Funcs.ArrayRemove(equipedRuneList, rune);
        //�� ���� UI ����
    }

    public void EquipRune(Item_Rune rune, int slot) 
    {
        if (equipedRuneList[slot]) runeBonusStatus -= equipedRuneList[slot].BonusStatus;
        runeBonusStatus += rune.BonusStatus;

        Funcs.ArrayReplace(equipedRuneList, rune, slot);
    }

    public void UnEquipRune(Item_Rune rune) 
    {
        Funcs.ArrayRemove(equipedRuneList, rune);
        Funcs.ArrayAdd(runeList, rune);
        runeBonusStatus -= rune.BonusStatus;
    }
}
