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

    }

    private void Start()
    {
        player = IngameController.Instance.Player;
        SubscribeUiToPlayer();
        InjectMenuButtonEvent();
        

        // 초기설정
        UpdateHpImage(player.status.curHp.Value);
        playerHudView.UpdatePassiveItem(null, player.inventroy.passiveItemSlot);
    }

    private void SubscribeUiToPlayer()
    {
        if (player.status.curHp != null)
           player.status.curHp.onChange += UpdateHpImage;

        if (player.inventroy.activeItemSlot != null)
            player.inventroy.activeItemSlot.cooldownTimer.onChange += UpdateActiveItemGauge;

        if (player.curWeapon.suctionStat.curSuctionRatio != null)
            player.curWeapon.suctionStat.curSuctionRatio.onChange += playerHudView.UpdateWeaponConsume;

        if (player.curWeapon.defaltStatus.bulletCount != null)
            player.curWeapon.defaltStatus.bulletCount.onChange += playerHudView.UpdateBulletCount;
    }

    public void SubscribeActiveUiToItem()
    {
        if (player.inventroy.activeItemSlot != null)
            player.inventroy.activeItemSlot.cooldownTimer.onChange += UpdateActiveItemGauge;
    }

    private void UpdateHpImage(int hp)
    {
        playerHudView.UpdateHpImage(hp, player.status.maxHp);
        if (hp < player.status.maxHp) playerHudView.PlayHitFeedback();
    }

    public void UpdateActiveItemGauge(float value)
    {
        playerHudView.UpdateActiveItemGauge(value / player.inventroy.activeItemSlot.cooldownTime);
        Debug.Log(value / player.inventroy.activeItemSlot.cooldownTime);
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
                    playerDetailStatusView.UpdateItemInfoBoardHolder(player.inventroy.activeItemSlot);
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
}
