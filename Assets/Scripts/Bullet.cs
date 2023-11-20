using System;
using System.Collections;
using System.Collections.Generic;

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
    Fire, //�߻縸 ���� ��
    SuckWait, //Suction ���۵ǰ� ��� ��ġ ������ ��
    Sucking, //�� �� �׾Ƹ� �Ա������� ������ ��
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
    public float suckWaitRandTime; //���ߴ� �ð� (�����ٲ���)
    public float suckingRandTime; //���߰��� �������� �ɸ��� �ð�. (���� �ٲ���)
    public float suckingTimeRatio;

    public Vector2 suckStartPos;
    public Player player;
}

public class Bullet : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("����� �����Ѱ��� ���� �ɼ�")]
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

    public void Fire(Vector2 dir)
    {
        curState = BulletState.Fire;

        rb.AddForce(dir * defaultStat.moveSpd, ForceMode2D.Force);
        SetLeftCount(splatterStat.maxCount);
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
        //�浹�Ҷ����� splatterCount ���
        //0 ���ϵǸ� �����ֱ�
        if (splatterStat.leftCount <= 0)
        {
            Resetting();
            Debug.Log("�� ���ڴٵ� �ı�");
            Destroy(this.gameObject);
            //Ǯ�����ٰ� �����ֱ�
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

                    transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.player.curWeapon.jarMouthTr.position, suckedStat.suckingTimeRatio);

                    if (suckedStat.suckingTimeRatio >= 1f)
                    {
                        //jar���콺�ʿ��� Sucking ������ bullet�� �浹�Ǹ� bulletCnt ���� �ϱ�?

                        Resetting();
                        //�����ϱ�
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
        //Splatter �ɼ� �϶��� ȣ��ɵ�
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("�� ����");

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


            // ƨ�� ���� ����
            splatterStat.bulletDir = normalDir;
        }
        else if (((1 << collision.gameObject.layer) & defaultStat.targetLayer) != 0)
        {
            CObj obj = collision.transform.GetComponent<CObj>();

            if (obj)
            {
                Vector2 normal = collision.GetContact(0).point;
                Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;
                obj.Hit(defaultStat.dmg, normalDir);
                Resetting();
                Destroy(this.gameObject);
            }
        }

    }

    private void GenerateSmoke()
    {
        //GameObject smoke = PoolingManager.Instance.LentalObj("BulletHitSmoke");
        //smoke.transform.position = this.transform.position;
    }
}