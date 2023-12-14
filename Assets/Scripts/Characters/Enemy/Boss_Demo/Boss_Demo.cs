using Enums;
using MoreMountains.Feedbacks;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Demo : Enemy
{
    public Boss_DemoActionTable ActionTable;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public float guardAngle = 90f;
    public Boss_Demo_Weapon weapon;

    public ParticleSystem footstepParticle;
    [SerializeField] MMF_Player hitFeedback;

    protected override void Initialize()
    {
        status.curHp = status.maxHp;
        weapon.Init(this);

        agent = GetComponent<NavMeshAgent>();
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
        Rigidbody2D.AddForce(dir * 2);
        status.curHp -= dmg;
        hitFeedback.PlayFeedbacks();
        if (status.curHp <= 0) ActionTable.SetCurAction((int)BossDemoActions.Death);

        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }

    private void FixedUpdate()
    {
        CalcSpriteDir();
    }

    private void CalcSpriteDir()
    {
        Vector2 targetDir = target.transform.position - this.transform.position;
        float angleToPlayer = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        if (angleToPlayer > -90f && angleToPlayer < 90f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            spriteDir = Vector3.right;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            spriteDir = Vector3.left;
        }
    }
}
