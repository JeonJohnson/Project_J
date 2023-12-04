using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Item", menuName = "Scriptable Object/Items/New Passive Item")]
public class Item_Passive : Item
{
    public WeaponUpgradeData weaponUpgradeData;

    public override bool Equip(Player player)
    {
        ApplyUpgradeData(player);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }

    private void ApplyUpgradeData(Player player)
    {
        if(weaponUpgradeData.bulletType != BulletType.Non)
        {
            if(weaponUpgradeData.bulletType != player.curWeapon.upgradeData.bulletType)
            {
                // 팝업창 표시 변경
            }

            player.curWeapon.upgradeData.bulletType = weaponUpgradeData.bulletType;
        }

        if (weaponUpgradeData.bulletEffect != BulletEffect.Non)
        {
            if (weaponUpgradeData.bulletEffect != player.curWeapon.upgradeData.bulletEffect)
            {
                // 팝업창 표시 변경
            }

            player.curWeapon.upgradeData.bulletEffect = weaponUpgradeData.bulletEffect;
        }

        if (weaponUpgradeData.bulletSpreadType != BulletSpreadType.Non)
        {
            if (weaponUpgradeData.bulletSpreadType != player.curWeapon.upgradeData.bulletSpreadType)
            {
                // 팝업창 표시 변경
            }

            player.curWeapon.upgradeData.bulletSpreadType = weaponUpgradeData.bulletSpreadType;
        }

        if (weaponUpgradeData.fireTriggerType != FireTriggerType.Non)
        {
            if (weaponUpgradeData.fireTriggerType != player.curWeapon.upgradeData.fireTriggerType)
            {
                // 팝업창 표시 변경
            }

            player.curWeapon.upgradeData.fireTriggerType = weaponUpgradeData.fireTriggerType;
        }
    }
}
