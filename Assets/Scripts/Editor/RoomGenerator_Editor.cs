using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGenerator_Editor : Editor
{
	//bool isInit = false;
	public override void OnInspectorGUI()
	{
		RoomGenerator generator = (RoomGenerator)target;
		
		DrawDefaultInspector();


		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);

		//1. 초기화
		//기본 바닥 만들어 주기
		if (GUILayout.Button("Create Whole Area"))
		{
			//if (!isInit)
			//{
			//	generator.Initialize();
			//}
			generator.CreateWholeArea();
		}

		//2. 구역 나눠주기
		GUILayout.Label("");
		if (GUILayout.Button("Split Area Once"))
		{
			generator.SplitArea_Once();
		}
		if (GUILayout.Button("Split Area To The End"))
		{
			generator.SplitArea_End();
		}
		//if (GUILayout.Button("SetActive"))
		//{
		//	generator.SplitArea_End();
		//}

		//3. 해당 구역 안에서 방 생성


		//4. 복도 생성


		//if (GUILayout.Button("Divied Once"))
		//{
		//	generator.Divied();
		//}


		//if (GUILayout.Button("Divieding Rooms"))
		//{
		//	generator.Divieding();
		//}


		//if (GUILayout.Button("Shrink Rooms"))
		//{
		//	generator.ShrinkRooms();
		//}

		//if (GUILayout.Button("Connecting Room Once"))
		//{
		//	generator.NewConnectSiblingRoom(generator.curCorridorDepth);
		//}


		//if (GUILayout.Button("Connecting Rooms"))
		//{
		//	generator.ConnectingRooms();
		//}

		//GUILayout.Label("");
		//if (GUILayout.Button("Reset"))
		//{
		//	generator.ResetRooms();
		//}


		//GUILayout.Label("\n");

		//if (GUILayout.Button("Generate Room"))
		//{
		//	generator.GeneratingRooms();
		//}




	}

}
