using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct TalkData
{
    public string name;
    public string context;
    public float fontSize;
    public float fontSpeed;
}

[System.Serializable]
public class DialogueFlow
{
    public List<TalkData> flow;

    public DialogueFlow()
    {
        flow = new List<TalkData>();
    }

    public void AddTalkData(string name, string context, float size, float speed)
    {
        TalkData talkData = new TalkData();
        talkData.name = name;
        talkData.context = context;
        talkData.fontSize = size;
        talkData.fontSpeed = speed;
        flow.Add(talkData);
    }
}


public class TutorialManager : Singleton<TutorialManager>
{
    private void Awake()
    {
        Initailize(true);
    }

    private string filePath = "Assets/Resources/Data/TalkData.csv";
    public List<DialogueFlow> dialogueFlows;

    private int curEventIndex = 0;
    private int curSpeechIndex = 0;
    [SerializeField]
    public Dictionary<string, GameObject> npcList = new Dictionary<string, GameObject>();

    int textIndex = 0;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            LoadDialogueData(filePath);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            PlayDialogueData();
        }
    }

    public void LoadDialogueData(string filePath)
    {
        dialogueFlows = new List<DialogueFlow>();

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            DialogueFlow currentFlow = null;

            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] parts = line.Split(',');
                int eventIndex;
                if (int.TryParse(parts[0], out eventIndex))
                {
                    if (currentFlow == null || dialogueFlows.Count <= eventIndex)
                    {
                        currentFlow = new DialogueFlow();
                        dialogueFlows.Add(currentFlow);
                    }

                    string npcName = parts[1];
                    string speech = parts[2];
                    float size = float.Parse(parts[3]);
                    float speed = float.Parse(parts[4]);
                    currentFlow.AddTalkData(npcName, speech, size, speed);
                }
            }
        }
    }

    private void PlayDialogueData()
    {
        ReadDialogueData(curEventIndex, curSpeechIndex);

        curSpeechIndex++;
        if (curSpeechIndex >= dialogueFlows[curEventIndex].flow.Count)
        {
            curEventIndex++;
            curSpeechIndex = 0;
        }
    }

    private void ReadDialogueData(int eventIndex, int speechIndex)
    {
        if(curEventIndex != eventIndex)
        {
            curEventIndex = eventIndex;
            curSpeechIndex = speechIndex;

            // search target npcs
        }
        GameObject npcGo;

        if(npcList.TryGetValue(dialogueFlows[eventIndex].flow[speechIndex].name, out npcGo))
        {
            NPC npc = npcGo.GetComponent<NPC>();
            npc.Talk(dialogueFlows[eventIndex].flow[speechIndex].context, dialogueFlows[eventIndex].flow[speechIndex].fontSize, dialogueFlows[eventIndex].flow[speechIndex].fontSpeed);
        }
        else
        {
            Debug.Log(dialogueFlows[eventIndex].flow[speechIndex].name + "을 npc dic 에서 찾을수 없음. 등록했나요?");
        }

    }
}
