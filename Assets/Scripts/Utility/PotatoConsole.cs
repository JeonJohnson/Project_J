using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class PotatoConsole : Singleton<PotatoConsole>
{
    //절대 얘 자체는 끄지마세용
    //얘는 계속 로그 받고있어야함!
    [SerializeField]    private GameObject holder;

	[Space(10f)]
	[SerializeField] private TextMeshProUGUI msgDisplay;
    private string allMsg;

	[Space(10f)]
	[SerializeField] private TMP_InputField input;
    [Space(10f)]
	[SerializeField]private Scrollbar scrollbar;
	[SerializeField]private ScrollRect scrollRect;

	public void Print(string message, Potato.LogType type = Potato.LogType.Normal)
    {
        string tempStr = message;

        string timeFontSize = ((int)(msgDisplay.fontSize * 0.7)).ToString();

		string time = $"<size={timeFontSize}><color=#808080>(" + DateTime.Now.ToString("HH:mm:ss") + ")</color></size>";

        switch (type)
        {
            case Potato.LogType.Normal:
                {
                    tempStr = "[Log] " + message;
                }
                break;
            case Potato.LogType.Warning:
                {
                    tempStr = "<color=yellow>[Warning] " + message + "</color>";
                    //tempStr = "<color=#FFFF00>" + message + "</color>";
                }
                break;
            case Potato.LogType.Error:
                {
                    tempStr = "<b><color=red>[Error] " + message + "</color></b>";
                    //tempStr = "<b><color=#FF0000>" + message + "</color></b>";
                }
                break;

            case Potato.LogType.Input:
                {
                    tempStr = "<color=#C0C0C0> > " + message + "</color>";
                }
                break;

            default:
                break;
        }

        allMsg += $"{time} {tempStr}\n";
        msgDisplay.text = allMsg;


        //요거 나중에 한 프레임 뒤에 설정되도록 코루틴 ㄱㄱ
        //scrollbar.value = 0f;
        //scrollRect.normalizedPosition = new Vector2(0f, 0f);
        StartCoroutine(ScrollBarCor());
    }

    private IEnumerator ScrollBarCor()
    {
        yield return null;
		scrollbar.value = 0f;
		scrollRect.normalizedPosition = new Vector2(0f, 0f);
	}

    private void InputConsole()
    {
        if (!holder.activeSelf)
        {
            return;
        }

		if (Input.GetKeyDown(KeyCode.Return))
		{
			string inputTxt = input.text;

			if (!inputTxt.Trim().Equals(string.Empty))
			{
				Print(inputTxt, Potato.LogType.Input);

				input.text = string.Empty;

			}
			input.Select();
		}
	}


    public void Active(bool active)
    {
        holder.SetActive(active);
    }

	public void Clear()
    {
        allMsg = string.Empty;
        msgDisplay.text = allMsg;
    }

	private void Awake()
	{
        Initailize(false);
		msgDisplay.text = string.Empty;
        //msg = new List<string>();

        
	}

	private void Start()
	{
        Active(false);
	}

	void Update()
    {

        InputConsole();


		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			Active(!holder.activeSelf);
		}

        if (holder.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
			Active(false);
		}
	}

    public override void OnEnable()
    {

    }


    public override void OnDisable()
    {
        StopCoroutine(ScrollBarCor());
	}
}
