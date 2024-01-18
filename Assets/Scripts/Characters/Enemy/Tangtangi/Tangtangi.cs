using Enums;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MoreMountains.Feedbacks;

public class Tangtangi : Enemy
{
    public TangtangiActionTable ActionTable;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public Weapon weapon;

    public string deadBodyPrefab;
    public bool isTracePlayer = false;

    public MMF_Player attackFeedback;
    public MMF_Player hitFeedback;

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
        Rigidbody2D.AddForce(dir * 300f);
        status.curHp -= dmg;
        animator.SetTrigger("Damage");
        SoundManager.Instance.PlayTempSound("Tangtangi_Hit", this.transform.position, 1f, 0.8f,1f);

        hitFeedback?.PlayFeedbacks();
        if (status.curHp <= 0 && ActionTable.CurAction_e != TangtangiActions.Death)
        {
            GameObject go = PoolingManager.Instance.LentalObj(deadBodyPrefab);
            go.transform.position = this.transform.position;
            go.GetComponent<Rigidbody2D>()?.AddForce(-dir * 800f);

            SoundManager.Instance.PlayTempSound("Tangtangi_Death", this.transform.position);
            ActionTable.SetCurAction((int)TangtangiActions.Death);
		}

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
