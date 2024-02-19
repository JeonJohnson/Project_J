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

        // 툴팁의 위치를 설정합니다.
        tooltipRect.anchoredPosition = ClampToScreen((Vector2)Input.mousePosition + offset);
    }

    private Vector2 ClampToScreen(Vector2 position)
    {
        Vector2 clampedPosition = position;

        // 툴팁이 화면 가시 영역을 벗어나는지 확인하고 조정합니다.
        if (canvasRect != null && tooltipRect != null)
        {
            Vector2 pivot = tooltipRect.pivot;
            Vector2 localPivot = new Vector2(canvasRect.rect.width * pivot.x, canvasRect.rect.height * pivot.y);

            // 툴팁이 화면 위쪽으로 벗어나는 경우
            if (position.y + tooltipRect.rect.height + localPivot.y > canvasRect.rect.height)
            {
                clampedPosition.y = canvasRect.rect.height - tooltipRect.rect.height - localPivot.y;
            }

            // 툴팁이 화면 왼쪽으로 벗어나는 경우
            if (position.x + tooltipRect.rect.width + localPivot.x > canvasRect.rect.width)
            {
                clampedPosition.x = canvasRect.rect.width - tooltipRect.rect.width - localPivot.x;
            }
        }

        return clampedPosition;
    }
}
