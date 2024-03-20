using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

using AYellowpaper.SerializedCollections;

using NavMeshPlus.Components;
using UnityEngine.UIElements;
using Unity.VisualScripting;


public enum RoomType
{
	//해당 방 어떻게 할지 
	Normal,
	Shop,
	Boss,
	End
}

public  class Room : MonoBehaviour
{
	
	[Space(10f)]
	[Header("Pos&Index")]
	[ReadOnly]
	public Vector3 centerPos;//not CenterIndex's position
	[ReadOnly]
	public Vector2Int centerIndex;
	[ReadOnly]
	public Vector3 originIndexPos;

	[Space(10f)]
	public RoomType roomType;
	[ReadOnly]
	public int roomIndex; //At stage's room List
	[ReadOnly]
	public Vector2Int size;
	

	[Space(10f)]
	public TilemapFlag[,] tileStates;
	public int GetTileCount(TilemapFlag tileType)
	{
		int count = 0;

		for (int x = 0; x < tileStates.GetLength(0); ++x)
		{
			for (int y = 0; y < tileStates.GetLength(1); ++y)
			{
				if ((tileStates[x, y] & tileType) != 0)
				{
					++count;
				}
			}
		}

		return count;
	}


	[Space(10f)]
	public SerializedDictionary<TilemapFlag, Tilemap> tilemaps;

	[Space(10f)]
	[ReadOnly]
	public List<Vector3> enemyPos;
	[ReadOnly]
	public List<Enemy> allEnemies;
	[ReadOnly]
	public List<Enemy> aliveEnemies;

	[Space(10f)]
	[ReadOnly]
	public List<GameObject> poolingObj;
	//[ReadOnly]
	//public List<GameObject> bullets;
	//[ReadOnly]
	//public List<GameObject> items;

	[Space(10f)]
	[SerializeField]
	protected NavMeshSurface navSurface;


	public void Setup(RoomOption option)
	{
		#region AreaSetting
		size.x = Mathf.RoundToInt(option.areaSize.x / RoomOption.tileSize);
		size.y = Mathf.RoundToInt(option.areaSize.y / RoomOption.tileSize);

		tileStates = new TilemapFlag[size.x, size.y];

		//areaHalfSize = Funcs.ToV2(gridLength) * 0.5f;
		//tileHalfSize = tileSize * 0.5f;

		centerPos = option.pivotPos + new Vector3(size.x * 0.5f, size.y * 0.5f);
		centerIndex = new Vector2Int((int)(size.x * 0.5f), (int)(size.y * 0.5f));
		originIndexPos = new Vector2(option.pivotPos.x + RoomOption.tileSize * 0.5f, option.pivotPos.y + RoomOption.tileSize * 0.5f); ;
		#endregion
	}


	private void Awake()
	{
		//tilemaps = new SerializedDictionary<TilemapFlag, Tilemap>();
	}

	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}


	public virtual void Cleanup()
	{

	}

	public void DestoryRoom()
	{

	}



	

	public Vector2 GetPos(Vector2Int index)
	{
		float tileSize = 1f;

		if (index.x < 0 | index.y < 0 | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2(float.MinValue, float.MinValue);
		}

		Vector2 pos = new Vector2(index.x * tileSize + tileSize*0.5f, index.y * tileSize + tileSize*0.5f);

		return pos;
	}

	public Vector2Int GetIndex(Vector2 pos)
	{
		float tileSize = 1f;

		Vector2 index = (pos - new Vector2(tileSize * 0.5f, tileSize * 0.5f)) * tileSize;

		if (index.x < 0f | index.y < 0f | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2Int(int.MinValue, int.MinValue);
		}

		return new Vector2Int((int)index.x, (int)index.y);
	}



}
