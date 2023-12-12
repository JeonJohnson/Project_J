using Enums;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Boss_DemoActionTable : ActionTable<Boss_Demo>
{
    [SerializeField]
    private BossDemoActions preAction_e;
    [SerializeField]
    private BossDemoActions curAction_e;

    private Vector3 dir;

    public Vector3 Dir { get { return dir; } set { dir = value; } }

    protected override void Initialize()
    {
        if (owner == null) owner = GetComponent<Boss_Demo>();
        if (owner != null)
        {
            owner.ActionTable = this;
            actions = new Action<Boss_Demo>[(int)BossDemoActions.End];
        }

        actions[(int)BossDemoActions.Idle] = new Boss_Idle();
        actions[(int)BossDemoActions.Move] = new Boss_Move();
        actions[(int)BossDemoActions.Attack0] = new Boss_Attack0();
        actions[(int)BossDemoActions.Attack1] = new Boss_Attack1();
        actions[(int)BossDemoActions.Attack2] = new Boss_Attack2();
        actions[(int)BossDemoActions.Death] = new Boss_Death();
    }
    protected override void Awake()
    {
        base.Awake();
        SetCurAction((int)BangtaniActions.Idle);
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
        preAction_e = (BossDemoActions)preAction_i;
        base.SetCurAction(index);
        curAction_e = (BossDemoActions)curAction_i;
    }
}


public class Boss_Idle : Action<Boss_Demo>
{
    float timer =2f;

    float weightFunction1 = 1f;
    float weightFunction2 = 1f;
    float weightFunction3 = 1f;

    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
    }

    public override void ActionUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            RandomlyExecuteSkill();
            timer = me.status.fireWaitTime;
        }

        if (me.DistToTarget > me.status.traceRange)
        {
            me.ActionTable.SetCurAction((int)BossDemoActions.Move);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }

    void RandomlyExecuteSkill()
    {
        float totalWeight = weightFunction1 + weightFunction2 + weightFunction3;

        float randomWeight = Random.Range(0f, totalWeight);

        if (randomWeight < weightFunction1)
        {
            me.ActionTable.SetCurAction((int)BossDemoActions.Attack0);
            weightFunction1 = 1f;
            weightFunction2 += 1f;
            weightFunction3 += 1f;
        }
        else if (randomWeight < weightFunction1 + weightFunction2)
        {
            me.ActionTable.SetCurAction((int)BossDemoActions.Attack1);
            weightFunction1 += 1f;
            weightFunction2 = 1f;
            weightFunction3 += 1f;
        }
        else
        {
            me.ActionTable.SetCurAction((int)BossDemoActions.Attack2);
            weightFunction1 += 1f;
            weightFunction2 += 1f;
            weightFunction3 = 1f;
        }
    }
}

public class Boss_Move : Action<Boss_Demo>
{
    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
        me.animator.SetBool("IsMove", true);
    }
    public override void ActionUpdate()
    {
        Vector3 dir = me.target.transform.position - me.transform.position;
        dir.Normalize();
        Vector3 destinationPos = (me.target.transform.position + dir * 5f);
        me.agent.SetDestination(destinationPos);
        me.agent.isStopped = false;

        // 추적
        if (Vector3.Distance(me.transform.position, me.target.transform.position) < me.status.traceRange)
        {
            me.agent.isStopped = true;
            me.ActionTable.SetCurAction((int)BossDemoActions.Idle);
        }
    }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit()
    {
        me.animator.SetBool("IsMove", false);
    }
}

public class Boss_Attack0 : Action<Boss_Demo>
{
    float timer;
    int curIndex;
    int curbulletCount;

    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
        curbulletCount = 0;
        curIndex = 0;
    }

    public override void ActionUpdate()
    {
        // 회전
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = me.status.fireRate;
            curIndex += 10;

            float angleRadians = Mathf.Deg2Rad * curIndex;
            // 각도에 해당하는 방향 벡터 계산
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));

            me.weapon.Fire(direction, me.status.spread, 200f);
            curbulletCount++;
        }

        if (curbulletCount == 36)
        {
            me.ActionTable.SetCurAction((int)BossDemoActions.Idle);
        }
    }
    public override void ActionExit() { }
}

public class Boss_Attack1 : Action<Boss_Demo>
{
    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
        me.StartCoroutine(ShootCoro());
    }

    public override void ActionUpdate()
    {

    }

    public override void ActionExit() { }


    IEnumerator ShootCoro()
    {
        for (int i = 0; i < 8; i++)
        {
            float angleRadians = Mathf.Deg2Rad * 360 / 8 * i;
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            me.weapon.Fire(direction, 0, 200);
        }
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < 8; i++)
        {
            float angleRadians = Mathf.Deg2Rad * 360 / 8 * (float)i + 22.5f;
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            me.weapon.Fire(direction, 0, 200);
        }
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < 8; i++)
        {
            float angleRadians = Mathf.Deg2Rad * 360 / 8 * i;
            Vector2 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            me.weapon.Fire(direction, 0, 200);
        }
        yield return new WaitForSeconds(1.2f);
        Vector2 dir = me.target.transform.position - me.weapon.firePos.transform.position;
        me.weapon.FireCrossBullet(dir);
        yield return new WaitForSeconds(0.8f);
        dir = me.target.transform.position - me.weapon.firePos.transform.position;
        me.weapon.FireCrossBullet(dir);
        yield return new WaitForSeconds(0.8f);
        dir = me.target.transform.position - me.weapon.firePos.transform.position;
        me.weapon.FireCrossBullet(dir);


        me.ActionTable.SetCurAction((int)BossDemoActions.Idle);
    }
}


public class Boss_Attack2 : Action<Boss_Demo>
{
    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
        me.StartCoroutine(AttackCoro());
    }

    public override void ActionUpdate(){ }

    public override void ActionExit() { }

    private IEnumerator AttackCoro()
    {
        int checkWallCount = 0;
        int makedBulletCount = 0;

        bool isMakingBullet = true;

        while(isMakingBullet)
        {
            Vector3 rndPos = me.transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
            Debug.Log(rndPos);

            if (CheckWall(rndPos))
            {
                Debug.Log(checkWallCount + "번 벽 걸림");
                checkWallCount++;
                if (checkWallCount > 50) break;
            }
            else
            {
                me.weapon.FireRainBullet(rndPos, 3f, 10f);
                makedBulletCount++;
                yield return new WaitForSeconds(0.4f);
            }
            if (makedBulletCount >= 8) break;
        }

        yield return new WaitForSeconds(5f);

        me.ActionTable.SetCurAction((int)BossDemoActions.Idle);
    }

    private bool CheckWall(Vector3 rndPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)rndPos, 1f, 1 << LayerMask.NameToLayer("Wall"));

        if(hits.Length > 0)
        {
            Debug.Log(hits[0].name);
            return true;
        }
        else
            return false;
    }
}

public class Boss_Death : Action<Boss_Demo>
{
    public override void ActionEnter(Boss_Demo script)
    {
        base.ActionEnter(script);
        me.gameObject.SetActive(false);
    }

    public override void ActionUpdate() { }

    public override void ActionFixedUpdate() { }

    public override void ActionLateUpdate() { }

    public override void ActionExit() { }
}