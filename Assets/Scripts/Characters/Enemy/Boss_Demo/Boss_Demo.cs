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
    public MMF_Player runFeedback;

    protected override void Initialize()
    {
        status.curHp = status.maxHp;
        weapon.Init(this);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = status.walkSpeed;
        status.fireTimer = status.fireWaitTime;

        //원래는 구독해서 써야하는데.. 시간읎당
        UiController_Proto.Instance.playerHudView?.bossHpBarHolder.SetActive(true);
    }

    public override void Awake()
    {
        base.Awake();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        Rigidbody2D.AddForce(dir * 2);
        if (!status.isDurable)
        {
            status.curHp -= dmg;
            hitFeedback.PlayFeedbacks();

            // 나중에 OnHit 이벤트 만들어서 구독해서 쓰게 바꾸기
            if (status.curHp <= status.maxHp * (30f / 100f)) { ActionTable.SetCurAction((int)BossDemoActions.Hide); Rigidbody2D.AddForce(dir * 50, ForceMode2D.Impulse); }
            UiController_Proto.Instance.playerHudView?.UpdateBossHpBar((float)status.curHp / (float)status.maxHp);
        }
        else
        {

        }
        if (status.curHp <= 0) ActionTable.SetCurAction((int)BossDemoActions.Death);

        HitInfo hitInfo = new HitInfo();
        hitInfo.isDurable = status.isDurable;
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
