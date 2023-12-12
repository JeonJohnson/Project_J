using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActionTable : ActionTable<Slime>
{
    [SerializeField]
    private SlimeActions preAction_e;
    [SerializeField]
    private SlimeActions curAction_e;

    private Vector3 dir;

    public Vector3 Dir { get { return dir; } set { dir = value; } }

    protected override void Initialize()
    {
        if (owner == null) owner = GetComponent<Slime>();
        if (owner != null)
        {
            owner.ActionTable = this;
            actions = new Action<Slime>[(int)SlimeActions.End];
        }

        actions[(int)SlimeActions.Idle] = new Slime_Idle();
        actions[(int)SlimeActions.Move] = new Slime_Move();
        actions[(int)SlimeActions.Attack] = new Slime_Attack();
        actions[(int)SlimeActions.Death] = new Slime_Death();
    }
    protected override void Awake()
    {
        base.Awake();
        SetCurAction((int)SlimeActions.Idle);
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
        preAction_e = (SlimeActions)preAction_i;
        base.SetCurAction(index);
        curAction_e = (SlimeActions)curAction_i;
    }
}



public class Slime_Idle : Action<Slime>
{
    public override void ActionEnter(Slime script)
    {
        base.ActionEnter(script);
        //timer = me.status.shootWaitTime;
    }

    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;
        if (me.status.fireTimer < 0f)
        {
            me.ActionTable.SetCurAction((int)SlimeActions.Attack);
            me.status.fireTimer = me.status.fireWaitTime;
        }

        if (me.DistToTarget > me.status.attackRange)
        {
            me.ActionTable.SetCurAction((int)SlimeActions.Move);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}

public class Slime_Move : Action<Slime>
{
    public override void ActionEnter(Slime script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("IsMove", true);
    }
    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;

        Vector3 dir = me.target.transform.position - me.transform.position;
        dir.Normalize();
        Vector3 destinationPos = (me.target.transform.position);
        me.agent.SetDestination(destinationPos);
        me.agent.isStopped = false;

        // ÃßÀû
        if (me.DistToTarget < me.status.attackRange)
        {
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)SlimeActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("IsMove", false);
    }
}

public class Slime_Attack : Action<Slime>
{
    float timer;
    int curbulletCount;

    public override void ActionEnter(Slime script)
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
            me.ActionTable.SetCurAction((int)SlimeActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
       
    }
}


public class Slime_Death : Action<Slime>
{
    public override void ActionEnter(Slime script)
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

        me.gameObject.SetActive(false);
    }

    public override void ActionUpdate() { }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}
