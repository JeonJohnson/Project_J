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

    public override void Start()
    {
        base.Start();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        target.OnEnemyHit(this);

        Rigidbody2D.AddForce(dir * 300f);
        animator.SetTrigger("Damage");

        hitFeedback?.PlayFeedbacks();
        if (status.curHp - dmg <= 0 && ActionTable.CurAction_e != TangtangiActions.Death)
        {
            //시체드랍
            GameObject go = PoolingManager.Instance.LentalObj(deadBodyPrefab);
            go.transform.position = this.transform.position;
            go.GetComponent<Rigidbody2D>()?.AddForce(-dir * 800f);

            //코인드랍
            int rndCoinCount = Random.Range(1, 5);
            for(int i=0; i < rndCoinCount; i++)
            {
                GameObject coinGo = PoolingManager.Instance.LentalObj("Coin", 1);
                coinGo.transform.position = this.transform.position;
                Vector2 RandomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                coinGo.GetComponent<Rigidbody2D>()?.AddForce(-RandomDir * 300f);
            }

            //근희임시추가
            //StageManager.Instance?.AddDeadBody(go.GetComponent<Enemy_DeadBody>());
            //근희임시추가

            //데이터 뿌리기
            target.OnEnemyKill(this);
            ActionTable.SetCurAction((int)TangtangiActions.Death);
            this.gameObject.SetActive(false);
        }
        return base.Hit(dmg, dir); 
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
