using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController: MonoBehaviour
{
    public Vector2 aimPos;
    private Vector3 aimDir;
    private Player player;

    private enum State
    {
        Aiming,
        NotAiming,
    }
    private State aimState;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        Aim();
    }

    private void Aim()
    {
        Vector2 MousePosition = Input.mousePosition;
        aimPos = Camera.main.ScreenToWorldPoint(MousePosition);

        aimDir = (aimPos - new Vector2(this.transform.position.x , this.transform.position.y).normalized);

        if(aimState == State.Aiming)
        {
            float dot = Vector3.Dot(player.transform.right, aimDir);
            if (dot > 0f)
            {
                player.spriteHolder.localScale = new Vector3(1f, 1f);
            }
            else if (dot < 0f)
            {
                player.spriteHolder.localScale = new Vector3(-1f, 1f);
            }
        }
    }
}
