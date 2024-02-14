using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    protected float duration;
    protected CObj target;

    public StatusEffect(float duration, CObj target)
    {
        this.duration = duration;
        this.target = target;
    }

    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}