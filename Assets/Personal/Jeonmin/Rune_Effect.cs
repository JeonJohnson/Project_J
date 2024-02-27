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
    public virtual void RuneEffectUse(){ } // ��ȣ�ۿ� Ű ���������� ���� �Լ�
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
        Debug.Log("��");
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
        Debug.Log("��~~~~");
        GameObject particle = PoolingManager.Instance.LentalObj("Effect_Magic_00");
        particle.transform.position = enemy.transform.position;

        float explosionScale = effect_value["explosionScale"];
        particle.transform.localScale = new Vector2(explosionScale, explosionScale);

        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, explosionScale / 2, layerMask); // ���� ���� ���� ��� �浹ü�� ������

        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy hitEnemy = colliders[i].gameObject.GetComponent<Enemy>(); // �浹ü�� �پ��ִ� Enemy ������Ʈ ��������
            if (hitEnemy != null)
            {
                hitEnemy.Hit(3, Vector2.zero); // Enemy�� Hit �޼��� ȣ��
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
        Debug.Log("���ο� ����");
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
        mousePosition.z = 0f; // 2D ���ӿ��� z���� 0���� ����

        Vector3 direction = mousePosition - weaponObj.transform.position;
        direction.Normalize(); // ���� ���� ����ȭ

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
