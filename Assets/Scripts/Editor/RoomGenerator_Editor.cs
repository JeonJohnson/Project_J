using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGenerator_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		RoomGenerator generator = (RoomGenerator)target;
		
		DrawDefaultInspector();


		GUILayout.Label("\nRoom Generator Buttons", EditorStyles.boldLabel);
		GUILayout.Label("dont divide upper 12~14 times.\nur Computer hates u...", EditorStyles.miniLabel);

		if (GUILayout.Button("Divied Once"))
		{
			generator.Divied();
		}


		if (GUILayout.Button("Divieding Rooms"))
		{
			generator.Divieding();
		}


		if (GUILayout.Button("Shrink Rooms"))
		{
			generator.ShrinkRooms();
		}

		if (GUILayout.Button("Connecting Room Once"))
		{
			generator.NewConnectSiblingRoom(generator.curCorridorDepth);
		}


		if (GUILayout.Button("Connecting Rooms"))
		{
			generator.ConnectingRooms();
		}

		GUILayout.Label("");
		if (GUILayout.Button("Reset"))
		{
			generator.ResetRooms();
		}


		GUILayout.Label("\n");

		if (GUILayout.Button("Generate Room"))
		{
			generator.GeneratingRooms();
		}


		
		
	}

}
