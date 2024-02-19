using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRuneEffectHandler : MonoBehaviour
{
    public List<RuneEffect> runeEffects = new List<RuneEffect>();
    public Player owner;

    private void Awake()
    {
        owner = this.gameObject.GetComponent<Player>();
    }

    public void LoadRuneEffect(string className, int value) //reflection
    {
        // Ŭ���������� Type�� ������
        Type type = Type.GetType(className);

        // Ŭ������ �����ϴ��� Ȯ��
        if (type != null && type.IsSubclassOf(typeof(RuneEffect)))
        {
            // Ŭ������ �ν��Ͻ��� ����
            RuneEffect runeEffect = Activator.CreateInstance(type) as RuneEffect;

            // ������ �ν��Ͻ��� ����Ʈ�� �߰�
            runeEffects.Add(runeEffect);
            runeEffect.RuneInit(owner, value);
        }
        else
        {
            Debug.LogError("RuneEffect class with the name " + className + " does not exist or is not a subclass of RuneEffect.");
        }
    }

    public void LoadRuneEffect(RuneEffect effect, int value)
    {
        runeEffects.Add(effect);
        effect.RuneInit(owner, value);
    }

    public void RemoveRuneEffect(string className) //reflection
    {
        // Ŭ���������� Type�� ������
        Type type = Type.GetType(className);

        // Ŭ������ �����ϴ��� Ȯ��
        if (type != null && type.IsSubclassOf(typeof(RuneEffect)))
        {
            // Ŭ������ �ν��Ͻ��� ����
            RuneEffect runeEffect = Activator.CreateInstance(type) as RuneEffect;

            // ������ �ν��Ͻ��� ����Ʈ�� �߰�
            runeEffects.Remove(runeEffect);
            runeEffect.RuneExit();
        }
        else
        {
            Debug.LogError("RuneEffect class with the name " + className + " does not exist or is not a subclass of RuneEffect.");
        }
    }

    public void RemoveRuneEffect(RuneEffect effect)
    {
        runeEffects.Remove(effect);
        effect.RuneExit();
    }

    private void Update()
    {
        for(int i = 0; i < runeEffects.Count; i++)
        {
            runeEffects[i].RuneEffectUpdate();
        }
    }
}