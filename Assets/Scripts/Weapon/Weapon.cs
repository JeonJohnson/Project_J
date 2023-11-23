using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public float fireRate;
        public int bulletCount;
    }

    public Stat stat;
    public float fireTimer;
    public CObj owner;

    public SpriteRenderer weaponSprite;
    public Transform firePos;

    [Header("TEST FUNCS")]
    public GameObject testBulletPrefab;


    public virtual void Init(CObj _owner)
    {
        owner = _owner;
        fireTimer = stat.fireRate;
    }

    public abstract void Fire();
}
