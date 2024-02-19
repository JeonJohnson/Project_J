using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform canvas;               // UI가 소속되어 있는 최상단의 Canvas Transform
    public Transform previousParent;       // 해당 오브젝트가 직전에 소속되어 있었던 부모 Transfron
    private RectTransform rect;             // UI 위치 제어를 위한 RectTransform
    private CanvasGroup canvasGroup;        // UI의 알파값과 상호작용 제어를 위한 CanvasGroup

    private Item _item;
    public Item Item
    { get { return _item; } }
    private Image image;

    private bool isDragging = false;
    private bool isHovering = false;
    private float hoveringTimer = 0f;

    private void Awake()
    {
        canvas = UiController_Proto.Instance.runeView.gameObject.transform;
        Debug.Log(canvas.gameObject.name);
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (isHovering) hoveringTimer += Time.unscaledDeltaTime;
        else hoveringTimer = 0f;

        if(isHovering)
        {
            if (hoveringTimer >= 0.5f && !isDragging)
            {
                UiController_Proto.Instance.runeView.slotHoverView.gameObject.SetActive(true);
                UiController_Proto.Instance.runeView.slotHoverView.UpdateInfo(_item);
            }
            else
            {
                UiController_Proto.Instance.runeView.slotHoverView.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 마우스 포인트가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    /// <summary>
    /// 마우스 포인트가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("탈출");
        isHovering = false;
        UiController_Proto.Instance.runeView.slotHoverView.gameObject.SetActive(false);
    }

    /// <summary>
    /// 현재 오브젝트를 드래그하기 시작할 때 1회 호출
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 직전에 소속되어 있던 부모 Transform 정보 저장
        previousParent = transform.parent;

        // 현재 드래그중인 UI가 화면의 최상단에 출력되도록 하기 위해
        transform.SetParent(canvas);        // 부모 오브젝트를 Canvas로 설정
        transform.SetAsLastSibling();       // 가장 앞에 보이도록 마지막 자식으로 설정

        // 드래그 가능한 오브젝트가 하나가 아닌 자식들을 가지고 있을 수도 때문에 CanvasGroup으로 통제
        // 알파값을 0.6으로 설정하고, 광선 충돌처리가 되지 않도록 한다
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        isDragging = true;
    }

    /// <summary>
    /// 현재 오브젝트를 드래그 중일 때 매 프레임 호출
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 현재 스크린상의 마우스 위치를 UI 위치로 설정 (UI가 마우스를 쫓아다니는 상태)
        rect.position = eventData.position;
    }

    /// <summary>
    /// 현재 오브젝트의 드래그를 종료할 때 1회 호출
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그를 시작하면 부모가 canvas로 설정되기 때문에
        // 드래그를 종료할 때 부모가 canvas이면 아이템 슬롯이 아닌 엉뚱한 곳에
        // 드롭을 했다는 뜻이기 때문에 드래그 직전에 소속되어 있던 아이템 슬롯으로 아이템 이동
        if (transform.parent == canvas)
        {
            // 마지막에 소속되어있었던 previousParent의 자식으로 설정하고, 해당 위치로 설정
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }

        // 알파값을 1로 설정하고, 광선 충돌처리가 되도록 한다
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        isDragging = false;
    }

    public void UpdateView(Item item)
    {
        _item = item;
        image.sprite = _item.item_sprite;
    }

    private void OnDisable()
    {

        isHovering = false;
        isDragging = false;
    }
}

