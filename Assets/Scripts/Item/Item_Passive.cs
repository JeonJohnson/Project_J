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
        ApplyUpgradeData(player);
        player.inventroy.AddItem(this);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }

    private void ApplyUpgradeData(Player player)
    {
        if(weaponData.bulletType != BulletType.Non)
        {
            if(weaponData.bulletType != player.inventroy.curWeaponSlot.weaponData.bulletType)
            {
                // �˾�â ǥ�� ����
            }

            player.inventroy.curWeaponSlot.weaponData.bulletType = weaponData.bulletType;
        }

        if (weaponData.bulletEffect != BulletEffect.Non)
        {
            if (weaponData.bulletEffect != player.inventroy.curWeaponSlot.weaponData.bulletEffect)
            {
                // �˾�â ǥ�� ����
            }

            player.inventroy.curWeaponSlot.weaponData.bulletEffect = weaponData.bulletEffect;
        }

        if (weaponData.bulletSpreadType != BulletSpreadType.Non)
        {
            if (weaponData.bulletSpreadType != player.inventroy.curWeaponSlot.weaponData.bulletSpreadType)
            {
                // �˾�â ǥ�� ����
            }

            player.inventroy.curWeaponSlot.weaponData.bulletSpreadType = weaponData.bulletSpreadType;
        }

        if (weaponData.fireTriggerType != FireTriggerType.Non)
        {
            if (weaponData.fireTriggerType != player.inventroy.curWeaponSlot.weaponData.fireTriggerType)
            {
                // �˾�â ǥ�� ����
            }

            player.inventroy.curWeaponSlot.weaponData.fireTriggerType = weaponData.fireTriggerType;
        }
    }
}
