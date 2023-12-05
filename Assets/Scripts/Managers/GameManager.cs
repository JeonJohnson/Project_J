using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 이제 GameManager 특정 씬에 안만들어도 됨.
/// 에디터에서 실행해도 GameManager 만들어 주는 씬 갔다 돌아가서 ㅋㅋ
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public void CheckStartScene()
    {
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (curSceneIndex != 0)
        {
            Debug.Log("엥 시작씬이 이상한데염");
        }
    }

	private void Awake()
	{
		Debug.Log("GameManager Awake");
        CreateManagerBoxes();
		SetDestructible(false);
	
        

	}
	void Start()
    {
        Debug.Log("GameManager Start");
    }

    void Update()
    {
   
    }


}
