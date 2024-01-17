using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using MoreMountains.Feedbacks;

[CustomEditor(typeof(DungeonGenerator_Drunken))]
public class DungeonGenerator_Drunken_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		DungeonGenerator_Drunken generator = (DungeonGenerator_Drunken)target;
		
		DrawDefaultInspector();


		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		//GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);


		if (GUILayout.Button("Create One Room"))
		{
			generator.CreateRoom();
		}

		GUILayout.Label("");
		if (GUILayout.Button("Create Rooms and Go to Game Scene"))
		{
			
			for (int i = 0; i < generator.createRoomCount; ++i)
			{
				generator.CreateRoom();
			}

			if (Application.isPlaying)
			{ generator.GotoGameScene_ForTest(); }
		}

		

		GUILayout.Label("");
		if (GUILayout.Button("reset"))
		{
			generator.Reset();
		}




	}

}
