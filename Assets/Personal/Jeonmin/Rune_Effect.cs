using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RuneEffect : MonoBehaviour
{
    public Player owner;
    public virtual void RuneInit(Player player)
    { 
        if(owner == null) { owner = player; }
    }
    public virtual void RuneEffectUpdate() { }
    public virtual void RuneEffectUse(){ } // ��ȣ�ۿ� Ű ���������� ���� �Լ�
    public virtual void RuneExit() { }
}

public class RuneEffect_HealOnKill : RuneEffect
{
    //�޾ƿð� : �� óġ üũ(�Ѿ˿��� �ޱ�)

    public override void RuneInit(Player player)
    {
        if (StageManager.Instance) StageManager.Instance.killCount.onChange += Heal;
    }

    public override void RuneExit() 
    {
        if (StageManager.Instance) StageManager.Instance.killCount.onChange -= Heal;
    }

    private void Heal(int dummyValue)
    {
        owner.status.curHp.Value += 1;
    }
}
