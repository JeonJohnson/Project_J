using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveActionTable : ActionTable<Player>
{
    [SerializeField]
    private PlayerMoveActions preAction_e;
    [SerializeField]
    private PlayerMoveActions curAction_e;

    private Vector3 lastMoveDir;
    public Vector3 LastMoveDir { get { return lastMoveDir; } set { lastMoveDir = value; } }

    public bool isMoving;

    protected override void Initialize()
    {
        if (owner == null) owner = GetComponent<Player>();
        if (owner != null)
        {
            owner.MoveActionTable = this;
            actions = new Action<Player>[(int)PlayerMoveActions.End];
        }

        actions[(int)PlayerMoveActions.Move] = new Player_Move();
        actions[(int)PlayerMoveActions.Roll] = new Player_Roll();
        actions[(int)PlayerMoveActions.None] = new Player_None();
    }

    protected override void Awake()
    {
        base.Awake();
        SetCurAction((int)PlayerMoveActions.Move);
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
        preAction_e = (PlayerMoveActions)preAction_i;
        base.SetCurAction(index);
        curAction_e = (PlayerMoveActions)curAction_i;
    }
}
