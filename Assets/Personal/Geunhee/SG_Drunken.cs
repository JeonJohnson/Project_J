using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEditor;

public enum NewExplorerPos { center, prePos, Random };
public enum ExplorerDir
{
	Up,
	Right,
	Down,
	Left,
	None
}


public struct Explorer
{
	public Explorer(Vector2Int _index)
	{
		index = _index;
		dir = Vector2Int.zero;
	}

	public Explorer(int xIndex, int yIndex)
	{
		index = new Vector2Int(xIndex, yIndex);
		dir = Vector2Int.zero;
	}

	public Vector2Int index;
	public Vector2Int dir;
}


public class SG_Drunken : StageGenerator
{
	private float wallTileOffset_Origin;
	private float shadowTileOffset_Origin;
	[Space(7.5f)]
	[Range(0.1f, 0.75f)]
	public float wallTileOffset;
	[Range(0.1f, 0.75f)]
	public float shadowTileOffset;


	[Space(10f)]
	public RoomOption_Drunken option;
	//추후 한 스테이지 내 여러 방들의 옵션이 바뀌어야 하는 경우
	//수정 해 줄 수 있도록.


	protected override void Setup()
	{
		//#region AreaSetting
		//gridLength.x = Mathf.RoundToInt(option.areaSize.x / tileSize);
		//gridLength.y = Mathf.RoundToInt(option.areaSize.y / tileSize);
		////tileGrid = new tileGridState[gridLength.x, gridLength.y];

		//areaHalfSize = Funcs.ToV2(gridLength) * 0.5f;
		//tileHalfSize = tileSize * 0.5f;

		//centerPos = pivotPos + new Vector2(gridLength.x * 0.5f, gridLength.y * 0.5f);
		//centerIndex = new Vector2Int((int)(gridLength.x * 0.5f), (int)(gridLength.y * 0.5f));
		//originPos = new Vector2(pivotPos.x + tileHalfSize, pivotPos.y + tileHalfSize);

		//if (areaTr != null)
		//{
		//	areaTr.localScale = new Vector3(gridLength.x, gridLength.y);
		//	areaTr.localPosition = new Vector3(gridLength.x * 0.5f, gridLength.y * 0.5f);
		//}
		//#endregion

		//#region camSetting
		////cam ??= Camera.main;
		//if (cam != null)
		//{
		//	cam.transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
		//	cam.orthographicSize = gridLength.x <= gridLength.y ? gridLength.y * 0.5f : gridLength.x * 0.5f;
		//	cam.orthographicSize += 1;
		//}
		//#endregion

		//#region Explorer
		//option.explorers = new List<Explorer>();
		//for (int i = 0; i < option.startExplorerCount; i++)
		//{
		//	option.explorers.Add(new Explorer(centerIndex));
		//}
		//#endregion

		#region StageSetting
		if (stage != null)
		{
			ResetStage();
		}
		stage = new GameObject("Stage").AddComponent<Stage>();
		#endregion


		#region Explorer
		option.explorers = new List<Explorer>();
		Vector2Int center = new((int)(option.areaSize.x * 0.5f) , (int)(option.areaSize.y * 0.5f));
		for (int i = 0; i < option.startExplorerCount; i++)
		{
			option.explorers.Add(new Explorer(center));
		}
		#endregion
	}

	public override Room CreateOneRoom()
	{
		//throw new System.NotImplementedException();
		GameObject roomObj = Instantiate(TilemapRefer_Prefab);
		Room roomScript = roomObj.GetComponent<Room>();

		roomScript.Setup(option);

	



		return roomScript;
	}

	public override void CreateRooms()
	{
		throw new System.NotImplementedException();
	}

	public override void ResetStage()
	{
		throw new System.NotImplementedException();
	}


	private void CreateGround(Room room)
	{ 
		
	
	
	}

	private bool MoveExplorerOnce(Room room)
	{ //Move Once
		//1. Checking ground Count Ratio 
		if (option.areaFillRatio <= (float)recentTilesCount[(int)TilemapLayer.Ground] / (float)tileGrid.Length)
		{
			return false;
		}

		//2. fire explorer
		destoryCount = Random.Range(destroyCountRange.x, destroyCountRange.y + 1);
		List<Explorer> fired = new List<Explorer>();
		for (int i = 0; i < explorers.Count; ++i)
		{
			if (fired.Count >= destoryCount || explorers.Count - fired.Count <= ExplorerCountRange.x)
			{
				break;
			}

			if (UnityEngine.Random.value <= destroyPercent)
			{
				fired.Add(explorers[i]);
			}
		}
		foreach (var fire in fired)
		{
			explorers.Remove(fire);
		}
		fired = null;

		//3. respawn exploror
		respawnCount = Random.Range(respawnCountRange.x, respawnCountRange.y + 1);
		List<Explorer> respawnList = new List<Explorer>();
		for (int i = 0; i < explorers.Count; ++i)
		{
			if (respawnList.Count >= respawnCount || respawnList.Count + explorers.Count >= ExplorerCountRange.y)
			{
				break;
			}
			else
			{
				if (Random.value <= respawnPercent)
				{
					switch (ExplorerSpawnOption)
					{
						case NewExplorerPos.center:
							respawnList.Add(new Explorer(centerIndex));
							break;
						case NewExplorerPos.prePos:
							respawnList.Add(new Explorer(explorers[i].index));
							break;
						case NewExplorerPos.Random:
							{
								int rand = Random.Range(0, 2);
								if (rand == 0)
								{
									respawnList.Add(new Explorer(centerIndex));
								}
								else
								{
									respawnList.Add(new Explorer(explorers[i].index));
								}
							}
							break;
						default:
							break;
					}


				}
			}
		}
		explorers.AddRange(respawnList);
		respawnList = null;

		curExplorerCount = explorers.Count;

		//4. set new direction
		for (int i = 0; i < explorers.Count; ++i)
		{
			if (Random.value <= newDirPercent)
			{
				Explorer temp = explorers[i];
				temp.dir = GetRandomDir();
				explorers[i] = temp;
				//이렇게 스왑 해주는 이유
				//구조체로 이루어진 List등 자료구조에서
				//index로 접근하여 구조체를 Get하면
				//밸류카피로 가져와짐.
			}
		}

		//5. move And Create Tile
		for (int i = 0; i < explorers.Count; ++i)
		{
			Explorer temp = explorers[i];
			temp.index += temp.dir;

			temp.index.x = Mathf.Clamp(temp.index.x, 1, gridLength.x - 2);
			temp.index.y = Mathf.Clamp(temp.index.y, 1, gridLength.y - 2);

			CreateTile(temp.index, tileGridState.Ground);
			explorers[i] = temp;
		}

		return true;
	}

	private void SimulatingSetup(Room room)
	{
		#region AreaSetting
		if (areaTr != null)
		{
			areaTr.localScale = new Vector3(room.size.x, room.size.y);
			areaTr.localPosition = new Vector3(room.size.x * 0.5f, room.size.y * 0.5f);
		}
		#endregion

		#region camSetting
		//cam ??= Camera.main;
		if (cam != null)
		{
			cam.transform.position = new Vector3(room.centerPos.x, room.centerPos.y, -10f);
			cam.orthographicSize = room.size.x <= room.size.y ? room.size.y * 0.5f : room.size.x * 0.5f;
			cam.orthographicSize += 1;
		}
		#endregion

		

	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}


}
