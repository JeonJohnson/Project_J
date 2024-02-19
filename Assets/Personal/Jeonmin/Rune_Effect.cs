using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RuneEffect
{
    public Player owner;
    public int effect_value;
    public virtual void RuneInit(Player player, int value)
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
    public override void RuneInit(Player player, int value)
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
        owner.status.curHp.Value += effect_value;
        Debug.Log("��");
    }
}

public class RuneEffect_ExplodeOnKill : RuneEffect
{
    public override void RuneInit(Player player, int value)
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
        particle.transform.localScale = new Vector2(effect_value,effect_value);

        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, effect_value, layerMask); // ���� ���� ���� ��� �浹ü�� ������

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
    public override void RuneInit(Player player, int value)
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
        SlowDebuff slowDebuff = new SlowDebuff(effect_value, enemy, 5f);
        enemy.statusEffect.Add(slowDebuff);
        slowDebuff.ApplyEffect();
    }
}
