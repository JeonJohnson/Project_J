using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //timer = me.status.shootWaitTime;
    }

    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;
        if (me.DistToTarget > me.status.traceRange)
        {
            me.ActionTable.SetCurAction((int)TangtangiActions.Move);
        }
        else
        {
            if (me.status.fireTimer < 0f)
            {
                me.ActionTable.SetCurAction((int)TangtangiActions.Attack);
                me.status.fireTimer = me.status.fireWaitTime;
            }
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}


public class Tangtangi_Move : Action<Tangtangi>
{
    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("IsMove", true);
    }
    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;

        Vector3 destinationPos = (me.target.transform.position);
        me.agent.SetDestination(destinationPos);
        me.agent.isStopped = false;

        // ÃßÀû
        if (me.DistToTarget < me.status.attackRange)
        {
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)TangtangiActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("IsMove", false);
    }
}

public class Tangtangi_Attack : Action<Tangtangi>
{
    float timer;
    int curbulletCount;

    public override void ActionEnter(Tangtangi script)
    {
        base.ActionEnter(script);
        timer = me.status.fireRate;
        curbulletCount = me.status.fireCountPerAttack;
    }

    public override void ActionUpdate()
    {
        timer -= Time.deltaTime;

        if (curbulletCount > 0)
        {
            if (timer <= 0f)
            {
                me.weapon.Fire();
                curbulletCount--;
                timer = me.status.fireRate;
            }
        }
        else
        {
            me.ActionTable.SetCurAction((int)BangtaniActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {

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
        StageManager.Instance?.OnMonsterDeath();
        me.gameObject.SetActive(false);
    }

    public override void ActionUpdate() { }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}