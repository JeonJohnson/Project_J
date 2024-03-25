using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("ShopSceneFuncs")]
    public TextMeshProUGUI[] itemInfoTexts;

    public void ShowRandomItems(int numberOfItems)
    {
        Item_Rune[] selectedItems = new Item_Rune[numberOfItems];

        for (int i = 0; i < numberOfItems; i++)
        {
            selectedItems[i] = GetRandomItem();
            if (selectedItems[i] == null) break;
            shopItems.Add(selectedItems[i]);
            UiController_Proto.Instance.shopView.AddShopSlot(selectedItems[i]);
        }
        if (selectedItems == null) return;
    }

    public void ShowRandomItemObj(int numberOfItems)
    {
        Item_Rune[] selectedItems = new Item_Rune[numberOfItems];

        for (int i = 0; i < numberOfItems; i++)
        {
            selectedItems[i] = GetRandomItem();
            if (selectedItems[i] == null) break;
            shopItems.Add(selectedItems[i]);
            //UiController_Proto.Instance.shopView.AddShopSlot(selectedItems[i]);
        }
        if (selectedItems == null) return;
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
        Item_Rune[] equipedItemArray = IngameController.Instance.Player.inventroy.equipedRuneList;
        Item_Rune[] unequipedItemArray = IngameController.Instance.Player.inventroy.runeList;

        // equipedItemArray와 unequipedItemArray에 있는 모든 아이템을 포함하는 HashSet을 생성합니다.
        HashSet<Item_Rune> equippedAndUnequippedItems = new HashSet<Item_Rune>(equipedItemArray.Concat(unequipedItemArray));
        equippedAndUnequippedItems = new HashSet<Item_Rune>(equippedAndUnequippedItems.Concat(shopItems));

        // allItemArray에 있는 아이템 중 equippedAndUnequippedItems에 없는 아이템을 포함하는 리스트를 생성합니다.
        List<Item_Rune> nonEquippedItems = itemArray.Where(item => !equippedAndUnequippedItems.Contains(item)).ToList();

        if (nonEquippedItems.Count == 0) return null;
        // nonEquippedItems에서 랜덤으로 아이템을 선택하여 반환합니다.
        return nonEquippedItems[Random.Range(0, nonEquippedItems.Count)];

        //return allItemArray[Random.Range(0, allItemArray.Length)];
    }

    public void ExitShop()
    {
        shopItems.Clear();
    }
}
