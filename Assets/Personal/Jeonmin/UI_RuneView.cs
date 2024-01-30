using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public Slot[] slots;
    public Slot[] equipSlots;

    [SerializeField] Image selectedSlotImage;
    [SerializeField] TextMeshProUGUI selectedSlotText;

    public SlotHoverView slotHoverView;

    public void UpdateSlotInfo()
    {

    }

    public void UpdateSlots()
    {

    }
}
