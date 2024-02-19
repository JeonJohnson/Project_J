using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopSlot : MonoBehaviour
{
    private bool isBuyable = true;

    private CanvasGroup canvasGroup;

    private Item item;
    public Item Item
    { get { return item; } }

    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemInfoText;


    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void UpdateView(Item _item)
    {
        item = _item;
        image.sprite = _item.item_sprite;
        itemNameText.text = _item.item_name;
        itemPriceText.text = "АЁАн: " +  _item.item_price.ToString();
        itemInfoText.text = _item.item_description;
    }

    public void Buy()
    {
        if (!isBuyable) { return; }

        if(IngameController.Instance.Player.shop.BuyItem((Item_Rune)item) == true)
        {
            isBuyable = false;
            canvasGroup.DOFade(0f, 0.15f);//.OnComplete(() => Destroy(this.gameObject));
        }
    }
}
