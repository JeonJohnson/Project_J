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
    public virtual void RuneEffectUse(){ } // 상호작용 키 눌렀을때만 사용될 함수
    public virtual void RuneExit() { }
}

public class RuneEffect_HealOnKill : RuneEffect
{
    //받아올거 : 적 처치 체크(총알에서 받기)

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
