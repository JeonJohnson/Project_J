using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : CObj
{
    public float distToTarget;
    public Player target;
    public NavMeshAgent agent;

    public virtual void Awake()
    {
        Initialize();
    }

    protected abstract void Initialize();


    public override void Hit(int dmg, Vector2 dir)
    {

    }

    public float DistToTarget
    {
        get
        {
            distToTarget = Vector3.Distance(target.transform.position, this.transform.position);
            return distToTarget;
        }
    }
}
