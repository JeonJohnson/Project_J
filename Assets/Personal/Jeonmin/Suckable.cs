using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Suckable : MonoBehaviour
{
    // sucked option 
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
        public MoreMountains.TopDownEngine.Weapon suckedWeapon;
    }

    [Header("Options")]
    [Tooltip("����� �����Ѱ��� ���� �ɼ�")]
    public SuckedOption suckedOption;

    [Tooltip("���� ��������")]
    public SuckedStat suckedStat;

    [Tooltip("���� ����")]
    public BulletState curState;

    [Header("Default Components")]
    private BoxCollider2D col;
    private Rigidbody2D rb;
    public SpriteRenderer srdr;
    private Projectile projectile;

    private Color defColor;

    private void Awake()
    {
        defColor = srdr.color;
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Sucked(MoreMountains.TopDownEngine.Weapon _suckedWeapon)
    {
        projectile = this.gameObject.GetComponent<Projectile>();
        projectile.enabled = false;

        srdr.color = srdr.color * 1.3f;
        curState = BulletState.SuckWait;
        suckedStat.suckWaitRandTime = UnityEngine.Random.Range(0.1f, 0.25f);

        rb.velocity = Vector3.zero;
        col.enabled = false;
        suckedStat.suckedWeapon = _suckedWeapon;

        //StartCoroutine(SuckWaitCor());
    }

    public IEnumerator SuckWaitCor()
    {
        yield return new WaitForSeconds(suckedStat.suckWaitRandTime);

        suckedStat.suckingRandTime = UnityEngine.Random.Range(0.15f, 0.35f);
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

                    transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.suckedWeapon.transform.position, suckedStat.suckingTimeRatio);

                    if (suckedStat.suckingTimeRatio >= 1f)
                    {
                        //jar���콺�ʿ��� Sucking ������ bullet�� �浹�Ǹ� bulletCnt ���� �ϱ�?

                        Resetting();
                        projectile.enabled = true;
                        //�����ϱ�
                        this.gameObject.SetActive(false);
                    }
                }
                break;
            default:
                break;
        }
    }

    public void Resetting()
    {
        col.enabled = true;
        srdr.color = Color.white;
        curState = BulletState.Fire;
    }
}	
