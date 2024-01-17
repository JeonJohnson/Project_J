using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrissHair : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform crosshair;

    private void Awake()
    {
        crosshair = GetComponent<RectTransform>();
        Cursor.visible = false;
    }
    void Update()
    {
        // 현재 마우스 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;

        // 마우스 위치를 월드 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);

        // 크로스헤어 위치 설정
        crosshair.localPosition = localPoint;
    }
}
