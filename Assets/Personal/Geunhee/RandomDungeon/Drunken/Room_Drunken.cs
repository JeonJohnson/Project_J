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


	public List<Vector3> enemyPos;
	

	public void Awake()
	{
		enemyPos = new	List<Vector3>();
	}


	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}



	public Vector2 GetPos(Vector2Int index)
	{
		if (index.x < 0 | index.y < 0 | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2(float.MinValue, float.MinValue);
		}

		Vector2 pos = new Vector2(index.x * 1 + 0.5f, index.y * 1 + 0.5f);

		return pos;
	}

	public Vector2Int GetIndex(Vector2 pos)
	{
		Vector2 index = (pos - new Vector2(0.5f,0.5f)) * 1;

		if (index.x < 0f | index.y < 0f | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2Int(int.MinValue, int.MinValue);
		}

		return new Vector2Int((int)index.x, (int)index.y);
	}
}
