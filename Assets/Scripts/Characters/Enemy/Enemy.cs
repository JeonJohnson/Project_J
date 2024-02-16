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
    public HitInfo hitInfo;
    public List<StatusEffect> statusEffect = new List<StatusEffect>();

    public virtual void Start()
    {
        Initialize();
        if (target == null) target = IngameController.Instance.Player;
    }
    
    protected abstract void Initialize();

    public override HitInfo Hit(int dmg, Vector2 dir) 
    {
        status.curHp -= dmg;
        hitInfo.dmg = dmg;
        hitInfo.hitDir = dir;
        hitInfo.isDurable = status.isDurable;
        hitInfo.isHitSucess = !status.isInvincible;
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
