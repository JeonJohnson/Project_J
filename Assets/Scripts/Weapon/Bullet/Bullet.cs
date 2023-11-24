using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Random = UnityEngine.Random;

[Serializable]
public struct BulletStat
{
    public int dmg;
    public float moveSpd;
    public float aliveTime;
    public LayerMask targetLayer;
}


public enum BulletState
{
    Fire, //발사만 됐을 때
    SuckWait, //Suction 시작되고 잠시 위치 멈췄을 때
    Sucking, //그 후 항아리 입구쪽으로 딸려올 때
    End
}

[Serializable]
public struct SplatterStat
{
    public Vector2 bulletDir;
    public int maxCount;
    public int leftCount;
    public TextMeshProUGUI hitCountTmp;
}

public enum SuckedOption
{
    None,
    Sucked
}
[Serializable]
public struct SuckedStat
{
    public float suckWaitRandTime; //멈추는 시간 (랜덤줄꺼임)
    public float suckingRandTime; //멈추고나서 빨려들어갈때 걸리는 시간. (랜덤 줄꺼임)
    public float suckingTimeRatio;

    public Vector2 suckStartPos;
    public Player player;
}

public class Bullet : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("흡수가 가능한가에 대한 옵션")]
    public SuckedOption suckedOption;


    [Header("Status")]
    public BulletStat defaultStat;
    public SplatterStat splatterStat;
    public SuckedStat suckedStat;

    [Header("State")]
    public BulletState curState;


    [Header("Default Components")]
    public CircleCollider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer srdr;


    private void FindDefaultComps()
    {
        if (col == null)
        {
            col = Funcs.IsComponentExistInFamily<CircleCollider2D>(this.gameObject);
        }

        if (!rb)
        {
            rb = Funcs.IsComponentExistInFamily<Rigidbody2D>(gameObject);
        }

        if (!srdr)
        {
            srdr = Funcs.IsComponentExistInFamily<SpriteRenderer>(this.gameObject);
        }
    }
    private void SetLeftCount(int cnt)
    {
        splatterStat.leftCount = cnt;
        //splatterStat.hitCountTmp.text = cnt.ToString();
    }

    public void Fire(Vector2 dir, int _SplatterCount = 1)
    {
        curState = BulletState.Fire;

        rb.AddForce(dir * defaultStat.moveSpd, ForceMode2D.Force);
        SetLeftCount(_SplatterCount); 
    }

    public void Sucked(Player _player)
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


    public void SplatterUpdate()
    {
        //충돌할때마다 splatterCount 깎고
        //0 이하되면 없애주기
        if (splatterStat.leftCount <= 0)
        {
            Resetting();
            Debug.Log("앍 숫자다됨 파괴");
            Destroy(this.gameObject);
            //풀링에다가 돌려주기
        }
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

	private void Resetting()
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
        SplatterUpdate();
        
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

	}

    private void GenerateSmoke()
    {
        //GameObject smoke = PoolingManager.Instance.LentalObj("BulletHitSmoke");
        //smoke.transform.position = this.transform.position;
    }
}
