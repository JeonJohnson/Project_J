using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class NPC : MonoBehaviour
{

    public float fontSize = 30;
    public string fontNayong = "�Ⱦ����";

    public void LoadTalkData(int talkDataIndex)
    {
        // talkDataIndex ���� ��ȭ �����͵��� �������� �о curTalkData �� ������� �־���
    }

    public SpeechBubble curSpeechBubble;

    private void Update()
    {
    }

    public void Talk(string context)
    {
        if (curSpeechBubble != null) PoolingManager.Instance.ReturnObj(curSpeechBubble.gameObject);
        curSpeechBubble = PoolingManager.Instance.LentalObj("SpeechBubble", 1).GetComponent<SpeechBubble>();

        curSpeechBubble.Init(this.gameObject);
        curSpeechBubble.Speech(fontNayong, fontSize);

        Debug.Log("��");
    }
}
