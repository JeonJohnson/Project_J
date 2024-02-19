using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            IngameController.Instance.Player.inventroy.coinCount.Value += 20;
            ShowRandomItems(4);
        }
    }

    public Item_Rune[] itemArray;
    public List<Item_Rune> shopItems = new List<Item_Rune>();

    public void ShowRandomItems(int numberOfItems)
    {
        Item_Rune[] selectedItems = new Item_Rune[numberOfItems];

        for (int i = 0; i < numberOfItems; i++)
        {
            selectedItems[i] = GetRandomItem();
            Debug.Log("상점에 아이템 추가: " + selectedItems[i].item_name);
            UiController_Proto.Instance.shopView.AddShopSlot(selectedItems[i]);
        }

        shopItems.AddRange(selectedItems);
    }

    public bool BuyItem(Item_Rune item)
    {
        if (IngameController.Instance.Player.inventroy.coinCount.Value < item.item_price)
            return false;

        IngameController.Instance.Player.inventroy.AddRune(item);
        IngameController.Instance.Player.inventroy.coinCount.Value -= item.item_price;
        RemoveItem(item);
        return true;
    }

    public void RemoveItem(Item_Rune item)
    {
        shopItems.Remove(item);
    }

    private Item_Rune GetRandomItem()
    {
        return itemArray[Random.Range(0, itemArray.Length)];
    }

    public void ExitShop()
    {
        shopItems.Clear();
    }
}
