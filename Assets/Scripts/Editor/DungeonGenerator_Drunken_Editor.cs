﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(DungeonGenerator_Drunken))]
public class DungeonGenerator_Drunken_Editor : Editor
{
	//bool isInit = false;
	public override void OnInspectorGUI()
	{
		DungeonGenerator_Drunken generator = (DungeonGenerator_Drunken)target;
		
		DrawDefaultInspector();


		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		//GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);

		//1. 초기화 / 구역 나눠주기
		//기본 바닥 만들어 주기

		if (GUILayout.Button("Create Room"))
		{
			generator.CreateRoom();
		}

		//if (GUILayout.Button("Create Ground"))
		//{
		//	//if (Application.isPlaying)
		//	//{

		//	//}
		//	//else
		//	//{
		//		generator.CreateGround();
		//	//}
		//}
		////if (GUILayout.Button("Create Ground Immediately"))
		////{
		////	generator.CreateGround_Immediately();
		////}
		//GUILayout.Label("");
		//if (GUILayout.Button("Test Wall"))
		//{
		//	for(int i  = 0; i < generator.roomCount; ++i)
		//	{ 
		//		generator.CreateWall();
		//		generator.CreateCliff();
		//	}
		//}

		GUILayout.Label("");
		if (GUILayout.Button("Add Room"))
		{
			generator.Setup();
			generator.CreateRoom();
			//generator.Reset();
		}

		GUILayout.Label("");
		if (GUILayout.Button("reset"))
		{
			generator.Reset();
		}




	}

}
