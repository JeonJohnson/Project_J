using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(SG_Drunken))]
public class SG_Drunken_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		SG_Drunken generator = (SG_Drunken)target;

	

		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		//GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);


		if (GUILayout.Button("Create Stage"))
		{
			generator.CreateStage();
		}

		//GUILayout.Label("");
		//if (GUILayout.Button("Create Rooms and Go to Game Scene"))
		//{

		//	for (int i = 0; i < generator.createRoomCount; ++i)
		//	{
		//		generator.CreateRoom();
		//	}

		//	if (Application.isPlaying)
		//	{ generator.GotoGameScene_ForTest(); }
		//}



		GUILayout.Label("");
		if (GUILayout.Button("reset"))
		{
			generator.ResetStage();
		}

		DrawDefaultInspector();



	}

}
