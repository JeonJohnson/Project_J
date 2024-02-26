using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;
using System.Linq;
using static Unity.Collections.Unicode;

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

    public Data<int> coinCount;

    private void Awake()
    {
        player = GetComponent<Player>();
        bulletCount = new Data<int>();
        ejectRemainBulletCount = new Data<int>();
        coinCount = new Data<int>();

        if (curWeaponItem == null) curWeaponItem = defWeaponItem;
        EquipWeapon(curWeaponItem);
    }

    public void EquipWeapon(Item_Weapon item_Weapon)
    {
        curWeaponItem = item_Weapon;
        player.curWeapon.weaponSprite.sprite = curWeaponItem.weaponSprite;
        ejectRemainBulletCount.Value = curWeaponItem.weaponData.magSize;

        UiController_Proto.Instance?.UpdateWeaponImage(curWeaponItem.item_sprite);
        SoundManager.Instance?.PlaySound("Player_WeaponSwap",Camera.main.gameObject,true);
    }

    public void UnEquipWeapon()
    {
        if (curWeaponItem == defWeaponItem) return;
        curWeaponItem = defWeaponItem;
        player.curWeapon.weaponSprite.sprite = curWeaponItem.weaponSprite;

        UiController_Proto.Instance?.UpdateWeaponImage(curWeaponItem.item_sprite);
        SoundManager.Instance.PlaySound("Player_WeaponSwap",Camera.main.gameObject, true);
    }

    public void AddRune(Item_Rune rune)
    {
        if (Funcs.IsArrayFull(runeList))
        {
            Debug.Log("ÀÎº¥ ²ËÂü. ¸®ÅÏ½ÃÅ´");
            return; 
        }

        Funcs.ArrayAdd(runeList, rune);
        //·é ½½·Ô UI ¾÷µ«
    }

    public void RemoveRune(Item_Rune rune)
    {
        Funcs.ArrayRemove(runeList, rune);
        Funcs.ArrayRemove(equipedRuneList, rune);
        //·é ½½·Ô UI ¾÷µ«
    }

    public void EquipRune(Item_Rune rune, int slot) 
    {
        if (equipedRuneList[slot])
        {
            int i = System.Array.IndexOf(runeList, rune);
            Funcs.ArrayReplace(runeList, equipedRuneList[slot], i);
        }
        Funcs.ArrayReplace(equipedRuneList, rune, slot);
        rune.Equip(player);
    }

    public void UnEquipRune(Item_Rune rune) 
    {
        Funcs.ArrayRemove(equipedRuneList, rune);
        Funcs.ArrayAdd(runeList, rune);
        runeBonusStatus -= rune.BonusStatus;
        rune.UnEquip(player);
    }

    public void ReplaceRune<T>(T[] array1, int index1, T[] array2, int index2) where T : Item_Rune
    {
        Funcs.ArraySwap(array1, index1, array2, index2);
        CalcRuneBonus();
    }

    public void EquipItem(Item item)
    {
  
    }

    public void CalcRuneBonus()
    {
        BonusStatus calcedBonusStatus = new BonusStatus();

        foreach (Item_Rune rune in equipedRuneList)
        {
            if(rune != null) calcedBonusStatus += rune.BonusStatus;
        }
        runeBonusStatus = calcedBonusStatus;
        Debug.Log(runeBonusStatus.weapon_BulletSize);
    }
}
