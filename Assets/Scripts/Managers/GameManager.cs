using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 이제 GameManager 특정 씬에 안만들어도 됨.
/// 에디터에서 실행해도 GameManager 만들어 주는 씬 갔다 돌아가서 ㅋㅋ
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private SceneLoader sceneLoader;

    public void LoadScene(int sceneIndex)
    {
        //SceneManager.LoadScene(sceneIndex);
        if (sceneLoader.isSceneLoading) { Debug.LogWarning("씬이 이미 불러와지는 중입니다"); return; };
        sceneLoader.LoadScene(sceneIndex);
        //좀 있다가 씬 로더 ㄱㄱ
    }

    public void LoadNextScene()
    {
        int curIndex = SceneManager.GetActiveScene().buildIndex;

        LoadScene(curIndex + 1);
    }

    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
  Application.Quit();
#endif

	}

	private void AppInitCheck()
    {
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (curSceneIndex != 0)
        {
            Debug.Log("엥 시작씬이 이상한데염");
        }
        else 
        {
            Debug.Log("GameManager 잘 만들었구연, 씬1로 갑니당");

#if UNITY_EDITOR
            SceneManager.LoadScene(Defines.editorStartScene);
#else
			SceneManager.LoadScene(1);
#endif

		}
    }



    public void InitializeScene(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 1:
                { }
                break;

            case 2:
                { }
                break;

			case 3:
				{ }
                break;

            case 4:
                { }
                break;

            default:
                break;
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

		AppInitCheck();
	}

    void Update()
    {
        
    }

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
        InitializeScene(scene.buildIndex);
	}

}
