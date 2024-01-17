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
	public Vector3 centerPos;//not CenterIndex's position
	public Vector2Int size;

	public SerializedDictionary<TilemapLayer, Tilemap> tilemaps;

	[SerializeField]
	private NavMeshSurface navSurface;

	
	public tileGridState[,] tileStates;

	public void Awake()
	{
		
	}


	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}
}
