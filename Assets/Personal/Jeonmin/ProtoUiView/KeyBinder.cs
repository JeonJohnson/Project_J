using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum UserAction
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,

    Attack,
    Run,
    Jump,

    // UI
    UI_Inventory,
    UI_Status,
    UI_Skill,
}

public class KeyBinder
{
    private Dictionary<UserAction, KeyCode> _bindingDict;
}

[Serializable]
public class InputBinding
{
    string filePath = "Assets/Resources//Data/bindings.json";

    public Dictionary<UserAction, KeyCode> Bindings => _bindingDict;
    private Dictionary<UserAction, KeyCode> _bindingDict;

    // ������
    public InputBinding(bool initalize = true)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>();

        if (initalize)
        {
            ResetAll();
        }
    }

    // ���ο� ���ε� ����
    public void ApplyNewBindings(InputBinding newBinding)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>(newBinding._bindingDict);
    }

    // ���ε� ���� �޼ҵ� : allowOverlap �Ű������� ���� �ߺ� ���ε� ��뿩�θ� �����Ѵ�.
    public void Bind(in UserAction action, in KeyCode code, bool allowOverlap = false)
    {
        if (!allowOverlap && _bindingDict.ContainsValue(code))
        {
            var copy = new Dictionary<UserAction, KeyCode>(_bindingDict);

            foreach (var pair in copy)
            {
                if (pair.Value.Equals(code))
                {
                    _bindingDict[pair.Key] = KeyCode.None;
                }
            }
        }
        _bindingDict[action] = code;
    }

    // �ʱ� ���ε��� ���� �޼ҵ�
    public void ResetAll()
    {
        Bind(UserAction.Attack, KeyCode.Mouse0);

        Bind(UserAction.MoveForward, KeyCode.W);
        Bind(UserAction.MoveBackward, KeyCode.S);
        Bind(UserAction.MoveLeft, KeyCode.A);
        Bind(UserAction.MoveRight, KeyCode.D);

        Bind(UserAction.Run, KeyCode.LeftControl);
        Bind(UserAction.Jump, KeyCode.Space);

        Bind(UserAction.UI_Inventory, KeyCode.I);
        Bind(UserAction.UI_Status, KeyCode.P);
        Bind(UserAction.UI_Skill, KeyCode.K);
    }


    // ������
    public InputBinding(SerializableInputBinding sib)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>();

        foreach (var pair in sib.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }

    public void ApplyNewBindings(SerializableInputBinding newBinding)
    {
        _bindingDict.Clear();

        foreach (var pair in newBinding.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }
    public void SaveToFile()
    {
        SerializableInputBinding sib = new SerializableInputBinding(this);
        string jsonStr = JsonUtility.ToJson(sib);
        File.WriteAllText(filePath, jsonStr);
    }

    public void LoadFromFile()
    {
        string jsonData;
        if (File.Exists(filePath))
        {
            jsonData = File.ReadAllText(filePath);
            _bindingDict = JsonUtility.FromJson<Dictionary<UserAction, KeyCode>>(jsonData);
        }
        else
        {
            return;
        }

        var sib = JsonUtility.FromJson<SerializableInputBinding>(jsonData);
        ApplyNewBindings(sib);
    }
}

[Serializable]
public class SerializableInputBinding
{
    public BindPair[] bindPairs;

    public SerializableInputBinding(InputBinding binding)
    {
        int len = binding.Bindings.Count;
        int index = 0;

        bindPairs = new BindPair[len];

        foreach (var pair in binding.Bindings)
        {
            bindPairs[index++] =
                new BindPair(pair.Key, pair.Value);
        }
    }
}

[Serializable]
public class BindPair
{
    public UserAction key;
    public KeyCode value;

    public BindPair(UserAction key, KeyCode value)
    {
        this.key = key;
        this.value = value;
    }
}