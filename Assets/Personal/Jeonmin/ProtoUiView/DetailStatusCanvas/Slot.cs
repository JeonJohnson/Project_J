using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [SerializeField] Image icon;
    [HideInInspector] public Item holdingItem;
    public Button button;

    private bool isHovering = false;
    private float hoverTimer = 0f;

    private void Update()
    {
        if(isHovering)
        {
            hoverTimer += Time.deltaTime;
            if (hoverTimer >= 0.2f)
            {
                UI_RuneView.instance?.slotHoverView.UpdateInfo(holdingItem);
                UI_RuneView.instance?.slotHoverView.gameObject.SetActive(true);
            }
            else
            {
                UI_RuneView.instance?.slotHoverView.gameObject.SetActive(false);
            }
        }
        else
        {
            UI_RuneView.instance?.slotHoverView.gameObject.SetActive(false);
            hoverTimer = 0f;
        }
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (holdingItem != null)
            {
                UI_RuneView.instance.UpdateSlotInfo();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isHovering = false;
    }
}
