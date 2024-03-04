using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using DG.Tweening;
using Structs;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;

public class Bullet_Rain : Bullet
{
    public GameObject bulletPrefab;
    public Light2D light2D;
    private float time = 3f;
    private Suckable suckable;

    private void Awake()
    {
        suckable = GetComponent<Suckable>();
        suckable.OnSucked += OnSuckedEvent;
    }

    public void Fire(Vector3 targetPos, float _time)
    {
        light2D.intensity = 0f;
        time = _time;
        srdr.color = Color.clear;
        col.enabled = false;
        StartCoroutine(lightFadeCoro());
        srdr.DOColor(Color.white, 1f).OnComplete(() => { MoveBullet(targetPos); });
    }

    IEnumerator lightFadeCoro()
    {
        float fadeTimer = 0f;
        while(fadeTimer < 1f)
        {
            fadeTimer += Time.deltaTime;
            light2D.intensity = fadeTimer;
            yield return null;
        }
    }

    private void MoveBullet(Vector3 targetPos)
    {
        col.enabled = true;
        this.transform.DOMove(targetPos, time).SetEase(Ease.InCubic).SetDelay(1f).OnComplete(() => { Explosion(); });
        srdr.DOColor(Color.red, 0.2f).SetLoops(-1).SetDelay(1 + time / 3f * 2f);
    }

    private void Explosion()
    {
        //SoundManager.Instance.PlayTempSound("Boss_Demo_Atk_Explosion", this.transform.position, 1f, 0.8f, 1f);

        for (int i = 0; i < 8; i++)
        {
            float angleRadians = Mathf.Deg2Rad * 360 / 8 * i;
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));

            GameObject bullet = PoolingManager.Instance.LentalObj("Bullet_Enemy", 1);
            bullet.SetActive(false);
            bullet.transform.position = this.transform.position;
            bullet.SetActive(true);

            bullet.GetComponent<Bullet>().Fire(direction, 1, 200f, 0.5f);
        }

        Resetting();
        PoolingManager.Instance.ReturnObj(this.gameObject);
    }

    public override void Resetting()
    {
        base.Resetting();
        srdr.DOKill();
        this.transform.DOKill();
        srdr.color = Color.white;
    }

    private void OnSuckedEvent()
    {
        Resetting();
        IngameController.Instance.Player.inventroy.bulletCount.Value++;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & defaultStat.targetLayer) != 0)
        {
            CObj obj = collision.transform.GetComponent<CObj>();

            if (obj)
            {
                Vector2 normal = collision.GetContact(0).point;
                Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;

                HitInfo hitinfo = obj.Hit(defaultStat.dmg, normalDir);

                Resetting();
                PoolingManager.Instance.ReturnObj(this.gameObject);
            }
        }
    }
}
