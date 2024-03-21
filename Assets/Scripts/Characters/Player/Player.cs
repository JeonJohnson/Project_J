using MoreMountains.Feedbacks;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = Potato.Debug;
public class Player : CObj
{
    private PlayerMoveActionTable moveActionTable;
    public PlayerMoveActionTable MoveActionTable { get { return moveActionTable; } set { moveActionTable = value; } }

    public PlayerAimController aimController;

    public AfterImageEffectController afterImageController;

    public PlayerAttackController attackController;

    public CamCtrl camController;

    public PlayerInventroy inventroy;

    public PlayerRuneEffectHandler runeEffectHandler;

    public PlayerInput playerInput;

    public Shop shop;

    public Rigidbody2D PlayerRigidbody2D { get; private set; }
    public Animator animator { get; private set; }

    public PlayerStatus status;
    public BonusStatus bonusStatus;
    public Transform spriteHolder;
    public Transform weaponHolder;
    public Weapon_Player curWeapon;

    [Header("Feedbacks")]
    [SerializeField] MMBlink hitBlink;
    [SerializeField] MMF_Player hitFeedback;
    public ParticleSystem footstepParticle;

    private Coroutine invincibleCor;

    private void Awake()
    {
        InitializePlayer();

	}

    public void InitializePlayer()
    {
        PlayerRigidbody2D = GetComponent<Rigidbody2D>();
        moveActionTable = GetComponent<PlayerMoveActionTable>();
        aimController = GetComponent<PlayerAimController>();
        afterImageController = spriteHolder.GetComponent<AfterImageEffectController>();
        animator = spriteHolder.GetComponent<Animator>();
        attackController = GetComponent<PlayerAttackController>();
        inventroy = GetComponent<PlayerInventroy>();
        runeEffectHandler = GetComponent<PlayerRuneEffectHandler>();
        playerInput = GetComponent<PlayerInput>();
        shop = GetComponent<Shop>();

        status.curHp = new Data<int>();
        status.curHp.Value = status.maxHp;
        Debug.Log("이닛 플레이어");
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        if (!status.isInvincible)
        { 
            status.curHp.Value -= 1;
            SetInvincible(status.invincibleTimeWhenHit);
            //SoundManager.Instance.PlaySound("Player_Hit",gameObject);
        }
        //PlayerRigidbody2D.AddForce(dir * 500f);

        if(status.curHp.Value <= 0 && !status.isDead)
        {
            Dead();
        }


        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }

    private void Dead()
    {
		//SoundManager.Instance.PlaySound("Player_Death", gameObject);
		status.isDead = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        PlayerRigidbody2D.simulated = false;
        spriteHolder.gameObject.SetActive(false);
        weaponHolder.gameObject.SetActive(false);
        aimController.enabled = false;
        aimController.camCtrl.enabled = false;
        moveActionTable.enabled = false;
        attackController.enabled = false;

        //if(IngameController.Instance.gameStatus == IngameController.GameStatus.Playing) UiController_Proto.Instance.ShowResultWindow(true, false);
        IngameController.Instance.GameOver(false);
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
            invincibleCor = StartCoroutine(InvincibleCor(time));
        }
        else
        {
            if (invincibleCor != null)
            {
                StopCoroutine(invincibleCor);
            }
            invincibleCor = StartCoroutine(InvincibleCor(time));
        }
    }

    public void LockPlayer(bool value)
    {
        if (value)
        {
            camController.enabled = false;
            aimController.enabled = false;
            attackController.enabled = false;
            moveActionTable.enabled = false;
            Time.timeScale = 0f;
        }
        else
        {
            camController.enabled = true;
            aimController.enabled = true;
            attackController.enabled = true;
            moveActionTable.enabled = true;
            Time.timeScale = 1.0f;
        }
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.O))
        {
            SetInvincible(99999);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventroy.bulletCount.Value = 999;
        }
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    StopCoroutine(invincibleCor);
        //}
    }
}
