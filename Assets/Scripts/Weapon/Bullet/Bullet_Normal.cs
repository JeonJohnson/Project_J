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
