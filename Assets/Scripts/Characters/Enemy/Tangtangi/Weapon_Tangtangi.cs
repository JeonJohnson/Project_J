using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Tangtangi : Weapon
{
    private Vector3 originPos;
    private Tangtangi owner;
    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Tangtangi)_owner;
    }

    private void Awake()
    {
        originPos = this.transform.localPosition;
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

        GameObject go = Instantiate(testBulletPrefab, firePos.transform.position, Quaternion.identity);
        go.GetComponent<Bullet>().defaultStat.moveSpd = 200;
        go.GetComponent<Bullet>().Fire(rndDir);

        Vector3 weaponRecoilDir;
        weaponRecoilDir = this.transform.up;

        if (owner.spriteDir == Vector3.left) weaponRecoilDir.x *= -1;

        transform.DOLocalMove(originPos - weaponRecoilDir * 0.1f, 0.03f).OnComplete(() => { RecoilEffect(); });
    }
    public void RecoilEffect()
    {
        transform.DOLocalMove(originPos, 0.12f);
    }

}
