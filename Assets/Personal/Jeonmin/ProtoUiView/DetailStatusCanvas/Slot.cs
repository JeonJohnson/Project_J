using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Slot : MonoBehaviour
{
    [SerializeField] Image icon;
    [HideInInspector] public Item holdingItem;
    public Button button;

    public void UpdateSlot(Item item)
    {
        if (item != null)
        {
            holdingItem = item;
            icon.enabled = true;
            icon.sprite = item.item_sprite;
        }
        else
        {
            icon.enabled = false;
        }
    }
}
