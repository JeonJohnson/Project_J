using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Hwasari : Weapon
{
    private Hwasari owner;
    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Hwasari)_owner;
    }

    private void Awake()
    {
    }

    private void Update()
    {
        Vector3 dir = owner.target.transform.position - this.transform.position;
        dir.Normalize();

        this.transform.rotation = Quaternion.LookRotation(this.transform.forward, dir);
    }

    public override void Fire()
    {
        float rnd = Random.Range(-owner.status.spread * 0.5f, owner.status.spread * 0.5f);
        Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
        Vector2 rndDir = rndRot * firePos.up;
        rndDir.Normalize();

        GameObject go = Instantiate(testBulletPrefab, firePos.transform.position, Quaternion.identity);
        go.GetComponent<Bullet>().defaultStat.moveSpd = 200;
        go.GetComponent<Bullet>().Fire(rndDir, 0, 120f, owner.status.bulletSize);
    }

    public void Fire(float additiveAngle)
    {
        Vector2 dir = Quaternion.Euler(0f, 0f, additiveAngle) * firePos.up;
        dir.Normalize();

        GameObject go = Instantiate(testBulletPrefab, firePos.transform.position, Quaternion.identity);
        go.GetComponent<Bullet>().defaultStat.moveSpd = 200;
        go.GetComponent<Bullet>().Fire(dir, 0, 120f, owner.status.bulletSize);
    }
}
