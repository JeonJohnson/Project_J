using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotHoverView : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameTmp;
    [SerializeField] private TextMeshProUGUI itemInfoTmp;

    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private Vector2 offset;

    public void UpdateInfo(Item item)
    {
        if(item == null) return;
        itemImage.sprite = item.item_sprite;
        itemNameTmp.text = item.item_name;
        itemInfoTmp.text = item.item_description;
    }

    private void Update()
    {
        this.gameObject.transform.position = Input.mousePosition;

        // ������ ��ġ�� �����մϴ�.
        tooltipRect.anchoredPosition = ClampToScreen((Vector2)Input.mousePosition + offset);
    }

    private Vector2 ClampToScreen(Vector2 position)
    {
        Vector2 clampedPosition = position;

        // ������ ȭ�� ���� ������ ������� Ȯ���ϰ� �����մϴ�.
        if (canvasRect != null && tooltipRect != null)
        {
            Vector2 pivot = tooltipRect.pivot;
            Vector2 localPivot = new Vector2(canvasRect.rect.width * pivot.x, canvasRect.rect.height * pivot.y);

            // ������ ȭ�� �������� ����� ���
            if (position.y + tooltipRect.rect.height + localPivot.y > canvasRect.rect.height)
            {
                clampedPosition.y = canvasRect.rect.height - tooltipRect.rect.height - localPivot.y;
            }

            // ������ ȭ�� �������� ����� ���
            if (position.x + tooltipRect.rect.width + localPivot.x > canvasRect.rect.width)
            {
                clampedPosition.x = canvasRect.rect.width - tooltipRect.rect.width - localPivot.x;
            }
        }

        return clampedPosition;
    }
}
