using System.Collections;
using UnityEngine;

public class SlowDebuff : StatusEffect
{
    private float slowPercent;
    private Coroutine slowCoroutine;

    public SlowDebuff(float duration, CObj target, float slowPercent) : base(duration, target)
    {
        this.slowPercent = slowPercent;
    }

    public override void ApplyEffect()
    {
        // 주기적으로 피해를 입히는 코루틴 시작
        slowCoroutine = StartCoroutine(SlowOverTime());
        
    }

    public override void RemoveEffect()
    {
        // 디버프 종료 시 피해 코루틴 중단
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        if (target is Enemy)
        {
            Enemy enemy = (Enemy)target;
            enemy.agent.speed = enemy.status.walkSpeed;
        }
        else if (target is Player)
        {

        }
    }

    private IEnumerator SlowOverTime()
    {
        float timer = this.duration;
        while (this.duration > 0f)
        {
            this.duration -= timer;
            DealSlowToTarget(this.slowPercent);
            yield return null;
        }
        RemoveEffect();
    }

    private void DealSlowToTarget(float slowPercent)
    {
        if(target is Enemy)
        {
            Enemy enemy = (Enemy)target;
            enemy.agent.speed = enemy.status.walkSpeed / slowPercent;
        }
        else if (target is Player)
        {

        }
    }
}