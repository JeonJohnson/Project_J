using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Drone : NPC
{
    public Player target;
    public Vector2 calibrateValue;
    public float speed = 1f;
    private Vector2 targetPos;
    private Vector2 circleAddValue;
    public float saftyDist = 2f;

    public bool isFollowing;
    private bool isMoving = false;

    public GameObject testBox;

    public Animator animator;

    Vector3 CalculatePosition(Vector3 anchor, float angle)
    {

        Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.left;


        Vector3 pos0 = anchor + direction * saftyDist;

        return pos0;
    }
    private void Start()
    {
        TutorialManager.Instance.npcList.Add(this.gameObject.name, this.gameObject);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Talk("¾È¾ÆÁà¿ä", 50, 0.1f);
        }

        CheckPlayerMovingTrigger();
        RotateDir();

        if (isFollowing)
        {
            targetPos = new Vector2(target.transform.position.x + calibrateValue.x + circleAddValue.x, target.transform.position.y + calibrateValue.y + circleAddValue.y);
            this.transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
            testBox.transform.position = targetPos;
        }

    }

    private void CheckPlayerMovingTrigger()
    {
        if (target.MoveActionTable.isMoving)
        {
            if (!isMoving)
            {
                animator.SetBool("Move", true);
                isMoving = true;
                float rndAngle = Random.Range(0f, -180f);
                circleAddValue = CalculatePosition(Vector3.zero, rndAngle);
            }
        }
        else
        {
            if (isMoving)
            {
                animator.SetBool("Move", false);
                isMoving = false;
            }
        }
    }

    private Vector3 droneDir;
    private Vector3 leftDir = new Vector3(-1f, 1f, 1f);
    private Vector3 righrDir = new Vector3(1f, 1f, 1f);
    private void RotateDir()
    {
        droneDir = target.transform.position - this.transform.position;
        if(droneDir.x > 0)
        {
            this.transform.localScale = leftDir;
        }
        else
        {
            this.transform.localScale = righrDir;
        }
    }
}
