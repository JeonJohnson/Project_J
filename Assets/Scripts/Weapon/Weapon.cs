using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    public float fireTimer;

    public SpriteRenderer weaponSprite;
    public Transform firePos;

    [Header("TEST FUNCS")]
    public GameObject testBulletPrefab;
    public GameObject testLaserBulletPrefab;


    public virtual void Init(CObj _owner)
    {

    }

    public abstract void Fire();
}
