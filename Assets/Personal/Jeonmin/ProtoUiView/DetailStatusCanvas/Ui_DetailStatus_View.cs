using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_DetailStatus_View : MonoBehaviour
{
    public enum MenuList
    {
        Status,
        Inventory,
        CombineList,
        Option,
        Exit
    }

    [Header("Menu Functions")]
    public Button[] Menu_Buttons; //0 내정보, 1 인벤토리, 2 조합식, 3 옵션, 4 메인메뉴
    public MenuList curMenu;

    [Header("PLAYER STATUS BOARD")]
    [SerializeField] GameObject statusBoardHolder;
    [SerializeField] TextMeshProUGUI playerStatusText;

    [Header("ITEM BOARD")]
    [SerializeField] GameObject ItemBoardHolder;
    [SerializeField] Slot[] ItemBoard_itemSlots;
    [SerializeField] TextMeshProUGUI ItemBoard_InformText;
    [SerializeField] GameObject ItemBoard_SelectFrame;

    [Header("ITEM INFO BOARD")]
    [SerializeField] GameObject ItemInfoBoardHolder;
    [SerializeField] Slot[] ItemInfoHolder_itemSlots;
    [SerializeField] TextMeshProUGUI ItemInfoHolder_SelectedItemText;
    [SerializeField] TextMeshProUGUI ItemInfoHolder_SelectedItemInfoText;

    [Header("COMBINELIST BOARD")]
    [SerializeField] GameObject CombinelistBoardHolder;

    [Header("OPTION BOARD")]
    [SerializeField] GameObject OptionBoardHolder;


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
        string hpText = "HP : " + player.status.curHp.Value.ToString() + "/" + player.status.maxHp;
        string armorText = "";
        string moveSpeedText = "Move Speed : " + player.status.walkSpeed.ToString() + player.inventroy.invenBonusStatus.bonus_Player_Speed.ToString();

        string expectDamageText = "DMG : " + ((player.curWeapon.weaponData.damage) *
            (player.curWeapon.weaponData.fireRate) *
            (player.curWeapon.weaponData.bulletNumPerFire)).ToString();

        string consumeRangeText = "Consume Range : " + player.curWeapon.suctionStat.suctionRange.ToString();
        string consumeAngleText = "Consume Angle : " + player.curWeapon.suctionStat.suctionAngle.ToString();
        string bulletTypeText = "Bullet Type : " + player.curWeapon.weaponData.bulletType.ToString();
        string bulletNumPerFireText = "Fire Rate : " + (player.curWeapon.weaponData.bulletNumPerFire).ToString();
        string bulletSpreadText = "Spread : " + player.curWeapon.weaponData.spread.ToString();
        string dpsText = "DPS : " + player.curWeapon.weaponData.fireRate.ToString();

        playerStatusText.text = hpText /*+ "\n" +armorText*/+"\n"+moveSpeedText+"\n"+expectDamageText+"\n"+consumeRangeText+"\n"+consumeAngleText+"\n"+bulletTypeText+"\n"+bulletNumPerFireText+"\n"+bulletSpreadText+"\n"+dpsText;
    }

    public void ShowMenu(MenuList menu)
    {
        statusBoardHolder.SetActive(false);
        ItemBoardHolder.SetActive(false);
        ItemInfoBoardHolder.SetActive(false);
        CombinelistBoardHolder.SetActive(false);
        OptionBoardHolder.SetActive(false);

        switch (menu)
        {
            case MenuList.Status:
            {
               statusBoardHolder.SetActive(true);
            }
            break;
        case MenuList.Inventory:
            {
                    ItemBoardHolder.SetActive(true);
                    ItemInfoBoardHolder.SetActive(true);
            }
            break;
            case MenuList.CombineList:
                {
                    CombinelistBoardHolder.SetActive(true);
                }
                break;
        case MenuList.Option:
            {
                    OptionBoardHolder.SetActive(true);
            }
            break;
        case MenuList.Exit:
            {

            }
            break;
        }
    }

    public void ChangeButtonSprite(MenuList menu)
    {
        Sprite defaultButtonSprite = Resources.Load<Sprite>("Sprites/SpriteSheet/UI_Sheet_01_3");
        Sprite selectButtonSprite = Resources.Load<Sprite>("Sprites/SpriteSheet/UI_Sheet_01_2");

        for (int i = 0; i < Menu_Buttons.Length; i++)
        {
            Menu_Buttons[i].gameObject.GetComponent<Image>().sprite = defaultButtonSprite;
        }

        Menu_Buttons[(int)menu].GetComponent<Image>().sprite = selectButtonSprite;
    }
}
