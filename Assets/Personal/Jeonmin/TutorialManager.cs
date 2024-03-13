using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct TalkData
{
    public string name;
    public string context;
}

[System.Serializable]
public class DialogueFlow
{
    public List<TalkData> flow;

    public DialogueFlow()
    {
        flow = new List<TalkData>();
    }

    public void AddTalkData(string name, string context)
    {
        TalkData talkData = new TalkData();
        talkData.name = name;
        talkData.context = context;
        flow.Add(talkData);
    }
}


public class TutorialManager : MonoBehaviour
{
    private string filePath = "Assets/Resources/Data/TalkData.csv";

    public List<DialogueFlow> dialogueFlows;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            LoadDialogueData(filePath);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log(dialogueFlows[0].flow[2].context);
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
                    currentFlow.AddTalkData(npcName, speech);
                }
            }
        }
    }
}
