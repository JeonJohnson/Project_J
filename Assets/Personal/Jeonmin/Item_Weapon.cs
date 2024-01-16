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
        if (weaponData.bulletType != BulletType.Non)
        {
            if (weaponData.bulletType != player.curWeapon.weaponData.bulletType)
            {
                // �˾�â ǥ�� ����
            }

            player.curWeapon.weaponData.bulletType = weaponData.bulletType;
        }

        if (weaponData.bulletEffect != BulletEffect.Non)
        {
            if (weaponData.bulletEffect != player.curWeapon.weaponData.bulletEffect)
            {
                // �˾�â ǥ�� ����
            }

            player.curWeapon.weaponData.bulletEffect = weaponData.bulletEffect;
        }

        if (weaponData.bulletSpreadType != BulletSpreadType.Non)
        {
            if (weaponData.bulletSpreadType != player.curWeapon.weaponData.bulletSpreadType)
            {
                // �˾�â ǥ�� ����
            }

            player.curWeapon.weaponData.bulletSpreadType = weaponData.bulletSpreadType;
        }

        if (weaponData.fireTriggerType != FireTriggerType.Non)
        {
            if (weaponData.fireTriggerType != player.curWeapon.weaponData.fireTriggerType)
            {
                // �˾�â ǥ�� ����
            }

            player.curWeapon.weaponData.fireTriggerType = weaponData.fireTriggerType;
        }
    }
}