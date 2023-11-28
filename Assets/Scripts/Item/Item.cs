using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] GameObject interactButton;

    private void Awake()
    {
        itemSpriteRenderer.sprite = itemData.item_sprite;
    }

    public void Equip(Player player)
    {
        this.gameObject.SetActive(false);
    }

    public void ShowInteractButton(bool value)
    {
        interactButton.SetActive(value);
    }
}
