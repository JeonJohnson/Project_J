using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class Suckable : MonoBehaviour, IPoolable
{
    // sucked option 
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
        public Transform suckedTr;
    }

    [Header("Options")]

    [Tooltip("상태 변수값들")]
    public SuckedStat suckedStat;

    [Tooltip("현재 상태")]
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
    public AudioSource aus;

    private void Awake()
    {
        defColor = srdr.color;
        defScale = this.transform.localScale;

        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if(aus == null)
        {
            aus = this.gameObject.AddComponent<AudioSource>();
            aus.spatialize = false;
        }
    }

    public void Sucked(Transform _suckedTr)
    {
        if (curState != BulletState.Fire) return;
        curState = BulletState.SuckWait;

        SoundManager.Instance?.PlaySound("Player_Sucked", aus, 0.05f , 1f, true);
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

        Debug.Log("흡수됨");
        PoolingManager.Instance.ReturnObj(this.gameObject);
    }

    public void PoolableInit()
    {
        col.enabled = true;
        this.transform.localScale = defScale;
        srdr.color = defColor;
        curState = BulletState.Fire;
    }

    public void PoolableReset()
    {
        StopAllCoroutines();
        if (projectile != null)
        {
            projectile.DistanceLimit = isDistanceLimit;
            projectile.enabled = true;
        }
        //Debug.Log("총알 리셋");
        col.enabled = true;
        this.transform.localScale = defScale;
        srdr.color = defColor;
        curState = BulletState.Fire;
    }

    public void OnDisable()
    {
        Debug.LogWarning("sex1");
    }

    public void OnDestroy()
    {
        Debug.LogWarning("sex2");
    }
}
