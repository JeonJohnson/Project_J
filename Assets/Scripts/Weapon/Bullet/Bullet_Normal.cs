using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet_Normal : Bullet
{
    private Light2D light2D;
    public Color maxColor;
    public Color minColor;

    private Color defColor;
    private Vector3 defScale;

    public override void Fire(Vector2 dir, int _SplatterCount = 1, float moveSpd = 200f, float bulletSize = 1f, int dmg = 1)
    {
        defaultStat.dmg = dmg;
        curState = BulletState.Fire;
        this.transform.localScale = new Vector2(bulletSize, bulletSize);

        rb.AddForce(dir * moveSpd, ForceMode2D.Force);
        SetLeftCount(_SplatterCount);
    }

    public override void Sucked(Player _player)
    {
        srdr.color = srdr.color * 2f;

        curState = BulletState.SuckWait;

        suckedStat.player = _player;
        suckedStat.suckWaitRandTime = Random.Range(0.1f, 0.25f);

        rb.velocity = Vector3.zero;

        col.enabled = false;

        StartCoroutine(SuckWaitCor());
    }

    public IEnumerator SuckWaitCor()
    {
        yield return new WaitForSeconds(suckedStat.suckWaitRandTime);

        suckedStat.suckingRandTime = Random.Range(0.15f, 0.35f);
        suckedStat.suckStartPos = transform.position;

        curState = BulletState.Sucking;
        suckedStat.suckingTimeRatio = 0f;

        while (suckedStat.suckingTimeRatio <= 1f)
        {
            suckedStat.suckingTimeRatio += Time.deltaTime / suckedStat.suckingRandTime;
            transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.player.curWeapon.firePos.position, suckedStat.suckingTimeRatio);
            srdr.color = new Color(srdr.color.r, srdr.color.g, srdr.color.b, 1 - suckedStat.suckingTimeRatio * 2f);
            this.transform.localScale = new Vector2(1 - suckedStat.suckingTimeRatio, 1 - suckedStat.suckingTimeRatio);
            yield return null;
        }

        Resetting();
        //∏Æº¬«œ±‚
        Destroy(this.gameObject);
    }

    public override void Resetting()
    {
        col.enabled = true;
        srdr.color = defColor;
        curState = BulletState.Fire;
        suckedStat.player = null;

        splatterStat.leftCount = splatterStat.maxCount;
        this.transform.localScale = defScale;
    }

    private void Awake()
    {
        FindDefaultComps();
        light2D = GetComponent<Light2D>();

        defaultStat.aliveTime = 30;
        defColor = srdr.color;
        defScale = this.transform.localScale;
    }

    public override void SetLeftCount(int cnt)
    {
        base.SetLeftCount(cnt);
    }

    void Update()
    {
    }


    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Splatter ø…º« ¿œ∂ß∏∏ »£√‚µ…µÌ
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            SetLeftCount(splatterStat.leftCount - 1);

            if (splatterStat.leftCount < 0)
            {
                GenerateSmoke();
                Resetting();
                Destroy(this.gameObject);
            }

            Vector2 normal = collision.GetContact(0).point;
            Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;


            // ∆®±Ë πÊ«‚ º≥¡§
            splatterStat.bulletDir = normalDir;
        }
        else if (((1 << collision.gameObject.layer) & defaultStat.targetLayer) != 0)
        {
            CObj obj = collision.transform.GetComponent<CObj>();

            if (obj)
            {
                Vector2 normal = collision.GetContact(0).point;
                Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;

                HitInfo hitinfo = obj.Hit(defaultStat.dmg, normalDir);

                if (hitinfo.isDurable)
                {
                    SetLeftCount(splatterStat.leftCount - 1);

                    if (splatterStat.leftCount < 0)
                    {
                        GenerateSmoke();
                        Resetting();
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        GameObject particle = PoolingManager.Instance.LentalObj("Effect_Smoke_04");
                        particle.transform.position = this.transform.position;
                    }
                }
                else
                {
                    GameObject particle = PoolingManager.Instance.LentalObj("Effect_Hit_" + Random.Range(0, 3).ToString());
                    particle.transform.position = this.transform.position + -(Vector3)normalDir * 1.5f;
                    Resetting();
                    Destroy(this.gameObject);
                }
            }
        }

        if (splatterStat.leftCount < 0)
        {
            Resetting();
            Debug.Log("æÃ º˝¿⁄¥Ÿµ  ∆ƒ±´");
            Destroy(this.gameObject);
        }
    }

    private void GenerateSmoke()
    {
        GameObject smoke = PoolingManager.Instance.LentalObj("Effect_Smoke_01");
        smoke.transform.position = this.transform.position;
    }
}
