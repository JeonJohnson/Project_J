using Enums;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bangtani : Enemy
{
    public BangtaniActionTable ActionTable;
    public BangtaniGuardController guardController;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public float guardAngle = 90f;
    public Weapon weapon;

    protected override void Initialize()
    {
        status.curHp = status.maxHp;
        weapon.Init(this);

        agent = GetComponent<NavMeshAgent>();
        guardController = GetComponent<BangtaniGuardController>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = status.walkSpeed;
        status.fireTimer = status.fireWaitTime;
    }

    public override void Awake()
    {
        base.Awake();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        bool isGuard = guardController.GetIsGuardSucess(dir, guardAngle);

        if (isGuard)
        {
            Rigidbody2D.AddForce(dir * 0.5f);
        }
        else
        {
            Rigidbody2D.AddForce(dir * 2);
            status.curHp -= dmg;
            if(status.curHp <= 0) ActionTable.SetCurAction((int)BangtaniActions.Death);
        }

        HitInfo hitInfo = new HitInfo();
        hitInfo.isDurable = isGuard;
        return hitInfo;
    }
}
