using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;


[InitializeOnLoad]
public static class EditorAppInitializer
{
	//게임의 확실한 시작 지점을 만들기 위해서 제작.
	
	//Release 파일은 어차피 우리가 빌드세팅 해놓은 순서대로 들어가니 상관X
	//But 에디터상 (테스트, 개발 환경에서는) 안그러니까 문제가 좀 생김.
	//그래서 GameManager 생성을 하는 Scene으로 강제로 옮겨주는거
	static EditorAppInitializer()
	{
		Defines.editorStartScene = SceneManager.GetActiveScene().buildIndex;

		string scenePath = EditorBuildSettings.scenes[0].path; 
		var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
		EditorSceneManager.playModeStartScene = sceneAsset;

		Debug.Log($"{Defines.editorStartScene}번째 씬으로 시작 하였으나 초기화를 위해 0번째 씬으로 이동합니다.");
	}
}
