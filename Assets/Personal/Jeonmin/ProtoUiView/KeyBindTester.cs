using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyBindTester : MonoBehaviour
{
    public InputBinding inputBinding;
    private Dictionary<UserAction, KeyCode> _bindingDict;

    void Start()
    {
        inputBinding = new InputBinding();
        inputBinding.LoadFromFile();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            inputBinding.Bind(UserAction.Jump, KeyCode.L);
        }
        else if(Input.GetKeyDown(KeyCode.Y))
        {
            inputBinding.Bind(UserAction.Jump, KeyCode.Space);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            inputBinding.SaveToFile();
        }

        if (Input.GetKeyDown(inputBinding.Bindings[UserAction.Jump]))
        {
            Debug.Log("지정키 실행");
        }
    }
}
