using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Laser : Bullet
{
    public float knockBackPower = 1.0f;

    public LineRenderer laserLineRenderer;   // 레이저의 시각적인 효과
    public LineRenderer infoLineRenderer;
    private Vector2 dir;
    public float fireWaitTime = 0.5f;
    public LayerMask wallLayer;

    public float size = 0.5f;

    public override void Fire(Vector2 _dir, int SplatterCount, float speed, float bulletSize = 1f, int dmg = 1)
    {
        defaultStat.dmg = dmg;
        size = bulletSize;
        defaultStat.targetLayer &= ~(1 << LayerMask.NameToLayer("Player"));
        splatterStat.leftCount = SplatterCount;
        StartCoroutine(laserCoro(_dir));
        //StartCoroutine(infoLaserCoro(_dir));
    }

    public void ChargeFire(Vector2 _dir, int SplatterCount)
    {
        defaultStat.targetLayer |= (1 << LayerMask.NameToLayer("Player"));
        splatterStat.leftCount = SplatterCount;
        StartCoroutine(infoLaserCoro(_dir));
    }

    private bool isCastInfoLaser = false;
    private void Update()
    {
        if (isCastInfoLaser)
        {
            CastInfoLaser();
        }
    }

    #region 튕김 즉발
    //private void CastInfoLaser()
    //{
    //    Color startColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 1f);
    //    infoLineRenderer.startColor = startColor;
    //    infoLineRenderer.endColor = startColor;

    //    int reflections = 0;
    //    Vector2 calcPos = this.transform.position; ;
    //    Vector2 direction = this.transform.right; // 총의 전방으로 설정
    //    Vector3 originalFirePointPosition = calcPos;

    //    infoLineRenderer.positionCount = 1;
    //    infoLineRenderer.SetPosition(0, originalFirePointPosition);

    //    while (reflections <= 5)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(calcPos, direction, Mathf.Infinity, defaultStat.targetLayer);

    //        if (hit.collider)
    //        {
    //            // 충돌 지점에 레이저 선 추가
    //            infoLineRenderer.positionCount++;
    //            infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, hit.point);

    //            // 벽에 충돌한 경우 반사
    //            direction = Vector2.Reflect(direction, hit.normal);

    //            reflections++;

    //            // 레이저 반사 후 다시 발사 지점 설정
    //            calcPos = hit.point + direction * 0.01f;
    //        }
    //        else
    //        {
    //            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //            infoLineRenderer.positionCount++;
    //            infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, calcPos + direction * 100f);
    //            break;
    //        }
    //    }
    //}

    //private IEnumerator infoLaserCoro(Vector2 _dir)
    //{
    //    dir = _dir;
    //    isCastInfoLaser = true;

    //    yield return new WaitForSeconds(fireWaitTime);
    //    isCastInfoLaser = false;
    //    Color endColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 0f);
    //    infoLineRenderer.startColor = endColor;
    //    infoLineRenderer.endColor = endColor;
    //    StartCoroutine(laserCoro(_dir));
    //}

    //private IEnumerator laserCoro(Vector2 _dir)
    //{
    //    Color startColor = new Color(255, 255, 255, 1f);
    //    laserLineRenderer.startColor = startColor;
    //    laserLineRenderer.endColor = startColor;

    //    dir = _dir;

    //    laserLineRenderer.positionCount = 1;
    //    laserLineRenderer.SetPosition(0, this.transform.position);

    //    Vector2 reflectPoint = this.transform.position;
    //    while (splatterStat.leftCount > 0)
    //    {

    //        RaycastHit2D hit = Physics2D.Raycast(reflectPoint, dir, Mathf.Infinity, defaultStat.targetLayer);

    //        if (hit.collider)
    //        {
    //            CObj obj = hit.collider.transform.GetComponent<CObj>();

    //            if (obj)
    //            {
    //                obj.Hit(defaultStat.dmg, _dir * knockBackPower);
    //                Debug.Log("앍!");
    //            }

    //            // 충돌 지점에 레이저 선 추가
    //            laserLineRenderer.positionCount++;
    //            laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, hit.point);

    //            // 벽에 충돌한 경우 반사
    //            dir = Vector2.Reflect(dir, hit.normal);

    //            // 레이저 반사 후 다시 발사 지점 설정
    //            reflectPoint = hit.point + (Vector2)dir * 0.01f;
    //            splatterStat.leftCount--;
    //        }
    //        else
    //        {
    //            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //            laserLineRenderer.positionCount++;
    //            laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, (reflectPoint + dir * 100f));
    //        }
    //    }
    //    yield return new WaitForSeconds(fireWaitTime);
    //    float timer = 1f;
    //    while (timer >= 0f)
    //    {
    //        timer -= Time.deltaTime * 5f;
    //        Color endColor = new Color(255, 255, 255, timer);
    //        laserLineRenderer.startColor = endColor;
    //        laserLineRenderer.endColor = endColor;
    //        yield return null;
    //    }
    //    Destroy(this.gameObject);
    //}
    #endregion



    #region 튕김당 딜레이 몹 충돌 인식X

    private void CastInfoLaser()
    {
        Color startColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 1f);
        infoLineRenderer.startColor = startColor;
        infoLineRenderer.endColor = startColor;

        infoLineRenderer.positionCount = 1;
        infoLineRenderer.SetPosition(0, this.transform.position);

        Vector2 reflectPoint = this.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, Mathf.Infinity, wallLayer);

        if (hit.collider)
        {
            // 충돌 지점에 레이저 선 추가
            infoLineRenderer.positionCount++;
            infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, hit.point);
        }
        else
        {
            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
            infoLineRenderer.positionCount++;
            infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, (reflectPoint + dir * 100f));
        }

    }

    private IEnumerator infoLaserCoro(Vector2 _dir)
    {
        dir = _dir;
        isCastInfoLaser = true;

        yield return new WaitForSeconds(fireWaitTime);
        isCastInfoLaser = false;
        Color endColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 0f);
        infoLineRenderer.startColor = endColor;
        infoLineRenderer.endColor = endColor;
        StartCoroutine(laserCoro(_dir));
    }

    private IEnumerator laserCoro(Vector2 _dir)
    {
        Color startColor = new Color(255, 255, 255, 1f);
        laserLineRenderer.startColor = startColor;
        laserLineRenderer.endColor = startColor;

        dir = _dir;

        laserLineRenderer.positionCount = 1;
        laserLineRenderer.SetPosition(0, this.transform.position);

        laserLineRenderer.startWidth = size;
        laserLineRenderer.endWidth = size;

        Vector2 reflectPoint = this.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, Mathf.Infinity, wallLayer) ;
        //defaultStat.targetLayer
        if (hit.collider)
        {
            //CObj obj = hit.collider.transform.GetComponent<CObj>();
            //if (obj)
            //{
            //    obj.Hit(defaultStat.dmg, _dir * knockBackPower);
            //}
            // 충돌 지점에 레이저 선 추가
            laserLineRenderer.positionCount++;
            laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, hit.point);

            // 벽에 충돌한 경우 반사
            dir = Vector2.Reflect(dir, hit.normal);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(reflectPoint, laserLineRenderer.startWidth , (hit.point - reflectPoint).normalized, Vector2.Distance(reflectPoint, hit.point), defaultStat.targetLayer); 
            for (int i = 0; i < hits.Length; i++)
            {
                CObj obj = hits[i].collider.gameObject.GetComponent<CObj>();
                if (obj)
                {
                    obj.Hit(defaultStat.dmg, _dir * knockBackPower);
                }
            }

            // 레이저 반사 후 다시 발사 지점 설정
            reflectPoint = hit.point + (Vector2)dir * 0.01f;
            splatterStat.leftCount--;
            if (splatterStat.leftCount > 0)
            {
                GameObject newLaserGo = Instantiate(this.gameObject, reflectPoint, Quaternion.identity);
                newLaserGo.GetComponent<Bullet_Laser>().ChargeFire(dir, splatterStat.leftCount);
            }
        }
        else
        {
            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
            laserLineRenderer.positionCount++;
            laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, (reflectPoint + dir * 100f));
        }

        yield return new WaitForSeconds(fireWaitTime);
        float timer = 1f;
        while (timer >= 0f)
        {
            timer -= Time.deltaTime * 5f;
            Color endColor = new Color(255, 255, 255, timer);
            laserLineRenderer.startColor = endColor;
            laserLineRenderer.endColor = endColor;
            yield return null;
        }
        Destroy(this.gameObject);
    }
    #endregion


    #region 튕김당 딜레이

    //private void CastInfoLaser()
    //{
    //    Color startColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 1f);
    //    infoLineRenderer.startColor = startColor;
    //    infoLineRenderer.endColor = startColor;

    //    infoLineRenderer.positionCount = 1;
    //    infoLineRenderer.SetPosition(0, this.transform.position);

    //    Vector2 reflectPoint = this.transform.position;
    //    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, Mathf.Infinity, defaultStat.targetLayer);

    //    if (hit.collider)
    //    {
    //        // 충돌 지점에 레이저 선 추가
    //        infoLineRenderer.positionCount++;
    //        infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, hit.point);
    //    }
    //    else
    //    {
    //        // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //        infoLineRenderer.positionCount++;
    //        infoLineRenderer.SetPosition(infoLineRenderer.positionCount - 1, (reflectPoint + dir * 100f));
    //    }

    //}

    //private IEnumerator infoLaserCoro(Vector2 _dir)
    //{
    //    dir = _dir;
    //    isCastInfoLaser = true;

    //    yield return new WaitForSeconds(fireWaitTime);
    //    isCastInfoLaser = false;
    //    Color endColor = new Color(infoLineRenderer.colorGradient.colorKeys[0].color.r, infoLineRenderer.colorGradient.colorKeys[0].color.g, infoLineRenderer.colorGradient.colorKeys[0].color.b, 0f);
    //    infoLineRenderer.startColor = endColor;
    //    infoLineRenderer.endColor = endColor;
    //    StartCoroutine(laserCoro(_dir));
    //}

    //private IEnumerator laserCoro(Vector2 _dir)
    //{
    //    Color startColor = new Color(255, 255, 255, 1f);
    //    laserLineRenderer.startColor = startColor;
    //    laserLineRenderer.endColor = startColor;

    //    dir = _dir;

    //    laserLineRenderer.positionCount = 1;
    //    laserLineRenderer.SetPosition(0, this.transform.position);

    //    Vector2 reflectPoint = this.transform.position;

    //    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, Mathf.Infinity, defaultStat.targetLayer);

    //    if (hit.collider)
    //    {
    //        CObj obj = hit.collider.transform.GetComponent<CObj>();

    //        if (obj)
    //        {
    //            obj.Hit(defaultStat.dmg, _dir * knockBackPower);
    //            Debug.Log("앍!");
    //        }

    //        // 충돌 지점에 레이저 선 추가
    //        laserLineRenderer.positionCount++;
    //        laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, hit.point);

    //        // 벽에 충돌한 경우 반사
    //        dir = Vector2.Reflect(dir, hit.normal);

    //        // 레이저 반사 후 다시 발사 지점 설정
    //        reflectPoint = hit.point + (Vector2)dir * 0.01f;
    //        splatterStat.leftCount--;
    //        if (splatterStat.leftCount > 0)
    //        {
    //            //yield return new WaitForSeconds(fireWaitTime);
    //            GameObject newLaserGo = Instantiate(this.gameObject, reflectPoint, Quaternion.identity);
    //            newLaserGo.GetComponent<Bullet_Laser>().Fire(dir, splatterStat.leftCount, 0f);
    //        }
    //    }
    //    else
    //    {
    //        // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //        laserLineRenderer.positionCount++;
    //        laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, (reflectPoint + dir * 100f));
    //    }
    //    yield return new WaitForSeconds(fireWaitTime);
    //    float timer = 1f;
    //    while (timer >= 0f)
    //    {
    //        timer -= Time.deltaTime * 5f;
    //        Color endColor = new Color(255, 255, 255, timer);
    //        laserLineRenderer.startColor = endColor;
    //        laserLineRenderer.endColor = endColor;
    //        yield return null;
    //    }
    //    Destroy(this.gameObject);
    //}
    #endregion
    #region old code
    //private void FireLaser()
    //{
    //    firePoint.transform.position = this.transform.position;
    //    firePoint.transform.rotation = this.transform.rotation;
    //    int reflections = 0;
    //    Vector2 direction = firePoint.right; // 총의 전방으로 설정

    //    lineRenderer.positionCount = 1;
    //    lineRenderer.SetPosition(0, firePoint.position);

    //    while (reflections <= maxReflections)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, Mathf.Infinity, wallLayer);

    //        if (hit.collider)
    //        {
    //            // 충돌 지점에 레이저 선 추가
    //            lineRenderer.positionCount++;
    //            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

    //            // 벽에 충돌한 경우 반사
    //            direction = Vector2.Reflect(direction, hit.normal);

    //            reflections++;

    //            // 레이저 반사 후 다시 발사 지점 설정
    //            firePoint.position = hit.point + direction * 0.01f;
    //        }
    //        else
    //        {
    //            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //            lineRenderer.positionCount++;
    //            lineRenderer.SetPosition(lineRenderer.positionCount - 1, (firePoint.position + (Vector3)direction * 100f));

    //            break;
    //        }
    //    }
    //}

    //private IEnumerator laserCoro2()
    //{
    //    Color startColor = new Color(255, 255, 255, 1f);
    //    lineRenderer.startColor = startColor;
    //    lineRenderer.endColor = startColor;
    //    firePoint.transform.position = this.transform.position;
    //    firePoint.transform.rotation = this.transform.rotation;
    //    int reflections = 0;
    //    Vector2 direction = firePoint.right; // 총의 전방으로 설정

    //    lineRenderer.positionCount = 1;
    //    lineRenderer.SetPosition(0, firePoint.position);

    //    while (reflections <= maxReflections)
    //    {
    //        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, Mathf.Infinity, wallLayer);

    //        if (hit.collider)
    //        {
    //            // 충돌 지점에 레이저 선 추가
    //            lineRenderer.positionCount++;
    //            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

    //            // 벽에 충돌한 경우 반사
    //            direction = Vector2.Reflect(direction, hit.normal);

    //            reflections++;

    //            // 레이저 반사 후 다시 발사 지점 설정
    //            firePoint.position = hit.point + direction * 0.01f;
    //            yield return new WaitForSeconds(0.2f);
    //        }
    //        else
    //        {
    //            // 벽에 충돌하지 않으면 레이저 선 그리기 종료
    //            lineRenderer.positionCount++;
    //            //lineRenderer.SetPosition(lineRenderer.positionCount - 1, (firePoint.position + (Vector3)direction * 100f));

    //            break;
    //        }
    //    }
    //    yield return new WaitForSeconds(1f);
    //    //Color endColor = new Color(255, 255, 255, 0f);
    //    //lineRenderer.startColor = endColor;
    //    //lineRenderer.endColor = endColor;
    //    Debug.Log("챈지드");
    //}
    #endregion
}
