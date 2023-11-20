using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public float speed = 1f;

    private Player player;

    private Vector3 moveDir;

    private Vector3 lastMoveDir;
    public Vector3 LastMoveDir { get { return lastMoveDir; } set { lastMoveDir = value; } }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        HandleMovement();
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

        bool isIdle = moveX == 0 && moveY == 0;
        if (isIdle)
        {
            //조준, 사격기능 활성화
        }
        else
        {
            lastMoveDir = moveDir;
        }
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
        player.PlayerRigidbody2D.velocity = Vector3.zero;
    }

    public Vector3 GetLastMoveDir()
    {
        return lastMoveDir;
    }

    private void FixedUpdate()
    {
        player.PlayerRigidbody2D.velocity = moveDir * speed;
    }
}
