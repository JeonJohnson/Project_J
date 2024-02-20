using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ShopView : MonoBehaviour
{
    public GameObject shopSlotPrefab;
    public Transform slotTr;
    public List<GameObject> shopSlots = new List<GameObject>();
    [SerializeField] TextMeshProUGUI coinCountText;
    public CanvasGroup canvasGroup;

    public void AddShopSlot(Item item)
    {
        GameObject slotGo = Instantiate(shopSlotPrefab, slotTr);
        shopSlots.Add(slotGo);
        ShopSlot slot = slotGo.GetComponent<ShopSlot>();

        slot.UpdateView(item);
    }

    public void ExitShop()
    {
        IngameController.Instance.Player.shop.ExitShop();
        for(int i = shopSlots.Count - 1; i >= 0; i--)
        {
            Destroy(shopSlots[i]);
        }
        UiController_Proto.Instance.ShowShopWindow(false);
    }

    public void UpdateCoinCountView(int coinCount)
    {
        coinCountText.text = ":" + coinCount.ToString();
    }

    private void OnEnable()
    {
        IngameController.Instance.Player.inventroy.coinCount.onChange += UpdateCoinCountView;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.15f).SetUpdate(true);
    }
    private void OnDisable()
    {
        IngameController.Instance.Player.inventroy.coinCount.onChange -= UpdateCoinCountView;
    }
}
