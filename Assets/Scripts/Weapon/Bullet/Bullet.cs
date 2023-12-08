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

    [Tooltip("상태 변수값들")]
    public BulletStat defaultStat;
    public SplatterStat splatterStat;
    public SuckedStat suckedStat;

    [Tooltip("현재 상태")]
    public BulletState curState;


    [Header("Default Components")]
    public CircleCollider2D col;
    public Rigidbody2D rb;
    public SpriteRenderer srdr;

    public void FindDefaultComps()
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
    public void SetLeftCount(int cnt)
    {
        splatterStat.leftCount = cnt;
        //splatterStat.hitCountTmp.text = cnt.ToString();
    }

    public virtual void Fire(Vector2 dir, int _SplatterCount = 1, float moveSpd = 200f)
    {

    }

    public virtual void Sucked(Player _player)
    {

    }

	public virtual void Resetting()
	{

	}

	private void Awake()
	{
        FindDefaultComps();

        defaultStat.aliveTime = 30;
    }
}
