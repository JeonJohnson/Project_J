using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Structs;

[CreateAssetMenu(fileName = "New Active Item", menuName = "Scriptable Object/Items/New Active Item")]
public class Item_Active : Item
{
    [HideInInspector] public UnityAction UseAction;
    public Data<float> cooldownTimer = new Data<float>();
    public float cooldownTime;
    private bool isPlaying = false;

    public override bool Equip(Player player)
    {
        cooldownTimer.Value = 0f;
        isPlaying = false;
        InitUseAction();
        player.inventroy.AddItem(this);
        return base.Equip(player);
    }

    public override bool UnEquip(Player player)
    {
        return base.UnEquip(player);
    }

    public override bool Use(Player player)
    {
        if(!isPlaying)
        {
            UseAction?.Invoke();
            player.StartCoroutine(coolDownCoroutine());
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void InitUseAction()
    {
        UseAction = null;
        UseAction += Item_Active_Actions.Dash;
    }

    private IEnumerator coolDownCoroutine()
    {
        isPlaying = true;
        cooldownTimer.Value = 0f;
        while (isPlaying)
        {
            cooldownTimer.Value += Time.deltaTime;
            UiController_Proto.Instance.UpdateActiveItemGauge(cooldownTimer.Value);
            if (cooldownTimer.Value >= cooldownTime) isPlaying = false;
            yield return null;
        }
        cooldownTimer.Value = cooldownTime;
        yield return null;
    }

    //[Flags]
    //public enum e_Item_Active_Actions
    //{
    //    None = 0,
    //    Dash = 1 << 0,
    //    AddAttackRange = 1 << 1,
    //    AddAttackPower = 1 << 2
    //}

    //public e_Item_Active_Actions e_Item_Active_action;

    //public void InitActionActions()
    //{
    //    UseAction = null;
    //    // e_Item_Active_action의 선택된 값들을 인식해서 동일이름의 Item_Active_Actions 클래스상 함수들을 UseAction 에 추가해 줌
    //    Type actionsType = typeof(Item_Active_Actions);
    //    e_Item_Active_Actions selectedActions = e_Item_Active_action;

    //    foreach (e_Item_Active_Actions action in Enum.GetValues(typeof(e_Item_Active_Actions)))
    //    {
    //        if ((selectedActions & action) == action)
    //        {
    //            MethodInfo method = actionsType.GetMethod(action.ToString());
    //            if (method != null)
    //            {
    //                UseAction += (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), new Item_Active_Actions(), method);
    //            }
    //        }
    //    }
    //}
}

//스크립터블 오브젝트를 상속의 상속을 하면 좇버그 터짐. 컴파일이 안되는거 같은데 오류가 하나도 안뜸(?)
//[CreateAssetMenu(fileName = "New Active Dash Item", menuName = "Scriptable Object/Items/New Active Dash Item")]
//public class Item_Active_Dash : Item_Active
//{
//    public override void InitUseAction()
//    {
//        base.InitUseAction();
//        UseAction += Item_Active_Actions.Dash;
//    }
//}

//[CreateAssetMenu(fileName = "New Active AddBonus Item", menuName = "Scriptable Object/Items/New Active AddBonus Item")]
//public class Item_Active_AddBonus : Item_Active
//{
//    public float duration;
//    private float duration_Timer;
//    public float Duration_Timer { get { return duration_Timer; } }

//    public override void InitUseAction()
//    {
//        base.InitUseAction();
//        UseAction += Item_Active_Actions.AddBonusStat;
//    }

//    private IEnumerator durationCoroutine()
//    {
//        Item_Active_Actions.AddBonusStat();

//        bool isPlaying = true;
//        duration_Timer = duration;
//        while (isPlaying)
//        {
//            duration_Timer -= Time.deltaTime;
//            if (duration_Timer < 0) isPlaying = false;
//            yield return null;
//        }

//        Item_Active_Actions.AddBonusStat();
//        yield return null;
//    }
//}


public static class Item_Active_Actions
{
    public static void Dash()
    {
        Player player = IngameController.Instance.player;
        player.MoveActionTable.SetCurAction((int)PlayerMoveActions.Roll);
        // player가 앞으로 대쉬하는 스크립트
    }

    public static void AddBonusStat()
    {
        Player player = IngameController.Instance.player;
        // player 공격범위가 5초동안 증가하는 스크립트
    }
}