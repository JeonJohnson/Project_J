using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class IngameLoader : MonoBehaviour
{

	public Button TestButton;
	//여기서 씬 이동하는 연출은 일단 로딩에 대해서 더 연구해보고 진행

	//테스트 방법 1
	//IngameLoading 씬에서 Stage제작.
	//비동기&Additive 옵션으로 먼저 IngameScene 불러오고 
				//이까지가 로드씬 0.9
	//그 다음에 IngameController 만들어진거 확인 했으면 거기다가 넣어주기
	
	//그 다음 ingameLoading씬 언로드 해주기 
			//이까지도 0.9

	//이 두단계를 모두 로딩으로 치기

	public StageGenerator SG;
	//public IngameController ctlr;

	public Stage tempStage;

	public void TestEvent()
	{
		SceneManager.sceneLoaded += GiveStage;
	}

	public void GiveStage(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != 5)
		{
			return;
		}
		IngameController.Instance.stage = tempStage;

		SceneManager.sceneLoaded -= GiveStage;
		Destroy(this.gameObject);
	}

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		//ctlr = IngameController.Instance;

		tempStage = SG.CreateStage();
		DontDestroyOnLoad(tempStage.gameObject);
		TestEvent();

		TestButton.onClick.AddListener(()=> GameManager.Instance.LoadNextScene());
	}

	
	private void Update()
	{
		
		
	}


	//IEnumerator TestLoading()
	//{
	//	Debug.Log("씬로드 시작");
	//	var OP = SceneManager.LoadSceneAsync((int)SceneName.Ingame, LoadSceneMode.Additive);
	//	OP.allowSceneActivation = false;
	//	SG.CreateStage();

	//	Debug.Log(OP.progress);
	//	yield return OP;
	//	Debug.Log("씬로드 끝");

	//	Debug.Log("씬 언로드 시작");
	//	OP = SceneManager.UnloadSceneAsync((int)SceneName.IngameLoading);
	//	Debug.Log(OP.progress);
	//	yield return OP;

	//}

	

	private void OnDestroy()
	{
		SG = null;
		//ctlr = null;
	}
}
