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
        // ���� ���콺 ��ġ ��������
        Vector3 mousePosition = Input.mousePosition;

        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);

        // ũ�ν���� ��ġ ����
        crosshair.localPosition = localPoint;
    }
}
