using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : NPC
{
    public Player target;
    public Vector2 calibrateValue;
    public float speed = 1f;
    private Vector2 targetPos;

    public bool isFollowing;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Talk("æ»æ∆¡‡ø‰");
        }

        if (isFollowing)
        {
            targetPos = new Vector2(target.transform.position.x + target.MoveActionTable.LastMoveDir.x * calibrateValue.x, target.transform.position.y + calibrateValue.y);
            this.transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
        }
    }
}
