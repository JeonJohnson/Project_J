using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Roll : Action<Player>
{
    private Vector3 rollDir;
    private float rollSpeed;

    public override void ActionEnter(Player script)
    {
        base.ActionEnter(script);
        rollDir = me.moveActionTable.LastMoveDir;
        rollSpeed = me.status.rollSpeed;
        me.afterImageController.enable = true;
        //�÷��̾� ���Ӹ�� ���
    }

    public override void ActionUpdate()
    {
        HandleRolling();
    }

    public override void ActionFixedUpdate() 
    {
        me.PlayerRigidbody2D.velocity = rollDir * rollSpeed;
    }

    public override void ActionLateUpdate() { }

    public override void ActionExit() 
    {
        me.afterImageController.enable = false;
    }


    private void HandleRolling()
    {

        float rollSpeedDropMultiplier = me.status.rollSpeedDrop;
        rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        float rollSpeedMinimum = me.status.walkSpeed;
        if (rollSpeed < rollSpeedMinimum)
        {
            me.MoveActionTable.SetCurAction((int)PlayerMoveActions.Move);
            //���� ����
        }
    }
}
