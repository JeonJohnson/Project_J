using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UI_RuneView : MonoBehaviour
{
    public static UI_RuneView instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public List<GameObject> slots = new List<GameObject>();
    public List<GameObject> equipSlots = new List<GameObject>();
    public List<GameObject> draggableSlots = new List<GameObject>();

    public Transform slotWindowTr;
    public Transform equpedSlotWindowTr;

    [SerializeField] Image selectedSlotImage;
    [SerializeField] TextMeshProUGUI selectedSlotText;

    public SlotHoverView slotHoverView;
    public GameObject droppableSlotPrefab;
    public GameObject draggableSlotPrefab;

    private bool isInitzed = false;

    public void UpdateSlots()
    {
        if(isInitzed)
        {
            for (int i = draggableSlots.Count - 1 ; i >= 0; i--)
            {
                Destroy(draggableSlots[i]);
            }
            draggableSlots.Clear();

            for (int i = 0; i < IngameController.Instance.Player.inventroy.runeList.Length; i++)
            {
                if (IngameController.Instance.Player.inventroy.runeList[i] != null)
                {
                    GameObject draggableSlot = Instantiate(draggableSlotPrefab, slots[i].transform);
                    draggableSlot.GetComponent<DraggableSlot>().UpdateView(IngameController.Instance.Player.inventroy.runeList[i]);
                    draggableSlots.Add(draggableSlot);
                }
            }
            for (int i = 0; i < IngameController.Instance.Player.inventroy.equipedRuneList.Length; i++)
            {
                if (IngameController.Instance.Player.inventroy.equipedRuneList[i] != null)
                {
                    GameObject draggableSlot = Instantiate(draggableSlotPrefab, equipSlots[i].transform);
                    draggableSlot.GetComponent<DraggableSlot>().UpdateView(IngameController.Instance.Player.inventroy.equipedRuneList[i]);
                    draggableSlots.Add(draggableSlot);
                }
            }
        }
        else
        {
            isInitzed = true;
            Init();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Init();
        }
    }

    public void Init()
    {
        for(int i = 0; i < IngameController.Instance.Player.inventroy.runeList.Length; i++)
        {
            GameObject droppableSlot = Instantiate(droppableSlotPrefab, slotWindowTr);
            slots.Add(droppableSlot);
            droppableSlot.GetComponent<DroppableSlot>().slotIndex = i;
            droppableSlot.GetComponent<DroppableSlot>().slotType = Enums.SlotType.Slot;

            if (IngameController.Instance.Player.inventroy.runeList[i] != null)
            {
                GameObject draggableSlot = Instantiate(draggableSlotPrefab, droppableSlot.transform);
                draggableSlot.GetComponent<DraggableSlot>().UpdateView(IngameController.Instance.Player.inventroy.runeList[i]);
                draggableSlots.Add(draggableSlot);
            }
        }

        for (int i = 0; i < IngameController.Instance.Player.inventroy.equipedRuneList.Length; i++)
        {
            GameObject droppableSlot = Instantiate(droppableSlotPrefab, equpedSlotWindowTr);
            equipSlots.Add(droppableSlot);
            droppableSlot.GetComponent<DroppableSlot>().slotIndex = i;
            droppableSlot.GetComponent<DroppableSlot>().slotType = Enums.SlotType.EqupedSlot;

            if (IngameController.Instance.Player.inventroy.equipedRuneList[i] != null)
            {
                GameObject draggableSlot = Instantiate(draggableSlotPrefab, droppableSlot.transform);
                draggableSlot.GetComponent<DraggableSlot>().UpdateView(IngameController.Instance.Player.inventroy.equipedRuneList[i]);
                draggableSlots.Add(draggableSlot);
            }
        }
    }
}
