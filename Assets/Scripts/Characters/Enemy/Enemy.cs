using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : CObj, IPoolable
{
    public float distToTarget;
    public Player target;
    public NavMeshAgent agent;
    public EnemyStatus status;
    [HideInInspector] public Vector3 spriteDir;

    public virtual void Awake()
    {
        Initialize();
        if (target == null) target = IngameController.Instance.Player;
    }

    protected abstract void Initialize();


    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }

    public void PoolableInit()
    {
        Initialize();
    }

    public void PoolableReset()
    {

    }

    public float DistToTarget
    {
        get
        {
            if (target == null) target = IngameController.Instance.Player;
            distToTarget = Vector3.Distance(target.transform.position, this.transform.position);
            return distToTarget;
        }
    }
}
