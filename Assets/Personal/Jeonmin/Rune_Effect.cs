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
    public virtual void RuneEffectUse(){ } // 상호작용 키 눌렀을때만 사용될 함수
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
        Debug.Log("힐");
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
        Debug.Log("펑~~~~");
        GameObject particle = PoolingManager.Instance.LentalObj("Effect_Magic_00");
        particle.transform.position = enemy.transform.position;
        particle.transform.localScale = new Vector2(effect_value,effect_value);

        LayerMask layerMask = LayerMask.GetMask("Enemy");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, effect_value, layerMask); // 원형 범위 내의 모든 충돌체를 가져옴

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
        Debug.Log("슬로우 시작");
        SlowDebuff slowDebuff = new SlowDebuff(effect_value, enemy, 5f);
        enemy.statusEffect.Add(slowDebuff);
        slowDebuff.ApplyEffect();
    }
}
