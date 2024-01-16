using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;

using NavMeshPlus.Components;


public class Room_Drunken : MonoBehaviour
{
	public Vector3 centerPos;

	public SerializedDictionary<TilemapLayer, Tilemap> tilemaps;

	[SerializeField]
	private NavMeshSurface navSurface;


	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}
}
