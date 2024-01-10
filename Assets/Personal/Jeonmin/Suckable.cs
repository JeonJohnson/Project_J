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
        public MoreMountains.TopDownEngine.Weapon suckedWeapon;
    }

    [Header("Options")]
    [Tooltip("흡수가 가능한가에 대한 옵션")]
    public SuckedOption suckedOption;

    [Tooltip("상태 변수값들")]
    public SuckedStat suckedStat;

    [Tooltip("현재 상태")]
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
                        //jar마우스쪽에서 Sucking 상태인 bullet이 충돌되면 bulletCnt 증가 하기?

                        Resetting();
                        projectile.enabled = true;
                        //리셋하기
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
