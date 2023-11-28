using Enums;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_None : Action<Player>
{

    public override void ActionEnter(Player script)
    {
        base.ActionEnter(script);
    }

    public override void ActionUpdate()
    {

    }

    public override void ActionFixedUpdate() {

    }

    public override void ActionLateUpdate() { }

    public override void ActionExit() 
    {

    }
}
