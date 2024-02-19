using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform canvas;               // UI�� �ҼӵǾ� �ִ� �ֻ���� Canvas Transform
    public Transform previousParent;       // �ش� ������Ʈ�� ������ �ҼӵǾ� �־��� �θ� Transfron
    private RectTransform rect;             // UI ��ġ ��� ���� RectTransform
    private CanvasGroup canvasGroup;        // UI�� ���İ��� ��ȣ�ۿ� ��� ���� CanvasGroup

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
            Debug.Log(hoveringTimer);
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
    /// ���콺 ����Ʈ�� ���� ������ ���� ���� ���η� �� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    /// <summary>
    /// ���콺 ����Ʈ�� ���� ������ ���� ������ �������� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Ż��");
        isHovering = false;
        UiController_Proto.Instance.runeView.slotHoverView.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� ������Ʈ�� �巡���ϱ� ������ �� 1ȸ ȣ��
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // �巡�� ������ �ҼӵǾ� �ִ� �θ� Transform ���� ����
        previousParent = transform.parent;

        // ���� �巡������ UI�� ȭ���� �ֻ�ܿ� ��µǵ��� �ϱ� ����
        transform.SetParent(canvas);        // �θ� ������Ʈ�� Canvas�� ����
        transform.SetAsLastSibling();       // ���� �տ� ���̵��� ������ �ڽ����� ����

        // �巡�� ������ ������Ʈ�� �ϳ��� �ƴ� �ڽĵ��� ������ ���� ���� ������ CanvasGroup���� ����
        // ���İ��� 0.6���� �����ϰ�, ���� �浹ó���� ���� �ʵ��� �Ѵ�
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        isDragging = true;
    }

    /// <summary>
    /// ���� ������Ʈ�� �巡�� ���� �� �� ������ ȣ��
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // ���� ��ũ������ ���콺 ��ġ�� UI ��ġ�� ���� (UI�� ���콺�� �Ѿƴٴϴ� ����)
        rect.position = eventData.position;
    }

    /// <summary>
    /// ���� ������Ʈ�� �巡�׸� ������ �� 1ȸ ȣ��
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�׸� �����ϸ� �θ� canvas�� �����Ǳ� ������
        // �巡�׸� ������ �� �θ� canvas�̸� ������ ������ �ƴ� ������ ����
        // ����� �ߴٴ� ���̱� ������ �巡�� ������ �ҼӵǾ� �ִ� ������ �������� ������ �̵�
        if (transform.parent == canvas)
        {
            // �������� �ҼӵǾ��־��� previousParent�� �ڽ����� �����ϰ�, �ش� ��ġ�� ����
            transform.SetParent(previousParent);
            rect.position = previousParent.GetComponent<RectTransform>().position;
        }

        // ���İ��� 1�� �����ϰ�, ���� �浹ó���� �ǵ��� �Ѵ�
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
