using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : CObj
{
    public Rigidbody2D PlayerRigidbody2D { get; private set; }
    public PlayerMoveActionTable moveActionTable { get; private set; }
    public PlayerMoveActionTable MoveActionTable { get { return moveActionTable; } set { moveActionTable = value; } }

    public PlayerAimController aimController;

    public AfterImageEffectController afterImageController;

    public Animator animator { get; private set; }

    public PlayerStatus status;
    public Transform spriteHolder;
    public Transform weaponHolder;

    public Weapon curWeapon;


    private void Awake()
    {
        PlayerRigidbody2D = GetComponent<Rigidbody2D>();
        moveActionTable = GetComponent<PlayerMoveActionTable>();
        aimController = GetComponent<PlayerAimController>();
        afterImageController = spriteHolder.GetComponent<AfterImageEffectController>();
        animator = spriteHolder.GetComponent<Animator>();
    }

    public override void Hit(int dmg, Vector2 dir)
    {
        status.curHp -= dmg;
        //PlayerRigidbody2D.AddForce(dir * 500f);
    }
}
