using Enums;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tangtangi : Enemy
{
    public TangtangiActionTable ActionTable;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public Weapon weapon;

    protected override void Initialize()
    {
        status.curHp = status.maxHp;
        weapon.Init(this);
        Rigidbody2D = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = status.walkSpeed;
        status.fireTimer = status.fireWaitTime;
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        Rigidbody2D.AddForce(dir * 2);
        status.curHp -= dmg;
        if (status.curHp <= 0) ActionTable.SetCurAction((int)BangtaniActions.Death);

        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }
}
