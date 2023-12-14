using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HwasariActionTable : ActionTable<Hwasari>
{
    [SerializeField]
    private HwasariActions preAction_e;
    [SerializeField]
    private HwasariActions curAction_e;

    private Vector3 dir;

    public Vector3 Dir { get { return dir; } set { dir = value; } }

    protected override void Initialize()
    {
        if (owner == null) owner = GetComponent<Hwasari>();
        if (owner != null)
        {
            owner.ActionTable = this;
            actions = new Action<Hwasari>[(int)HwasariActions.End];
        }

        actions[(int)HwasariActions.Idle] = new Hwasari_Idle();
        actions[(int)HwasariActions.Move] = new Hwasari_Move();
        actions[(int)HwasariActions.Attack] = new Hwasari_Attack();
        actions[(int)HwasariActions.Death] = new Hwasari_Death();
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
        preAction_e = (HwasariActions)preAction_i;
        base.SetCurAction(index);
        curAction_e = (HwasariActions)curAction_i;
    }
}



public class Hwasari_Idle : Action<Hwasari>
{
    public override void ActionEnter(Hwasari script)
    {
        base.ActionEnter(script);
        //timer = me.status.shootWaitTime;
    }

    public override void ActionUpdate()
    {
        me.status.fireTimer -= Time.deltaTime;
        if (me.DistToTarget > me.status.traceRange)
        {
            me.ActionTable.SetCurAction((int)HwasariActions.Move);
        }
        else
        {
            if (me.status.fireTimer < 0f)
            {
                me.ActionTable.SetCurAction((int)HwasariActions.Attack);
                me.status.fireTimer = me.status.fireWaitTime;
            }
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}

public class Hwasari_Move : Action<Hwasari>
{
    public override void ActionEnter(Hwasari script)
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

        // 추적
        if (me.DistToTarget < me.status.attackRange)
        {
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)HwasariActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("IsMove", false);
    }
}

public class Hwasari_Attack : Action<Hwasari>
{
    float timer;
    int curbulletCount;

    public override void ActionEnter(Hwasari script)
    {
        base.ActionEnter(script);
        me.attackFeedback.InitialDelay = me.status.fireWaitTime - 1.5f;
        me.attackFeedback.PlayFeedbacks();
        timer = me.status.fireWaitTime;
        curbulletCount = me.status.fireCountPerAttack;
    }

    public override void ActionUpdate()
    {
        timer -= Time.deltaTime;

        if (curbulletCount > 0)
        {
            if (timer <= 0f)
            {
                float startAngle = -me.status.spread / 2;  // 시작 각도
                float angleStep = me.status.spread / (me.status.bulletNumPerFire - 1);
                for (int i = 0; i < me.status.bulletNumPerFire; i++)
                {
                    Debug.Log("타아아앙");
                    me.weapon.Fire(startAngle + i * angleStep);
                }
                curbulletCount--;
                timer = me.status.fireRate;
            }
        }
        else
        {
            me.ActionTable.SetCurAction((int)HwasariActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {

    }
}


public class Hwasari_Death : Action<Hwasari>
{
    public override void ActionEnter(Hwasari script)
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