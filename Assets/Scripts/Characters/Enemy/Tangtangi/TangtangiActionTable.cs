using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AI;

public class TangtangiActionTable : ActionTable<Tangtangi>
{
    [SerializeField]
    private TangtangiActions preAction_e;
    [SerializeField]
    private TangtangiActions curAction_e;

    private Vector3 dir;

    public Vector3 Dir { get { return dir; } set { dir = value; } }

    protected override void Initialize()
    {
        if (owner == null) owner = GetComponent<Tangtangi>();
        if (owner != null)
        {
            owner.ActionTable = this;
            actions = new Action<Tangtangi>[(int)TangtangiActions.End];
        }

        actions[(int)TangtangiActions.Idle] = new Tangtangi_Idle();
        actions[(int)TangtangiActions.Move] = new Tangtangi_Move();
        actions[(int)TangtangiActions.Patrol] = new Tangtangi_Patrol();
        actions[(int)TangtangiActions.MoveRandom] = new Tangtangi_MoveRandom();
        actions[(int)TangtangiActions.Attack] = new Tangtangi_Attack();
        actions[(int)TangtangiActions.Death] = new Tangtangi_Death();
    }
    protected override void Awake()
    {
        base.Awake();
        SetCurAction((int)HwasariActions.Idle);
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void SetCurAction(int index)
    {
        preAction_e = (TangtangiActions)preAction_i;
        base.SetCurAction(index);
        curAction_e = (TangtangiActions)curAction_i;
    }
}



public class Tangtangi_Idle : Action<Tangtangi>
{
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("Idle", true);
        //timer = me.status.shootWaitTime;
    }

    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;
        if (me.DistToTarget > me.status.traceRange)
        {
            Debug.Log("패트롤");
            me.ActionTable.SetCurAction((int)TangtangiActions.Patrol);
        }
        else
        {
            if (me.DistToTarget < me.status.attackRange && CanShootPlayer())
            {
                if (me.status.fireTimer < 0f)
                {
                    me.ActionTable.SetCurAction((int)TangtangiActions.Attack);
                    me.status.fireTimer = me.status.fireWaitTime;
                }
            }
            else
            {
                me.ActionTable.SetCurAction((int)TangtangiActions.Move);
            }
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() {
        me.animator.SetBool("Idle", false);
    }

    bool CanShootPlayer()
    {
        // 적에서 플레이어 방향으로 레이를 쏘아 벽을 감지
        RaycastHit2D hit = Physics2D.Raycast(me.weapon.firePos.position, me.target.transform.position - me.weapon.firePos.position, 999f);


        // 벽을 감지했다면 플레이어가 벽 뒤에 있다고 판단
        if (hit.collider)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return true;
            else return false;
        }
        else return false;
    }
}

public class Tangtangi_Patrol : Action<Tangtangi>
{
    bool isMoving;
    float idleTimer;
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("Walking", true);
        isMoving = false;
        idleTimer = 2f;
    }

    public override void ActionUpdate()
    {
        if (!isMoving)
        {
            idleTimer -= Time.deltaTime;
            if(idleTimer < 0f)
            {
                idleTimer = 2f;
                isMoving = true;
                me.agent.SetDestination(GetRandomNavMeshPosition());
                me.agent.isStopped = false;

                me.animator.SetBool("Walking", true);
                me.animator.SetBool("Idle", false);
            }
        }
        else
        {
            if (me.agent.remainingDistance <= 0)
            {
                isMoving = false;
                me.agent.isStopped = true;
                me.animator.SetBool("Walking", false);
                me.animator.SetBool("Idle", true);
            }
        }

        if (me.DistToTarget < me.status.traceRange)
        {
            isMoving = false;
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)TangtangiActions.Move);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() {
        me.animator.SetBool("Walking", false);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        // NavMesh 상에서 유효한 랜덤 위치를 찾아 반환
        Vector3 randomDirection = Random.insideUnitSphere * 5f; // 반경 10 유효
        randomDirection += me.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        return hit.position;
    }
}


public class Tangtangi_Move : Action<Tangtangi>
{
    bool isMoving;
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("Walking", true);
    }
    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;

        Vector3 destinationPos = (me.target.transform.position);
        me.agent.SetDestination(destinationPos);
        me.agent.isStopped = false;

        // 추적
        if (me.DistToTarget < me.status.attackRange && CanShootPlayer())
        {
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)TangtangiActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("Walking", false);
    }

    bool CanShootPlayer()
    {
        // 적에서 플레이어 방향으로 레이를 쏘아 벽을 감지
        RaycastHit2D hit = Physics2D.Raycast(me.weapon.firePos.position, me.target.transform.position - me.weapon.firePos.position, 999f);


        // 벽을 감지했다면 플레이어가 벽 뒤에 있다고 판단
        if(hit.collider)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) return true;
            else return false;
        }
        else return false;
    }
}

public class Tangtangi_MoveRandom : Action<Tangtangi>
{
    bool isMoving;
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        isMoving = false;
        me.animator.SetBool("Walking", true);
    }
    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;

        if(!isMoving)
        {
            isMoving = true;
            me.agent.SetDestination(GetRandomNavMeshPosition());
            me.agent.isStopped = false;
        }
        else
        {
            if(me.agent.remainingDistance <= 0)
            {
                me.agent.SetDestination(GetRandomNavMeshPosition());
            }
        }

        // 추적
        if (me.status.fireTimer < 0)
        {
            me.agent.isStopped = true;
            isMoving = false;
            me.ActionTable.SetCurAction((int)TangtangiActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("Walking", false);
    }

    Vector3 GetRandomNavMeshPosition()
    {
        // NavMesh 상에서 유효한 랜덤 위치를 찾아 반환
        Vector3 randomDirection = Random.insideUnitSphere * 5f; // 반경 10 유효
        randomDirection += me.transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        return hit.position;
    }
}

public class Tangtangi_Attack : Action<Tangtangi>
{
    float timer;
    int curbulletCount;

    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        curbulletCount = me.status.fireCountPerAttack;
        me.animator.SetBool("Idle", true);
    }

    public override void ActionUpdate()
    {
        timer -= Time.deltaTime;

        if (curbulletCount > 0)
        {
            if (timer <= 0f)
            {
                for(int i = 0; i < me.status.bulletNumPerFire; i++)
                {
                    me.weapon.Fire();
                }
                curbulletCount--;
                timer = me.status.fireRate;
            }
        }
        else
        {
            me.ActionTable.SetCurAction((int)TangtangiActions.MoveRandom);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("Idle", false);
    }
}


public class Tangtangi_Death : Action<Tangtangi>
{
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        //GameObject slim_prop = PoolingManager.Instance.LentalObj("Slime_Prop");
        //slim_prop.transform.position = me.transform.position;
        //slim_prop.transform.localScale = me.transform.localScale;
        //slim_prop.GetComponent<Prop>().Play(5f);

        //GameObject wp_prop = PoolingManager.Instance.LentalObj("Gun_Prop");
        //wp_prop.transform.position = me.transform.position;
        //wp_prop.transform.localScale = me.transform.localScale;
        //wp_prop.GetComponent<Prop>().Play(1.5f);
        //StageManager.Instance?.OnMonsterDeath();
        me.gameObject.SetActive(false);
    }

    public override void ActionUpdate() { }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}