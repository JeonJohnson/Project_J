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
    /// ���콺 ����Ʈ�� ���� ������ ���� ���� ���η� �� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������ ������ ������ ��������� ����
        image.color = Color.yellow;
    }

    /// <summary>
    /// ���콺 ����Ʈ�� ���� ������ ���� ������ �������� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // ������ ������ ������ �Ͼ������ ����
        image.color = Color.white;
    }

    /// <summary>
    /// ���� ������ ���� ���� ���ο��� ����� ���� �� 1ȸ ȣ��
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag�� ���� �巡���ϰ� �ִ� ���(=������)
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
            // �巡���ϰ� �ִ� ����� �θ� ���� ������Ʈ�� �����ϰ�, ��ġ�� ���� ������Ʈ ��ġ�� �����ϰ� ����
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;

            UI_RuneView.instance.UpdateSlots();
        }
    }
}