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
        // 클래스명으로 Type을 가져옴
        Type type = Type.GetType(className);

        // 클래스가 존재하는지 확인
        if (type != null && type.IsSubclassOf(typeof(RuneEffect)))
        {
            // 클래스의 인스턴스를 생성
            RuneEffect runeEffect = Activator.CreateInstance(type) as RuneEffect;

            // 생성된 인스턴스를 리스트에 추가
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
        // 클래스명으로 Type을 가져옴
        Type type = Type.GetType(className);

        // 클래스가 존재하는지 확인
        if (type != null && type.IsSubclassOf(typeof(RuneEffect)))
        {
            // 클래스의 인스턴스를 생성
            RuneEffect runeEffect = Activator.CreateInstance(type) as RuneEffect;

            // 생성된 인스턴스를 리스트에 추가
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
