using System;
using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

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
    public virtual void SetLeftCount(int cnt)
    {
        splatterStat.leftCount = cnt;
        //splatterStat.hitCountTmp.text = cnt.ToString();
    }

    public virtual void Fire(Vector2 dir, int _SplatterCount = 1, float moveSpd = 200f, float bulletSize = 1f, int dmg = 1)
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
