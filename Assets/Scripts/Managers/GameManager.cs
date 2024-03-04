using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Debug = Potato.Debug;

/// <summary>
/// 이제 GameManager 특정 씬에 안만들어도 됨.
/// 에디터에서 실행해도 GameManager 만들어 주는 씬 갔다 돌아가서 ㅋㅋ
/// </summary>
/// 

public struct GameOption
{
    //static 으로 만들어주기 
    public float bgmVolume;
    public float effectVolume;

    public float mouseSensitivity;

    //키 바인딩

    //해상도
    
    //따로 데이터 때서 로컬 파일로 저장해둔 뒤에 게임 시작할대 불러오기
}

public enum SceneName
{ 
    Intro = 1,
    Title,
    Ingame,
    End
}
public class GameManager : Singleton<GameManager>
{
   //public static GameOption option = new GameOption();

    [SerializeField]
    private SceneLoader sceneLoader;

    public void LoadScene(int sceneIndex)
    {
        
        if (sceneLoader.isSceneLoading) { Debug.LogWarning("씬이 이미 불러와지는 중입니다"); return; };
        sceneLoader.LoadScene(sceneIndex);
        //좀 있다가 씬 로더 ㄱㄱ
    }

    public void LoadScene(SceneName scene)
    {
        int sceneIndex = (int)scene;
        if (sceneLoader.isSceneLoading) { Debug.LogWarning("씬이 이미 불러와지는 중입니다"); return; };
        sceneLoader.LoadScene(sceneIndex);
        //좀 있다가 씬 로더 ㄱㄱ
    }

    public void LoadNextScene()
    {
        int curIndex = SceneManager.GetActiveScene().buildIndex;

        LoadScene(curIndex + 1);
    }

    public void SubscribeLoadingEvent(UnityAction func, int curScene, int loadScene)
    {
        sceneLoader.OnLoadingEvents[curScene] += func;
            
    }

    public void CancleLoadingEvent(UnityAction func, int curScene, int loadScene)
    {
        sceneLoader.OnLoadingEvents[curScene] -= func;
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
        switch ((SceneName)sceneIndex)
        {
            case SceneName.Intro:
                break;
            case SceneName.Title:
                {
                    SoundManager.Instance.PlayBGM("BGM_Title");
                }
                break;
            case SceneName.Ingame:
                {
					SoundManager.Instance.PlayBGM("BGM_Ingame_0");
				}
                break;
            case SceneName.End:
                break;
            default:
                break;
        }

    }

	private void Awake()
	{
        Initailize(false);
		//Debug.Log("GameManager Awake");

        Application.targetFrameRate = 60;

	}
	void Start()
    {
        //Debug.Log("GameManager Start");

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
