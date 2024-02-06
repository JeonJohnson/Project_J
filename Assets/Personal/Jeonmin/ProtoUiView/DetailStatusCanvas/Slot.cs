using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler
{
    [SerializeField] Image icon;
    [HideInInspector] public Item holdingItem;
    public Button button;

    private bool isHovering = false;
    private float hoverTimer = 0f;

    private	Image			image;
	private	RectTransform	rect;

	private void Awake()
	{
		image	= GetComponent<Image>();
		rect	= GetComponent<RectTransform>();
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
		if ( eventData.pointerDrag != null )
		{
			// 드래그하고 있는 대상의 부모를 현재 오브젝트로 설정하고, 위치를 현재 오브젝트 위치와 동일하게 설정
			eventData.pointerDrag.transform.SetParent(transform);
			eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
		}
	}


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

    public void OnDrag(PointerEventData eventData)
    {
        isHovering = false;
    }
}
