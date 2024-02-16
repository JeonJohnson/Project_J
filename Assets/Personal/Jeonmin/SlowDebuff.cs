using System.Collections;
using UnityEngine;

public class SlowDebuff : StatusEffect
{
    private float slowPercent;
    private Coroutine slowCoroutine;

    public SlowDebuff(float duration, CObj target, float slowPercent) : base(duration, target)
    {
        this.slowPercent = slowPercent;
        this.target = target;
        this.duration = duration;
    }

    public override void ApplyEffect()
    {
        slowCoroutine = target.StartCoroutine(SlowOverTime());
    }

    public override void RemoveEffect()
    {
        if (slowCoroutine != null)
        {
            target.StopCoroutine(slowCoroutine);
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
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            DealSlowToTarget(this.slowPercent);
            Debug.Log("슬로우중");
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