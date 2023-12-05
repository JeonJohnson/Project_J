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
    //    // e_Item_Active_action�� ���õ� ������ �ν��ؼ� �����̸��� Item_Active_Actions Ŭ������ �Լ����� UseAction �� �߰��� ��
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

//��ũ���ͺ� ������Ʈ�� ����� ����� �ϸ� ������ ����. �������� �ȵǴ°� ������ ������ �ϳ��� �ȶ�(?)
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
        // player�� ������ �뽬�ϴ� ��ũ��Ʈ
    }

    public static void AddBonusStat()
    {
        Player player = IngameController.Instance.player;
        // player ���ݹ����� 5�ʵ��� �����ϴ� ��ũ��Ʈ
    }
}