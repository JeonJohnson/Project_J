using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionTable<T> : MonoBehaviour
{
    [SerializeField]
    protected T owner;
    protected Action<T>[] actions;
    [SerializeField]
    protected Action<T> preAction;
    [SerializeField]
    protected Action<T> curAction;

    protected int preAction_i;
    protected int curAction_i;

    protected abstract void Initialize();

    protected virtual void Awake()
    {
        Initialize();
    }
    protected virtual void Start() { }
    protected virtual void Update()
    {
        if (curAction != null)
        {
            curAction.ActionUpdate();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (curAction != null) curAction.ActionFixedUpdate();
    }
    protected virtual void LateUpdate()
    {
        if (curAction != null) curAction.ActionLateUpdate();
    }

    public virtual void SetCurAction(int index)
    {
        int nextAction = -1;
        nextAction = System.Array.IndexOf(actions, actions[index]);
        if (nextAction < 0) return;

        if (curAction != null)
        {
            curAction.ActionExit();
            preAction = curAction;

            preAction_i = System.Array.IndexOf(actions, curAction);
        }

        curAction = actions[nextAction];
        curAction_i = nextAction;

        curAction.ActionEnter(owner);
    }
}
