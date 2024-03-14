using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class NPC : MonoBehaviour
{

    public float fontSize = 30;
    public string fontNayong = "안아줘요";

    public void LoadTalkData(int talkDataIndex)
    {
        // talkDataIndex 번의 대화 데이터들을 엑셀에서 읽어서 curTalkData 에 순서대로 넣어줌
    }

    public SpeechBubble curSpeechBubble;

    private void Awake()
    {
        
    }

    private void Update()
    {
    }

    public void Talk(string context, float size, float speed)
    {
        if (curSpeechBubble != null) PoolingManager.Instance.ReturnObj(curSpeechBubble.gameObject);
        curSpeechBubble = PoolingManager.Instance.LentalObj("SpeechBubble", 1).GetComponent<SpeechBubble>();

        curSpeechBubble.Init(this.gameObject);
        curSpeechBubble.Speech(context, size, speed);
    }
}
