using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;
using UnityEngine.WSA;

public class PlayerInventroy : MonoBehaviour
{
    private Player player;
    public Item_Active activeItemSlot;
    public Item_Passive[] passiveItemSlot = new Item_Passive[6];
    public Item[] useableItemSlot = new Item[25];

    public BonusStatus invenBonusStatus;

    private float activeItem_CooldownTimer;
    private bool isWindowActivated = false;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && activeItemSlot != null)
        {
              activeItemSlot.Use(player);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isWindowActivated = !isWindowActivated;
            UiController_Proto.Instance.ShowDetailStatusWindow(isWindowActivated);
        }
    }

    public void AddItem(Item itemData)
    {
        switch (itemData.e_item_Type)
        {
            case Enums.Item_Type.Active:
                {
                    if (activeItemSlot != null) RemoveItemBonus(activeItemSlot.BonusStatus);
                    activeItemSlot = (Item_Active)itemData;
                    AddItemBonus(itemData.BonusStatus);
                    UiController_Proto.Instance.playerHudView.UpdateActiveItem(itemData.item_sprite);
                }
                break;
            case Enums.Item_Type.Passive:
                {
                    if(Funcs.IsArrayFull(passiveItemSlot)) { Debug.Log("! Inventory is Full"); }
                    else
                    {
                        Funcs.ArrayAdd(passiveItemSlot, itemData);
                        AddItemBonus(itemData.BonusStatus);

                        //ui 처리
                        UiController_Proto.Instance.playerHudView.UpdatePassiveItem(itemData.item_sprite, passiveItemSlot);
                    }

                }
                break;
            case Enums.Item_Type.Useable:
                {
                    if (Funcs.IsArrayFull(useableItemSlot)) { Debug.Log("! Inventory is Full"); }
                    else
                    {
                        Funcs.ArrayAdd(useableItemSlot, itemData);
                        AddItemBonus(itemData.BonusStatus);
                    }

                }
                break;
        }
    }

    private void RemoveItem(Item itemData)
    {
        RemoveItemBonus(itemData.BonusStatus);

        switch (itemData.e_item_Type)
        {
            case Enums.Item_Type.Active:
                {
                    activeItemSlot = null;
                }
                break;
            case Enums.Item_Type.Passive:
                {
                    Funcs.ArrayRemove(passiveItemSlot, itemData);
                }
                break;
            case Enums.Item_Type.Useable:
                {
                    Funcs.ArrayRemove(useableItemSlot, itemData);
                }
                break;
        }
    }

    public void ReplaceItem(Item itemData, int index)
    {
        switch (itemData.e_item_Type)
        {
            case Enums.Item_Type.Active:
                {
                    if (activeItemSlot != null) RemoveItemBonus(activeItemSlot.BonusStatus);

                    activeItemSlot = (Item_Active)itemData;
                    AddItemBonus(itemData.BonusStatus);
                    UiController_Proto.Instance.playerHudView.UpdateActiveItem(itemData.item_sprite);
                }
                break;
            case Enums.Item_Type.Passive:
                {
                    if (Funcs.IsArrayFull(passiveItemSlot)) { Debug.Log("! Inventory is Full"); }
                    else
                    {
                        if (passiveItemSlot[index] != null) RemoveItemBonus(passiveItemSlot[index].BonusStatus);
                        Funcs.ArrayReplace(passiveItemSlot, itemData, index);
                        AddItemBonus(itemData.BonusStatus);
                        UiController_Proto.Instance.playerHudView.UpdatePassiveItem(itemData.item_sprite, passiveItemSlot);
                    }

                }
                break;
            case Enums.Item_Type.Useable:
                {
                    if (Funcs.IsArrayFull(useableItemSlot)) { Debug.Log("! Inventory is Full"); }
                    else
                    {
                        if (useableItemSlot[index] != null) RemoveItemBonus(useableItemSlot[index].BonusStatus);
                        Funcs.ArrayReplace(useableItemSlot, itemData, index);
                        AddItemBonus(itemData.BonusStatus);
                    }

                }
                break;
        }
    }


    #region itemBonusCalcFuncs
    private void AddItemBonus(BonusStatus itemBonus)
    {
        this.invenBonusStatus.bonus_Player_Hp += itemBonus.bonus_Player_Hp;
        this.invenBonusStatus.bonus_Player_Armor += itemBonus.bonus_Player_Armor;
        this.invenBonusStatus.bonus_Player_Speed += itemBonus.bonus_Player_Speed;
        this.invenBonusStatus.bonus_Weapon_Speed += itemBonus.bonus_Weapon_Speed;
        this.invenBonusStatus.bonus_Weapon_Spread += itemBonus.bonus_Weapon_Spread;
        this.invenBonusStatus.bonus_Weapon_Damage += itemBonus.bonus_Weapon_Damage;
        this.invenBonusStatus.bonus_Weapon_FireRate += itemBonus.bonus_Weapon_FireRate;
        this.invenBonusStatus.bonus_Weapon_BulletNumPerFire += itemBonus.bonus_Weapon_BulletNumPerFire;
        this.invenBonusStatus.bonus_Weapon_Critial += itemBonus.bonus_Weapon_Critial;
        this.invenBonusStatus.bonus_Weapon_BulletSize += itemBonus.bonus_Weapon_BulletSize;
    }

    private void RemoveItemBonus(BonusStatus itemBonus)
    {
        this.invenBonusStatus.bonus_Player_Hp -= itemBonus.bonus_Player_Hp;
        this.invenBonusStatus.bonus_Player_Armor -= itemBonus.bonus_Player_Armor;
        this.invenBonusStatus.bonus_Player_Speed -= itemBonus.bonus_Player_Speed;
        this.invenBonusStatus.bonus_Weapon_Speed -= itemBonus.bonus_Weapon_Speed;
        this.invenBonusStatus.bonus_Weapon_Spread -= itemBonus.bonus_Weapon_Spread;
        this.invenBonusStatus.bonus_Weapon_Damage -= itemBonus.bonus_Weapon_Damage;
        this.invenBonusStatus.bonus_Weapon_FireRate -= itemBonus.bonus_Weapon_FireRate;
        this.invenBonusStatus.bonus_Weapon_BulletNumPerFire -= itemBonus.bonus_Weapon_BulletNumPerFire;
        this.invenBonusStatus.bonus_Weapon_Critial -= itemBonus.bonus_Weapon_Critial;
        this.invenBonusStatus.bonus_Weapon_BulletSize -= itemBonus.bonus_Weapon_BulletSize;
    }
    #endregion

    private void AddWeaponUpgradeData(WeaponUpgradeData weaponUpgradeData, Weapon_Player weapon)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            collision.gameObject.GetComponent<ItemPicker>().ShowInteractButton(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if(Input.GetKey(KeyCode.E))
            {
                Debug.Log("템 추가");
                ItemPicker itemPicker = collision.gameObject.GetComponent<ItemPicker>();
                //AddItem(itemPicker.itemData);
                collision.gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            collision.gameObject.GetComponent<ItemPicker>().ShowInteractButton(false);
        }
    }
}
