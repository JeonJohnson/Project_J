using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;

using NavMeshPlus.Components;


public class Room_Drunken : Room
{
	public void Awake()
	{
		enemyPos = new List<Vector3>();
	}


	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}



	
}
