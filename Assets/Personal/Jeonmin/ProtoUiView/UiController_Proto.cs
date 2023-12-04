using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiController_Proto : Singleton<UiController_Proto>
{
    public UiView playerUiView;
    public GameObject detailStatusCanvasGo;

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
        playerUiView.UpdatePassiveItem(null, player.inventroy.passiveItemSlot);
    }

    private void SubscribeUiToPlayer()
    {
        if (player.status.curHp != null)
           player.status.curHp.onChange += UpdateHpImage;

        if (player.inventroy.activeItemSlot != null)
            player.inventroy.activeItemSlot.cooldownTimer.onChange += UpdateActiveItemGauge;

        if (player.curWeapon.suctionStat.curSuctionRatio != null)
            player.curWeapon.suctionStat.curSuctionRatio.onChange += playerUiView.UpdateWeaponConsume;

        if (player.curWeapon.defaltStatus.bulletCount != null)
            player.curWeapon.defaltStatus.bulletCount.onChange += playerUiView.UpdateBulletCount;
    }

    public void SubscribeActiveUiToItem()
    {
        if (player.inventroy.activeItemSlot != null)
            player.inventroy.activeItemSlot.cooldownTimer.onChange += UpdateActiveItemGauge;
    }

    private void UpdateHpImage(int hp)
    {
        playerUiView.UpdateHpImage(hp, player.status.maxHp);
        playerUiView.PlayHitFeedback();
    }

    public void UpdateActiveItemGauge(float value)
    {
        playerUiView.UpdateActiveItemGauge(value / player.inventroy.activeItemSlot.cooldownTime);
        Debug.Log(value / player.inventroy.activeItemSlot.cooldownTime);
    }
}
