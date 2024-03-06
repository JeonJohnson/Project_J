using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class PlayerAimController: MonoBehaviour
{
    public Vector2 aimPos;
    private Vector3 aimDir;
    public Vector3 AimDir { get { return aimDir; } }    
    private Player player;

    public CinemachineConfiner cinemachineConfiner;
    public CamCtrl camCtrl;

    public bool isPadControl;

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
        if(!isPadControl)
        {
            aimPos = Camera.main.ScreenToWorldPoint(MousePosition);
        }
        else
        {
            aimPos += player.playerInput.mouseAxis * Time.deltaTime;
        }

        aimDir = (aimPos - new Vector2(this.transform.position.x , this.transform.position.y)).normalized;

        player.animator.SetFloat("Horizontal", aimDir.x);
        player.animator.SetFloat("Vertical", aimDir.y);

        UiController_Proto.Instance?.playerHudView.UpdateCrossHairAnchorPos(MousePosition);

    }

    public float GetAimAngle(Vector3 pointVec)
    {
        float angle = Vector3.Angle(Vector3.up, aimDir);
        return angle;
    }
}
