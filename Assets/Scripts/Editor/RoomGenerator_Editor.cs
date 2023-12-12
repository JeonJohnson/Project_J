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
		//GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);

		//1. 초기화 / 구역 나눠주기
		//기본 바닥 만들어 주기
		if (GUILayout.Button("Create Whole Area"))
		{
			generator.CreateWholeArea();
		}
		GUILayout.Label("");
		if (GUILayout.Button("Split Area Once"))
		{
			generator.SplitArea_Once();
		}
		if (GUILayout.Button("Split Area To The End"))
		{
			generator.SplitArea_End();
		}
		if (GUILayout.Button("Reset Area"))
		{
			generator.ResetArea();
		}

		//2. 여러 규칙에 알맞은 방 찾아서 배치해주기
		GUILayout.Label("");
		if (GUILayout.Button("Assing Rooms"))
		{
			generator.AssignRooms();
		}
		if (GUILayout.Button("Reset Rooms"))
		{
			generator.ResetRooms();
		}

		//3. 중점 조절
		GUILayout.Label("");
		if (GUILayout.Button("Calibrate Position"))
		{
			generator.CalibratePosition();
		}

		//4.복도 생성하기
		GUILayout.Label("");
		if (GUILayout.Button("ConnectingRoom"))
		{
			generator.ConnectingRoom();	
		}
		if (GUILayout.Button("Rest Corridor"))
		{
			generator.ResetCorridors();
		}

		GUILayout.Label("");
		if (GUILayout.Button("Generating Dungeon"))
		{
			generator.GeneratingRandomDungeon();
		}
		if (GUILayout.Button("Reset Dungeon"))
		{
			generator.AllReset();
		}

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
