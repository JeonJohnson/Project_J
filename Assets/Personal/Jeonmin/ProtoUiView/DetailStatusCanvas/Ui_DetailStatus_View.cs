using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_DetailStatus_View : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerStatusText;
    [SerializeField] Slot[] itemSlots;
    [SerializeField] TextMeshProUGUI selectedItemStatusCanvas;

    public void UpdateSlots(PlayerInventroy inventroy)
    {
        itemSlots[0].UpdateSlot(inventroy.activeItemSlot);

        int passiveItemSlotIndex = 1;
        for(int i = 1; i < itemSlots.Length; i++)
        {
                itemSlots[passiveItemSlotIndex].UpdateSlot(inventroy.passiveItemSlot[i-1]);
                passiveItemSlotIndex++;
        }
    }

    public void UpdatePlayerStatus(Player player)
    {
        string hpText = player.status.curHp.Value.ToString() + "/" + player.status.maxHp;
        string armorText = "";
        string moveSpeedText = player.status.walkSpeed.ToString() + player.inventroy.invenBonusStatus.bonus_Player_Speed.ToString();

        string expectDamageText = ((player.curWeapon.defaltStatus.damage + player.inventroy.invenBonusStatus.bonus_Weapon_Damage + player.bonusStatus.bonus_Weapon_Damage) *
            (player.curWeapon.defaltStatus.fireRate + player.bonusStatus.bonus_Weapon_FireRate + player.inventroy.invenBonusStatus.bonus_Weapon_FireRate) *
            (player.curWeapon.defaltStatus.bulletNumPerFire + player.inventroy.invenBonusStatus.bonus_Weapon_BulletNumPerFire + player.bonusStatus.bonus_Weapon_BulletNumPerFire)).ToString();

        string consumeRangeText = player.curWeapon.suctionStat.suctionRange.ToString();
        string consumeAngleText = player.curWeapon.suctionStat.suctionAngle.ToString();
        string bulletTypeText = player.curWeapon.upgradeData.bulletType.ToString();
        string bulletNumPerFireText = (player.curWeapon.defaltStatus.bulletNumPerFire + player.inventroy.invenBonusStatus.bonus_Weapon_BulletNumPerFire + player.bonusStatus.bonus_Weapon_BulletNumPerFire).ToString();
        string bulletSpreadText = player.curWeapon.defaltStatus.bulletSpread.ToString();
        string dpsText = player.curWeapon.defaltStatus.fireRate + player.bonusStatus.bonus_Weapon_FireRate+ player.inventroy.invenBonusStatus.bonus_Weapon_FireRate.ToString();

        playerStatusText.text = hpText + "\n" +armorText+"\n"+moveSpeedText+"\n"+expectDamageText+"\n"+consumeRangeText+"\n"+consumeAngleText+"\n"+bulletTypeText+"\n"+bulletNumPerFireText+"\n"+bulletSpreadText+"\n"+dpsText;
    }
}
