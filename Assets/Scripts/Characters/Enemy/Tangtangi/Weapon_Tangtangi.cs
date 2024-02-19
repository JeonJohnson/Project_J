using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon_Tangtangi : Weapon
{
    [SerializeField] Animator muzzleFlashAnim;
    [SerializeField] Light2D muzzlFlashLight;
    private float flashTimer = 0f;
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

        flashTimer -= Time.deltaTime;
        flashTimer = Mathf.Clamp(flashTimer, 0f, 1f);

        if(flashTimer > 0f)
        {
            muzzleFlashAnim.gameObject.SetActive(true);
            muzzlFlashLight.gameObject.SetActive(true);
        }
        else
        {
            muzzleFlashAnim.gameObject.SetActive(false);
            muzzlFlashLight.gameObject.SetActive(false);
        }
    }

    public override void Fire()
    {
        flashTimer = 0.25f;
        float rnd = Random.Range(-owner.status.spread * 0.5f, owner.status.spread * 0.5f);
        Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
        Quaternion dispersionRotation = Quaternion.Euler(0f, 0f, rnd);
        Vector2 rndDir = rndRot * firePos.up;
        rndDir.Normalize();

        GameObject go = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);
        go.SetActive(false);
        go.transform.position = firePos.position;
        go.SetActive(true);

        //근희임시추가
        StageManager.Instance?.AddBullet(go);
		//근희임시추가

		go.GetComponent<Bullet>().Fire(rndDir, owner.status.bulletSplatterCount, owner.status.bulletSpeed, owner.status.bulletSize, 1);

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
