using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Ui_DetailStatus_View;

public class UiController_Proto : Singleton<UiController_Proto>
{
    public Ui_DetailStatus_View playerDetailStatusView;
    public UiView playerHudView;
    public UI_RuneView runeView;
    public UI_ShopView shopView;

    public Player player;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    private void Awake()
    {
        Initailize(true);
    }

    private void Start()
    {
        player = IngameController.Instance.Player;
        SubscribeUiToPlayer();
        InjectMenuButtonEvent();

        // 초기설정
        UpdateHpImage(player.status.curHp.Value);
        playerHudView.UpdateWeaponRemainCount(0);
    }

    private void SubscribeUiToPlayer()
    {
        if (player.status.curHp != null)
           player.status.curHp.onChange += UpdateHpImage;

        if (player.curWeapon.suctionStat.curSuctionRatio != null)
            player.curWeapon.suctionStat.curSuctionRatio.onChange += playerHudView.UpdateWeaponConsume;

        if (player.inventroy.bulletCount != null)
            player.inventroy.bulletCount.onChange += playerHudView.UpdateBulletCount;

        if (player.inventroy.ejectRemainBulletCount != null)
            player.inventroy.ejectRemainBulletCount.onChange += playerHudView.UpdateWeaponRemainCount;

        if (player.inventroy.coinCount != null)
            player.inventroy.coinCount.onChange += playerHudView.UpdateCoinCount;
    }

    public void SubscribeActiveUiToItem()
    {
        //if (player.inventroy.activeItemSlot != null)
        //    player.inventroy.activeItemSlot.cooldownTimer.onChange += UpdateActiveItemGauge;
    }

    private void UpdateHpImage(int hp)
    {
        playerHudView.UpdateHpImage(hp, player.status.maxHp);
        if (hp < player.status.maxHp) playerHudView.PlayHitFeedback();
    }

    public void UpdateWeaponImage(Sprite sprite)
    {
        playerHudView.UpdateWeapon(sprite);
    }

    public void UpdateActiveItemGauge(float value)
    {
        //playerHudView.UpdateActiveItemGauge(value / player.inventroy.activeItemSlot.cooldownTime);
        //Debug.Log(value / player.inventroy.activeItemSlot.cooldownTime);
    }

    public void ShowDetailStatusWindow(bool isTrue)
    {
        if(isTrue)
        {
            if (playerDetailStatusView.gameObject.activeSelf) return;
            playerDetailStatusView.gameObject.SetActive(isTrue);
            ShowMenu(MenuList.Status);
        }
        else
        {
            playerDetailStatusView.gameObject.SetActive(isTrue);
        }
    }

    public void InjectMenuButtonEvent()
    {
        for(int i = 0; i < playerDetailStatusView.Menu_Buttons.Length; i++)
        {
            int temp = i;
            playerDetailStatusView.Menu_Buttons[i].onClick.AddListener(()=> { ShowMenu((MenuList)temp); });
        }
    }

    public void ShowMenu(MenuList menu)
    {
        playerDetailStatusView.ShowMenu(menu);
        switch (menu)
        {
            case MenuList.Status:
                {
                    playerDetailStatusView.UpdatePlayerStatusHolder(player);
                }
                break;
            case MenuList.Inventory:
                {
                    playerDetailStatusView.UpdateItemBoardHolder(player.inventroy);
                    //playerDetailStatusView.UpdateItemInfoBoardHolder(player.inventroy.activeItemSlot);
                }
                break;
            case MenuList.CombineList:
                {

                }
                break;
            case MenuList.Option:
                {

                }
                break;
            case MenuList.Exit:
                {

                }
                break;
        }
    }

    public void ShowResultWindow(bool value, bool isWin)
    {
        playerHudView.resultCanvasGroup.alpha = 0f;
        playerHudView.resultHolder.SetActive(value);
        if (value == true) 
        {
            playerHudView.UpdateResult(isWin);
            playerHudView.resultCanvasGroup.DOFade(1f, 0.5f);
        }
    }

    public void ShowRuneWindow(bool value)
    {
        runeView.gameObject.SetActive(value);
        if(value == true) runeView.UpdateSlots();
    }

    public void ShowShopWindow(bool value)
    {
        if(value == true)
        {
            IngameController.Instance.ResetAllWindow();
            IngameController.Instance?.Player.LockPlayer(true);
            shopView.UpdateCoinCountView(IngameController.Instance.Player.inventroy.coinCount.Value);
            Cursor.visible = true;
        }
        else
        {
            IngameController.Instance?.Player.LockPlayer(false);
            Cursor.visible = false;
        }

        shopView.gameObject.SetActive(value);
    }
}
