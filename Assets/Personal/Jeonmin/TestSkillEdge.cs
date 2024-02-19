using UnityEngine;

public class ConeSkillIndicator : MonoBehaviour
{
    public float skillRange = 5f; // ��ų�� �ִ� ��Ÿ�
    public int numberOfPoints = 30; // ������ �����ϴ� ���� ����
    public float coneAngle = 45f; // ������ ����

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawCone();
    }

    private void Update()
    {
            DrawCone();
    }

    void DrawCone()
    {
        lineRenderer.positionCount = numberOfPoints + 1;
        float angleStep = coneAngle / numberOfPoints;
        float currentAngle = -coneAngle / 2;

        for (int i = 0; i <= numberOfPoints; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * skillRange;
            float y = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * skillRange;

            Vector3 point = new Vector3(x, y, 0f);
            lineRenderer.SetPosition(i, transform.TransformPoint(point)); // ���� ��ǥ�踦 ���� ��ǥ��� ��ȯ

            currentAngle += angleStep;
        }
    }
}