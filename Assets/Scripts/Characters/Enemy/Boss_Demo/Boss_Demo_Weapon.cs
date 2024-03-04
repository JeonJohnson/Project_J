using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Demo_Weapon : Weapon
{
    private Boss_Demo owner;
    public GameObject TestRainBulletHolder;
    public GameObject TestCrossBulletHolder;

    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Boss_Demo)_owner;
    }

    private void Awake()
    {
    }

    private void Update()
    {
       
    }

    public override void Fire()
    {
        float rnd = Random.Range(-owner.status.spread * 0.5f, owner.status.spread * 0.5f);
        Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
        Quaternion dispersionRotation = Quaternion.Euler(0f, 0f, rnd);
        Vector2 rndDir = rndRot * firePos.up;
        rndDir.Normalize();

        //Debug.Log(Quaternion.LookRotation(Vector3.forward, dir));

        GameObject go = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);
        go.SetActive(false);
        go.transform.position = firePos.position;
        go.SetActive(true);

        go.GetComponent<Bullet>().defaultStat.moveSpd = 200f;
        go.GetComponent<Bullet>().Fire(rndDir);
    }

    public void Fire(Vector2 angle, float spread, float speed, float size = 1f)
    {
        float rnd = Random.Range(-spread * 0.5f, spread * 0.5f);
        Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
        Quaternion dispersionRotation = Quaternion.Euler(0f, 0f, rnd);
        Vector2 rndDir = rndRot * angle;
        rndDir.Normalize();

        //Debug.Log(Quaternion.LookRotation(Vector3.forward, dir));

        GameObject go = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);

        go.transform.position = firePos.position;
        go.transform.localScale = new Vector2(size, size);

        go.GetComponent<Bullet>().Fire(rndDir, 4, speed, 0.5f);
    }

    public void FireRainBullet(Vector3 Pos, float time, float distance)
    {
        GameObject go = Instantiate(TestRainBulletHolder, Pos, Quaternion.identity);
        go.GetComponent<Bullet_RainHolder>().Fire(time,distance);
    }

    public void FireCrossBullet(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject go = Instantiate(TestCrossBulletHolder, firePos.transform.position, Quaternion.Euler(0, 0, angle).normalized);
    }
}
