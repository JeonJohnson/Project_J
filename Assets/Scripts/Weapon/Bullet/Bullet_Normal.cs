using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Bullet
{

    public override void Fire(Vector2 dir, int _SplatterCount = 1, float moveSpd = 200f)
    {
        curState = BulletState.Fire;

        rb.AddForce(dir * moveSpd, ForceMode2D.Force);
        SetLeftCount(_SplatterCount);
    }

    public override void Sucked(Player _player)
    {
        srdr.color = Color.black;

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
    }

    public void MoveUpdate()
    {
        switch (curState)
        {
            case BulletState.Fire:
                {
                    //transform.position += transform.up * Time.deltaTime * defaultStat.moveSpd;
                }
                break;
            case BulletState.SuckWait:
                {

                }
                break;
            case BulletState.Sucking:
                {
                    suckedStat.suckingTimeRatio += Time.deltaTime / suckedStat.suckingRandTime;

                    transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.player.curWeapon.firePos.position, suckedStat.suckingTimeRatio);

                    if (suckedStat.suckingTimeRatio >= 1f)
                    {
                        //jar마우스쪽에서 Sucking 상태인 bullet이 충돌되면 bulletCnt 증가 하기?

                        Resetting();
                        //리셋하기
                        Destroy(this.gameObject);
                    }
                }
                break;
            default:
                break;
        }
    }

    public override void Resetting()
    {
        col.enabled = true;
        srdr.color = Color.white;
        curState = BulletState.Fire;
        suckedStat.player = null;

        splatterStat.leftCount = splatterStat.maxCount;

    }

    private void Awake()
    {
        FindDefaultComps();

        defaultStat.aliveTime = 30;
    }



    void Update()
    {
        MoveUpdate();
    }


    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Splatter 옵션 일때만 호출될듯
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            --splatterStat.leftCount;
            SetLeftCount(splatterStat.leftCount);

            if (splatterStat.leftCount <= 0)
            {
                GenerateSmoke();
                Resetting();
                Destroy(this.gameObject);
            }

            Vector2 normal = collision.GetContact(0).point;
            Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;


            // 튕김 방향 설정
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
                    --splatterStat.leftCount;
                    SetLeftCount(splatterStat.leftCount);

                    if (splatterStat.leftCount <= 0)
                    {
                        GenerateSmoke();
                        Resetting();
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    Resetting();
                    Destroy(this.gameObject);
                }
            }
        }

        if (splatterStat.leftCount <= 0)
        {
            Resetting();
            Debug.Log("앍 숫자다됨 파괴");
            Destroy(this.gameObject);
        }
    }

    private void GenerateSmoke()
    {
        //GameObject smoke = PoolingManager.Instance.LentalObj("BulletHitSmoke");
        //smoke.transform.position = this.transform.position;
    }
}
