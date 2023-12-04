using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class ItemPicker : MonoBehaviour
{
    public Item itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] GameObject interactButton;

    private void Awake()
    {
        CopyScriptableObjectItem();
        itemSpriteRenderer.sprite = itemData.item_sprite;
    }

    private void CopyScriptableObjectItem()
    {
        Type itemType = itemData.GetType();
        if (itemType == typeof(Item_Active))
        {
            Item_Active copyedItem = itemData.Copy<Item_Active>();
            itemData = copyedItem;
        }
        else if (itemType == typeof(Item_Passive))
        {
            Item_Passive copyedItem = itemData.Copy<Item_Passive>();
            itemData = copyedItem;
        }
        else
        {
            Item copyedItem = itemData.Copy<Item>();
            itemData = copyedItem;
        }
    }

    public void Equip(Player player)
    {
        itemData.Equip(player);
        this.gameObject.SetActive(false);
    }

    public void ShowInteractButton(bool value)
    {
        interactButton.SetActive(value);
    }
}
