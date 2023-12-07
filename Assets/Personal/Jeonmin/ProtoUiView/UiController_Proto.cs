using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        GameObject playerGo = GameObject.Find("Player");
        if (playerGo != null) player = playerGo.GetComponent<Player>();
        else Debug.Log("Can't Find Player On Scene");
    }

    private void Start()
    {
        SubscribeUiToPlayer();

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
        playerHudView.PlayHitFeedback();
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
            playerDetailStatusView.UpdatePlayerStatus(player);
            playerDetailStatusView.UpdateSlots(player.inventroy);
        }
        else
        {
            playerDetailStatusView.gameObject.SetActive(isTrue);
        }
    }
}
