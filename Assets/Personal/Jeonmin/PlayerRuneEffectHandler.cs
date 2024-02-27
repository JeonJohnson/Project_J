using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

using Debug = Potato.Debug;
public class PlayerRuneEffectHandler : MonoBehaviour
{
    public List<RuneEffect> runeEffects = new List<RuneEffect>();
    public Player owner;

    private void Awake()
    {
        owner = this.gameObject.GetComponent<Player>();
    }

    public RuneEffect LoadRuneEffect(string className, SerializedDictionary<string, int> value) //reflection
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

            return runeEffect;
        }
        return null;
    }

    public void LoadRuneEffect(RuneEffect effect, SerializedDictionary<string, int> value)
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
            Debug.Log("�� ���� ����");

            RuneEffect runeEffect = Activator.CreateInstance(type) as RuneEffect;

            RemoveRuneEffect(runeEffect);
        }
    }

    public void RemoveRuneEffect(RuneEffect effect)
    {
        if (effect == null) return;
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
