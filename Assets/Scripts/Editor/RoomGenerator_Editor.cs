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

		if (GUILayout.Button("Divied"))
		{
			generator.Divide();
		}
	}

}
