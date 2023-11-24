using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Bangtani : Weapon
{
    private Bangtani owner;
    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Bangtani)_owner;
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
        Quaternion dispersionRotation = Quaternion.Euler(0f, 0f, rnd);
        Vector2 rndDir = rndRot * firePos.up;
        rndDir.Normalize();

        //Debug.Log(Quaternion.LookRotation(Vector3.forward, dir));

        GameObject go = Instantiate(testBulletPrefab);
        go.transform.position = firePos.transform.position;
        go.GetComponent<Bullet>().defaultStat.moveSpd = 200;
        go.GetComponent<Bullet>().Fire(rndDir);
    }
}
