using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action<T>
{
    protected T me;
    public virtual void ActionEnter(T script)
    {
        if (me == null) me = script;
    }
    public abstract void ActionUpdate();
    public virtual void ActionFixedUpdate() { }
    public virtual void ActionLateUpdate() { }
    public abstract void ActionExit();
}

