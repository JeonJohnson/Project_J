using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(DungeonGenerator_Isaac))]
public class DungeonGenerator_Isaac_Editor : Editor
{
	//bool isInit = false;
	public override void OnInspectorGUI()
	{
		DungeonGenerator_Isaac generator = (DungeonGenerator_Isaac)target;
		
		DrawDefaultInspector();


		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		//GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);

		//1. 초기화 / 구역 나눠주기
		//기본 바닥 만들어 주기


		GUILayout.Label("");
		if (GUILayout.Button("Test!!!!"))
		{
			//for (int i = 0; i < generator.createRoomCount; ++i)
			//{
			//	generator.CreateRoom();
			//}
			//generator.GotoGameScene();

			generator.CreateAnotherRoom();

		}


			if (GUILayout.Button("Create One Room"))
		{
			//generator.CreateRoom();
		}


		//GUILayout.Label("");
		//if (GUILayout.Button("Add Room"))
		//{
		//	generator.Setup();
		//	generator.CreateRoom();
		//	//generator.Reset();
		//}

		GUILayout.Label("");
		if (GUILayout.Button("reset"))
		{
			//generator.Reset();
		}




	}

}
