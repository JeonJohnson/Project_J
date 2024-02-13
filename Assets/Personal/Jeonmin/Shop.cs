using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<Item_Rune> shopItems = new List<Item_Rune>();

    public void ShowRandomItems(int numberOfItems)
    {
        Item_Rune[] selectedItems = new Item_Rune[numberOfItems];

        for (int i = 0; i < numberOfItems; i++)
        {
            selectedItems[i] = GetRandomItem();
            Debug.Log("상점에 추가된 아이템: " + selectedItems[i].item_name);
        }

        shopItems.AddRange(selectedItems);
    }

    public void RemoveItem(Item_Rune item)
    {
        shopItems.Remove(item);
    }

    private Item_Rune GetRandomItem()
    {
        return shopItems[Random.Range(0, shopItems.Count)];
    }
}
