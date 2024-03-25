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
using System;


public enum RoomType
{
	//해당 방 어떻게 할지 
	Normal,
	Shop,
	Boss,
	End
}
[System.Serializable]
public struct RoomSpec
{
	[ReadOnly]
	public Vector3 centerPos;//not CenterIndex's position
	[ReadOnly]
	public Vector2Int centerIndex;
	[ReadOnly]
	public Vector3 originIndexPos;
	[ReadOnly]
	public Vector2Int size;

	public RoomType roomType;
	[ReadOnly]
	public int roomIndex; //At stage's room List
}

public class Room : MonoBehaviour
{
	[SerializeField]
	RoomSpec spec;
	public RoomSpec  Spec
	{
		get
		{
			return spec;
		}
	}

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
	public TilemapFlag GetTileState(int x, int y)
	{
		return tileStates[x, y];
	}

	[Space(10f)]
	public SerializedDictionary<TilemapFlag, Tilemap> tilemaps;

	[Space(12.5f)]
	[Header("Pooling")]
	[ReadOnly]
	public List<Vector3> enemyPos;
	[ReadOnly]
	public List<Enemy> allEnemies;
	//[ReadOnly]
	//public List<Enemy> aliveEnemies;

	[Space(10f)]
	public Transform poolingTr;
	[ReadOnly]
	public List<GameObject> poolingObjs;
	//Bullet, Coin, Enemy's Weapon, DeadBody, etc...
	
	[Space(10f)]
	[SerializeField]
	protected NavMeshSurface navSurface;


	public void Setup(RoomOption option)
	{
		#region AreaSetting
		spec = new RoomSpec();

		spec.size.x = Mathf.RoundToInt(option.areaSize.x / RoomOption.tileSize);
		spec.size.y = Mathf.RoundToInt(option.areaSize.y / RoomOption.tileSize);

		tileStates = new TilemapFlag[spec.size.x, spec.size.y];

		spec.centerPos = option.pivotPos + new Vector3(spec.size.x * 0.5f, spec.size.y * 0.5f);
		spec.centerIndex = new Vector2Int((int)(spec.size.x * 0.5f), (int)(spec.size.y * 0.5f));
		spec.originIndexPos = new Vector2(option.pivotPos.x + RoomOption.tileSize * 0.5f, option.pivotPos.y + RoomOption.tileSize * 0.5f); ;
		#endregion
	}

	public void BakeNavMesh()
	{
		navSurface.BuildNavMeshAsync();
	}

	public void AddPoolingObj(GameObject obj)
	{
		obj.transform.parent = poolingTr;
		poolingObjs.Add(obj);
	}

	public virtual void Cleanup()
	{
		foreach (var item in poolingObjs)
		{
			IPoolable poolable = item.GetComponent<IPoolable>();
			poolable?.PoolableReset();

			PoolingManager.Instance?.ReturnObj(item);
		}
	}

	public void Disappear()
	{ 
	
	}

	public void Appear()
	{ 
	
	}


	public Vector2 GetPos(Vector2Int index)
	{
		float tileSize = 1f;

		if (index.x < 0 | index.y < 0 | index.x >= tileStates.GetLength(0) | index.y >= tileStates.GetLength(1))
		{
			return new Vector2(float.MinValue, float.MinValue);
		}

		Vector2 pos = new Vector2(index.x * tileSize + tileSize * 0.5f, index.y * tileSize + tileSize * 0.5f);

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





	private void Awake()
	{
		navSurface.hideEditorLogs = false;
	}


	



}
