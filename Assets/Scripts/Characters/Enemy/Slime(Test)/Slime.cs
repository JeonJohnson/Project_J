using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using UnityEngine.AI;

public class Slime : Enemy
{
    public SlimeActionTable ActionTable;
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator animator;
    public EnemyStatus status;
    public Weapon weapon;

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

    public override void Hit(int dmg, Vector2 dir)
    {
        base.Hit(dmg, dir);
        status.curHp -= dmg;
        Rigidbody2D.AddForce(dir * 10);
    }

}
