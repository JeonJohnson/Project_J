using Enums;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : Action<Player>
{

    private Vector3 moveDir;

    public override void ActionEnter(Player script)
    {
        base.ActionEnter(script);
    }

    public override void ActionUpdate()
    {
        HandleMovement();
    }

    public override void ActionFixedUpdate() {
        me.PlayerRigidbody2D.velocity = moveDir * (me.status.walkSpeed);
    }

    public override void ActionLateUpdate() { }

    public override void ActionExit() 
    {
        me.PlayerRigidbody2D.velocity = Vector3.zero;
        me.animator.SetBool("isMove", false);
    }


    private void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveX = +1f;
        }

        moveDir = new Vector3(moveX, moveY).normalized;

        bool isMove = moveX != 0 || moveY != 0;
        if (isMove)
        {
            me.footstepParticle.Play();
            me.animator.SetBool("isMove", true);
            me.MoveActionTable.LastMoveDir = moveDir;
            //조준, 사격기능 활성화
        }
        else
        {
            me.footstepParticle.Stop();
            me.animator.SetBool("isMove", false);
        }
    }
}
