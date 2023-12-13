using UnityEngine;
using System.Collections;

public class TestLaserGun : MonoBehaviour
{
    public Transform firePoint;
    public LayerMask wallLayer;
    public LineRenderer lineRenderer;
    public Gradient laserGradient;
    public float maxReflections = 5;
    public float fadeDuration = 1.5f;
    public float timeDelay = 0.2f;

    public Bullet_Laser laserBullet;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject bulletGo = Instantiate(laserBullet.gameObject,this.transform.position,Quaternion.identity);
            bulletGo.transform.eulerAngles = this.transform.eulerAngles;
            bulletGo.GetComponent<Bullet_Laser>().Fire(this.transform.right, 5, 0f);
            //StartCoroutine(FireLaserCoro());
        }
    }

    Vector2 direction;
    private void FireLaser()
    {
        firePoint.transform.position = this.transform.position;
        firePoint.transform.rotation = this.transform.rotation;
        int reflections = 0;
        direction = firePoint.right;
        Vector3 originalFirePointPosition = firePoint.position;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, originalFirePointPosition);

        while (reflections <= maxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, Mathf.Infinity, wallLayer);

            if (hit.collider)
            {
                // Add a new point to the line renderer with the appropriate color
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                lineRenderer.colorGradient = laserGradient;

                // Reflect the direction after a delay
                StartCoroutine(ReflectWithDelay(direction, hit.normal, timeDelay));

                // Set the new fire point
                firePoint.position = hit.point + direction * 0.01f;

                reflections++;
            }
            else
            {
                // If no wall hit, end the line renderer
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, originalFirePointPosition + (Vector3)direction * 100f);

                break;
            }
        }

        // Start the fade-out process for each segment
        StartCoroutine(FadeOut());
    }

    IEnumerator FireLaserCoro()
    {

        firePoint.transform.position = this.transform.position;
        firePoint.transform.rotation = this.transform.rotation;
        int reflections = 0;
        direction = firePoint.right;
        Vector3 originalFirePointPosition = firePoint.position;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, originalFirePointPosition);

        while (reflections <= maxReflections)
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, Mathf.Infinity, wallLayer);

            if (hit.collider)
            {
                // Add a new point to the line renderer with the appropriate color
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                lineRenderer.colorGradient = laserGradient;

                // Reflect the direction after a delay
                yield return new WaitForSeconds(timeDelay);
                direction = Vector2.Reflect(direction, hit.normal);

                // Set the new fire point
                firePoint.position = hit.point + direction * 0.01f;

                reflections++;
            }
            else
            {
                // If no wall hit, end the line renderer
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, originalFirePointPosition + (Vector3)direction * 100f);

                break;
            }
        }

        // Start the fade-out process for each segment
        StartCoroutine(FadeOut());
    }


    private IEnumerator ReflectWithDelay(Vector2 incomingDirection, Vector2 normal, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reflect the direction after the delay
        direction = Vector2.Reflect(incomingDirection, normal);
    }

    private IEnumerator FadeOut()
    {
        int segmentCount = lineRenderer.positionCount;
        float segmentFadeDuration = fadeDuration / segmentCount;

        for (int i = 0; i < 2; i++)
        {
            // Calculate the start and end colors for the current segment
            int endIndex = Mathf.Min(i + 1, segmentCount - 1);
            Color startColor = lineRenderer.startColor;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < segmentFadeDuration)
            {
                // Interpolate between start and end colors
                float alpha = Mathf.Lerp(startColor.a, endColor.a, elapsedTime / segmentFadeDuration);
                lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
                lineRenderer.endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set the color to completely transparent
            lineRenderer.startColor = endColor;
            lineRenderer.endColor = endColor;
        }

        // Reset the position count
        lineRenderer.positionCount = 0;
    }
}
