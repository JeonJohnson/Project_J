using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

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
        public Transform suckedTr;
    }

    [Header("Options")]

    [Tooltip("���� ��������")]
    public SuckedStat suckedStat;

    [Tooltip("���� ����")]
    public BulletState curState;

    [Header("Default Components")]
    public Collider2D col;
    private Rigidbody2D rb;
    public SpriteRenderer srdr;
    private Projectile projectile;
    private bool isDistanceLimit;

    private Color defColor;
    private Vector2 defScale;

    public Action OnSucked;

    private void Awake()
    {
        defColor = srdr.color;
        defScale = this.transform.localScale;

        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Sucked(Transform _suckedTr)
    {
        projectile = this.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            if (projectile.DistanceLimit) isDistanceLimit = true;
            else isDistanceLimit = false;
            projectile.DistanceLimit = false;
            projectile.enabled = false;
        }
        if (rb != null) rb.velocity = Vector3.zero;

        srdr.color = srdr.color * 5f;
        curState = BulletState.SuckWait;
        suckedStat.suckWaitRandTime = UnityEngine.Random.Range(0.1f, 0.25f);

        col.enabled = false;
        suckedStat.suckedTr = _suckedTr;

        StartCoroutine(SuckWaitCor());
    }

    public IEnumerator SuckWaitCor()
    {
        yield return new WaitForSeconds(suckedStat.suckWaitRandTime);

        suckedStat.suckingRandTime = UnityEngine.Random.Range(0.15f, 0.35f);
        suckedStat.suckStartPos = transform.position;

        curState = BulletState.Sucking;
        suckedStat.suckingTimeRatio = 0f;

        while (suckedStat.suckingTimeRatio <= 1f)
        {
            suckedStat.suckingTimeRatio += Time.deltaTime / suckedStat.suckingRandTime;
            transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.suckedTr.position, suckedStat.suckingTimeRatio);
            srdr.color = new Color(srdr.color.r, srdr.color.g, srdr.color.b, 1 - suckedStat.suckingTimeRatio * 2f);
            this.transform.localScale = new Vector2(1 - suckedStat.suckingTimeRatio, 1 - suckedStat.suckingTimeRatio);
            yield return null;
        }

        OnSucked?.Invoke();

        //�����ϱ�
        Resetting();

        Bullet bullet = GetComponent<Bullet>();
        if (bullet) bullet.Resetting();
        if(GetComponent<Bullet>()) { }
        Destroy(this.gameObject);
    }

    public void Resetting()
    {
        if (projectile != null)
        {
            projectile.DistanceLimit = isDistanceLimit;
            projectile.enabled = true;
        }
        col.enabled = true;
        this.transform.localScale = defScale;
        srdr.color = Color.white;
        curState = BulletState.Fire;
    }
}
