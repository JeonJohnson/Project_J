using UnityEngine;

public class ConeSkillIndicator : MonoBehaviour
{
    public float skillRange = 5f; // 스킬의 최대 사거리
    public int numberOfPoints = 30; // 원뿔을 구성하는 점의 개수
    public float coneAngle = 45f; // 원뿔의 각도

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
            lineRenderer.SetPosition(i, transform.TransformPoint(point)); // 로컬 좌표계를 월드 좌표계로 변환

            currentAngle += angleStep;
        }
    }
}