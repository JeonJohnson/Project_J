using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enums;

public class DroppableSlot : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image image;
    private RectTransform rect;

    public int slotIndex;
    public SlotType slotType;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 마우스 포인트가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템 슬롯의 색상을 노란색으로 변경
        image.color = Color.yellow;
    }

    /// <summary>
    /// 마우스 포인트가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // 아이템 슬롯의 색상을 하얀색으로 변경
        image.color = Color.white;
    }

    /// <summary>
    /// 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag는 현재 드래그하고 있는 대상(=아이템)
        if (eventData.pointerDrag != null)
        {
            PlayerInventroy inventory = IngameController.Instance.Player.inventroy;

            DraggableSlot draggableSlot = eventData.pointerDrag.GetComponent<DraggableSlot>();
            DroppableSlot previousSlot = draggableSlot.previousParent.gameObject.GetComponent<DroppableSlot>();
            if (draggableSlot)
            {
                if (slotType == SlotType.Slot)
                {
                    if(previousSlot.slotType == SlotType.Slot)
                    {
                        inventory.ReplaceRune(inventory.runeList, slotIndex, inventory.runeList, previousSlot.slotIndex);
                    }
                    else if(previousSlot.slotType == SlotType.EqupedSlot)
                    {
                        inventory.equipedRuneList[previousSlot.slotIndex].UnEquip(IngameController.Instance.Player);
                        inventory.ReplaceRune(inventory.runeList, slotIndex, inventory.equipedRuneList, previousSlot.slotIndex);
                    }
                }
                else if(slotType == SlotType.EqupedSlot)
                {
                    if (previousSlot.slotType == SlotType.Slot)
                    {
                        inventory.runeList[previousSlot.slotIndex].Equip(IngameController.Instance.Player);
                        inventory.ReplaceRune(inventory.equipedRuneList, slotIndex, inventory.runeList, previousSlot.slotIndex);
                    }
                    else if (previousSlot.slotType == SlotType.EqupedSlot)
                    {
                        inventory.ReplaceRune(inventory.equipedRuneList, slotIndex, inventory.equipedRuneList, previousSlot.slotIndex);
                    }
                }
            }
            // 드래그하고 있는 대상의 부모를 현재 오브젝트로 설정하고, 위치를 현재 오브젝트 위치와 동일하게 설정
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;

            UI_RuneView.instance.UpdateSlots();
        }
    }
}