using MoreMountains.Feedbacks;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : CObj
{
    private PlayerMoveActionTable moveActionTable;
    public PlayerMoveActionTable MoveActionTable { get { return moveActionTable; } set { moveActionTable = value; } }

    public PlayerAimController aimController;

    public AfterImageEffectController afterImageController;

    public PlayerAttackController attackController;

    public Rigidbody2D PlayerRigidbody2D { get; private set; }
    public Animator animator { get; private set; }

    public PlayerStatus status;
    public Transform spriteHolder;
    public Transform weaponHolder;
    public Weapon_Player curWeapon;

    [Header("Feedbacks")]
    [SerializeField] MMBlink hitBlink;
    [SerializeField] MMF_Player hitFeedback;

    private Coroutine invincibleCor;

    private void Awake()
    {
        PlayerRigidbody2D = GetComponent<Rigidbody2D>();
        moveActionTable = GetComponent<PlayerMoveActionTable>();
        aimController = GetComponent<PlayerAimController>();
        afterImageController = spriteHolder.GetComponent<AfterImageEffectController>();
        animator = spriteHolder.GetComponent<Animator>();
        attackController = GetComponent<PlayerAttackController>();

        if (UiController_Proto.Instance != null) { UiController_Proto.Instance.playerUiView.UpdateHpImage(status.curHp); }
    }

    private void Update()
    {

    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        if (!status.isInvincible)
        { 
            status.curHp -= dmg;
            SetInvincible(status.invincibleTimeWhenHit);

            if (UiController_Proto.Instance != null)
            {
                UiController_Proto.Instance.playerUiView.UpdateHpImage(status.curHp);
                UiController_Proto.Instance.PlayHitFeedback();
            }
        }
        //PlayerRigidbody2D.AddForce(dir * 500f);

        if(status.curHp <= 0)
        {
            Dead();
        }


        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }

    private void Dead()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        PlayerRigidbody2D.simulated = false;
        spriteHolder.gameObject.SetActive(false);
        weaponHolder.gameObject.SetActive(false);
        moveActionTable.enabled = false;
        attackController.enabled = false;
    }

    public IEnumerator InvincibleCor(float time)
    {
        float timer = time;
        status.isInvincible = true;

        //yield return new WaitForSeconds(time);

        hitBlink.Phases[0].PhaseDuration = time;
        hitFeedback.PlayFeedbacks();
        while (true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                break;
            }

            yield return null;
        }
        status.isInvincible = false;
    }

    public void SetInvincible(float time)
    {
        if (!status.isInvincible)
        {
            if (invincibleCor != null)
            {
                StopCoroutine(invincibleCor);
            }
            invincibleCor =
            StartCoroutine(InvincibleCor(time));
        }
        else
        {
            if (invincibleCor != null)
            {
                StopCoroutine(invincibleCor);
            }
            invincibleCor =
            StartCoroutine(InvincibleCor(time));
        }
    }
}
