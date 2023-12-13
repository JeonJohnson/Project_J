using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_DetailStatus_View : MonoBehaviour
{
    [Header("PLAYER STATUS BOARD")]
    [SerializeField] TextMeshProUGUI playerStatusText;

    [Header("ITEM BOARD")]
    [SerializeField] Slot[] ItemBoard_itemSlots;
    [SerializeField] TextMeshProUGUI ItemBoard_InformText;
    [SerializeField] GameObject ItemBoard_SelectFrame;

    [Header("ITEM INFO BOARD")]
    [SerializeField] Slot[] ItemInfoHolder_itemSlots;
    [SerializeField] TextMeshProUGUI ItemInfoHolder_SelectedItemText;
    [SerializeField] TextMeshProUGUI ItemInfoHolder_SelectedItemInfoText;

    public void UpdateItemBoardHolder(PlayerInventroy inventroy)
    {
        UpdateItemBoardInfo(ItemBoard_itemSlots[0]);
        UpdateItemInfoBoardHolder(ItemBoard_itemSlots[0].holdingItem);

        ItemBoard_itemSlots[0].UpdateSlot(inventroy.activeItemSlot);
        ItemBoard_itemSlots[0].button.onClick.RemoveAllListeners();
        ItemBoard_itemSlots[0].button.onClick.AddListener(() => UpdateItemBoardInfo(ItemBoard_itemSlots[0]));
        ItemBoard_itemSlots[0].button.onClick.AddListener(() => UpdateItemInfoBoardHolder(ItemBoard_itemSlots[0].holdingItem));

        int passiveItemSlotIndex = 1;
        for(int i = 1; i < ItemBoard_itemSlots.Length; i++)
        {
            int temp = i; // 클로저 문제때문에 하드코딩 https://mentum.tistory.com/343
            ItemBoard_itemSlots[passiveItemSlotIndex].UpdateSlot(inventroy.passiveItemSlot[i-1]);

            ItemBoard_itemSlots[passiveItemSlotIndex].button.onClick.RemoveAllListeners();
            ItemBoard_itemSlots[passiveItemSlotIndex].button.onClick.AddListener(() => UpdateItemBoardInfo(ItemBoard_itemSlots[temp]));
            ItemBoard_itemSlots[passiveItemSlotIndex].button.onClick.AddListener(() => UpdateItemInfoBoardHolder(ItemBoard_itemSlots[temp].holdingItem));

            passiveItemSlotIndex++;
        }
    }

    public void UpdateItemBoardInfo(Slot slot)
    {
        ItemBoard_SelectFrame.transform.position = slot.transform.position;
        if (slot == null || slot.holdingItem == null)
        {
            ItemBoard_InformText.text = "";
            return;
        }
        ItemBoard_InformText.text = slot.holdingItem.item_explain;
    }

    public void UpdateItemInfoBoardHolder(Item item)
    {
        ItemInfoHolder_itemSlots[0].UpdateSlot(item);

        string item_NameText;
        string item_ExplainText;

        if (item == null)
        {
            item_NameText = ""; item_ExplainText = "";
        }
        else
        {
            item_NameText = item.item_name; 
            item_ExplainText = item.item_explain;
        }

        ItemInfoHolder_SelectedItemText.text = item_NameText;
        ItemInfoHolder_SelectedItemInfoText.text = item_ExplainText;

        int passiveItemSlotIndex = 1;
        for (int i = 1; i < ItemBoard_itemSlots.Length; i++)
        {
            // 아이템 조합식 찾기

            // 아이템 조합식들 슬롯에 적용
            //ItemBoard_itemSlots[passiveItemSlotIndex].UpdateSlot(inventroy.passiveItemSlot[i - 1]);
            //passiveItemSlotIndex++;
        }
    }

    public void UpdatePlayerStatusHolder(Player player)
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
