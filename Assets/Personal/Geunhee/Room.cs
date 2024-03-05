using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

using AYellowpaper.SerializedCollections;

using NavMeshPlus.Components;

public enum RoomType
{ 
	Normal,
	Shop,
	Boss,
	End
}

public abstract class Room : MonoBehaviour
{
	public RoomType roomType;
	
	[Space(10f)]
	public Vector3 centerPos;//not CenterIndex's position
	public Vector2Int size;

	[Space(10f)]
	public tileGridState[,] tileStates;

	[Space(10f)]
	public SerializedDictionary<TilemapLayer, Tilemap> tilemaps;

	[Space(10f)]
	public List<Vector3> enemyPos;
	[SerializeField]
	protected NavMeshSurface navSurface;

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
		Vector2 index = (pos - new Vector2(0.5f, 0.5f)) * 1;

		if (index.x < 0f | index.y < 0f | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2Int(int.MinValue, int.MinValue);
		}

		return new Vector2Int((int)index.x, (int)index.y);
	}


	public void Cleanup()
	{ 
	
	}
}
