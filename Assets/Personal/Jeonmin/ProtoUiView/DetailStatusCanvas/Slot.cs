using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Slot : MonoBehaviour
{
    [SerializeField] Image icon;

    public void UpdateSlot(Item item)
    {
        if (item != null)
        {
            icon.enabled = true;
            icon.sprite = item.item_sprite;
        }
        else icon.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("인포 활성화");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("인포 비활성화");
    }
}
