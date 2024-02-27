using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using static UnityEngine.RuleTile.TilingRuleOutput;

using Debug = Potato.Debug;
public abstract class RuneEffect
{
    public Player owner;
    public SerializedDictionary<string, int> effect_value;
    public virtual void RuneInit(Player player, SerializedDictionary<string, int> value)
    { 
        if(owner == null) { owner = player; }
        effect_value = value;
    }
    public virtual void RuneEffectUpdate() { }
    public virtual void RuneEffectUse(){ } // 상호작용 키 눌렀을때만 사용될 함수
    public virtual void RuneExit() { }
}

public class RuneEffect_HealOnKill : RuneEffect
{
    public override void RuneInit(Player player, SerializedDictionary<string, int> value)
    {
        base.RuneInit(player, value);
        if (StageManager.Instance) StageManager.Instance.killCount.onChange += Heal;
    }

    public override void RuneExit() 
    {
        if (StageManager.Instance) StageManager.Instance.killCount.onChange -= Heal;
    }

    private void Heal(int dummyValue)
    {
        int healValue = effect_value["heal"];
        int targetHp = Mathf.Clamp(owner.status.curHp.Value + healValue, 0, owner.status.maxHp);
        owner.status.curHp.Value = targetHp;
        Debug.Log("힐");
    }
}

public class RuneEffect_ExplodeOnKill : RuneEffect
{
    public override void RuneInit(Player player, SerializedDictionary<string, int> value)
    {
        base.RuneInit(player, value);
        if (StageManager.Instance) StageManager.Instance.enemyDeathData.onChange += Explode;
    }

    public override void RuneExit()
    {
        if (StageManager.Instance) StageManager.Instance.enemyDeathData.onChange -= Explode;
    }

    private void Explode(Enemy enemy)
    {
        Debug.Log("펑~~~~");
        GameObject particle = PoolingManager.Instance.LentalObj("Effect_Magic_00");
        particle.transform.position = enemy.transform.position;

        float explosionScale = effect_value["explosionScale"];
        particle.transform.localScale = new Vector2(explosionScale, explosionScale);

        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, explosionScale / 2, layerMask); // 원형 범위 내의 모든 충돌체를 가져옴

        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy hitEnemy = colliders[i].gameObject.GetComponent<Enemy>(); // 충돌체에 붙어있는 Enemy 컴포넌트 가져오기
            if (hitEnemy != null)
            {
                hitEnemy.Hit(3, Vector2.zero); // Enemy의 Hit 메서드 호출
            }
        }
    }
}

public class RuneEffect_SlowOnEnemyHit : RuneEffect
{
    public override void RuneInit(Player player, SerializedDictionary<string, int> value)
    {
        base.RuneInit(player, value);
        if (StageManager.Instance) StageManager.Instance.enemyHitData.onChange += Slow;
    }

    public override void RuneEffectUpdate()
    {

    }

    public override void RuneExit()
    {
        if (StageManager.Instance) StageManager.Instance.enemyHitData.onChange -= Slow;
    }

    private void Slow(Enemy enemy)
    {
        Debug.Log("슬로우 시작");
        int slowValue = effect_value["slow"];
        SlowDebuff slowDebuff = new SlowDebuff(slowValue, enemy, 5f);
        enemy.statusEffect.Add(slowDebuff);
        slowDebuff.ApplyEffect();
    }
}

public class RuneEffect_PlayerThrowWeapon : RuneEffect
{
    float throwCooldownTimer;
    private Throwable throwable;

    private bool isThrown;

    public override void RuneInit(Player player, SerializedDictionary<string, int> value)
    {
        base.RuneInit(player, value);
        throwable = owner.curWeapon.gameObject.GetComponent<Throwable>();
        throwable.triggedCObj.onChange += OnWeaponHit;
    }

    public override void RuneEffectUpdate()
    {
        throwCooldownTimer -= Time.deltaTime;
        throwCooldownTimer = Mathf.Clamp(throwCooldownTimer, 0, 99999);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!throwable.isThrow && throwCooldownTimer <= 0f)
            {
                Throw();
                throwCooldownTimer = 2f;
            }

            if(throwable.isThrow && throwCooldownTimer <= 0f)
            {
                throwable.Return();
                throwCooldownTimer = 5f;
            }
        }


        if(throwable.isThrow)
        {
            RotateWeapon();
        }
    }

    public override void RuneExit()
    {
        throwable.triggedCObj.onChange -= OnWeaponHit;
        throwable.Return();
    }

    private void Throw()
    {
        int throwRange = effect_value["throwPower"];
        throwable.Throw(throwRange);
    }

    private void RotateWeapon()
    {
        GameObject weaponObj = throwable.gameObject;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // 2D 게임에서 z값은 0으로 설정

        Vector3 direction = mousePosition - weaponObj.transform.position;
        direction.Normalize(); // 방향 벡터 정규화

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float currentRotation = Mathf.LerpAngle(weaponObj.transform.eulerAngles.z, angle - 90f, 10f * Time.deltaTime);
        weaponObj.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
    }

    private void OnWeaponHit(CObj cObj)
    {
        Rigidbody2D rb = throwable.gameObject.GetComponent<Rigidbody2D>();
        float thrownWeaponSpeed = Mathf.Abs(rb.velocity.x + rb.velocity.y);
        Debug.Log(rb.velocity);
        if (thrownWeaponSpeed > 0.2f)
        {
            int dmg = effect_value["damage"];
            cObj.Hit(dmg, Vector3.zero);
        }
    }
}
