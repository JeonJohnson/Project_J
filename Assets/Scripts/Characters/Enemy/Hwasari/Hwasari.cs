using Enums;
using MoreMountains.Feedbacks;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hwasari : Enemy
{
    public HwasariActionTable ActionTable;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public Weapon_Hwasari weapon;

    public MMF_Player attackFeedback;
    [SerializeField] MMF_Player hitFeedback;

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

    public override void Start()
    {
        base.Start();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        hitFeedback.PlayFeedbacks();

            Rigidbody2D.AddForce(dir);
            status.curHp -= dmg;
            if (status.curHp <= 0) ActionTable.SetCurAction((int)BangtaniActions.Death);

        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }
}
