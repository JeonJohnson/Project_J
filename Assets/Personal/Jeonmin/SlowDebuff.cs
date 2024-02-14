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
        // �ֱ������� ���ظ� ������ �ڷ�ƾ ����
        slowCoroutine = StartCoroutine(SlowOverTime());
        
    }

    public override void RemoveEffect()
    {
        // ����� ���� �� ���� �ڷ�ƾ �ߴ�
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