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
        this.gameObject.SetActive(false);
    }

    public void UpdateCoinCountView(int coinCount)
    {
        coinCountText.text = ":" + coinCount.ToString();
    }

    private void OnEnable()
    {
        IngameController.Instance.Player.inventroy.coinCount.onChange += UpdateCoinCountView;
    }
    private void OnDisable()
    {
        IngameController.Instance.Player.inventroy.coinCount.onChange -= UpdateCoinCountView;
    }
}
