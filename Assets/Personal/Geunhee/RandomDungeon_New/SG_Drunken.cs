using System;
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
using static UnityEditor.PlayerSettings;

using Random = UnityEngine.Random;

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

public class ExploreCompany
{
	public ExploreCompany(int explorerCount, Vector2Int centerIndex)
	{
		explorers = new List<Explorer>();

		for (int i = 0; i < explorerCount; ++i)
		{
			explorers.Add(new Explorer(centerIndex));
		}

		hireCount = 0;
		fireCount = 0;
	}


	public List<Explorer> explorers;
	public int hireCount;
	public int fireCount;
}


public class SG_Drunken : StageGenerator
{

	//private int[] tempTileTypeCount = new int[(int)TilemapLayer.End];
	
	private int[] tempTileTypeCount = new int[Enum.GetValues(typeof(TilemapFlag)).Length];
	//이렇게 되면 접근할떄 다시 밑2 로그연산 박아줘야하기에 Enum으로 배열 쓰는게 조금 빡셈
	//ex) Ground찾을때
	//기존 enum => tempTileTypeCount[(int)Ground]
	//BitFlag => tempTileTypeCount[(int)Math.Log(Ground,2)]
	//그러므로 Flag로 쓰는 enum이면 꼬옥  변수명에 Flag박아주기


	private float roofTileOffset_Origin = 0.3f;
	private float shadowTileOffset_Origin = 0.2f;
	[Space(7.5f)]
	[Range(0.1f, 0.75f)]
	public float roofTileOffset;
	[Range(0.1f, 0.75f)]
	public float shadowTileOffset;


	[Space(10f)]
	//public List<Explorer> explorers;
	//public int hireCount;
	//public int fireCount;
	public ExploreCompany exploreCompany;
	public RoomOption_Drunken option;

	//추후 한 스테이지 내 여러 방들의 옵션이 바뀌어야 하는 경우
	//수정 해 줄 수 있도록.

	//////////////////////////////////////////////////////////////////////////////

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
		//explorers = new List<Explorer>();
		//for (int i = 0; i < option.startExplorerCount; i++)
		//{
		//	explorers.Add(new Explorer(centerIndex));
		//}
		//#endregion

		#region StageSetting
		if (stage == null)
		{
			stage = new GameObject("Stage").AddComponent<Stage>();
		}
		//ResetStage();
		#endregion


		#region Explorer
		Vector2Int centerIndex = new((int)(option.areaSize.x * 0.5f) , (int)(option.areaSize.y * 0.5f));
		//explorers = new List<Explorer>();
		//for (int i = 0; i < option.startExplorerCount; i++)
		//{
		//	explorers.Add(new Explorer(centerIndex));
		//}
		exploreCompany = new ExploreCompany(option.startExplorerCount, centerIndex);
		#endregion

		tempTileTypeCount = new int[Enum.GetValues(typeof(TilemapFlag)).Length];
	}

	public override Room CreateOneRoom()
	{
		Room curRoom = CreateRoomObject();
		CreateGround(curRoom);
		CreateWall(curRoom);
		ApplyTilePosOffset(curRoom);
		SetEnemySpawnPos(curRoom);
		curRoom.BakeNavMesh();

		return curRoom;
	}


	public override void CreateStage()
	{

		for (int i = 0; i < roomCount; ++i)
		{
			Setup();
			Room curRoom = CreateOneRoom();
			//curRoom = CreateOneRoom();
#if UNITY_EDITOR
			SimulatingSetup(curRoom);
#endif
			//CreateGround(curRoom);
			//CreateWall(curRoom);
			//curRoom.BakeNavMesh();

			curRoom.gameObject.name += $"({stage.rooms.Count})";
			curRoom.transform.SetParent(stage.transform);
			stage.rooms.Add(curRoom);

			//if (i < roomCount-1)
			//{ curRoom.gameObject.SetActive(false); }
			curRoom.gameObject.SetActive(false);
		}

		SetShopRoom(stage);
		SetBossRoom(stage);

		
	}

	public override void ResetStage()
	{
		//Stage파괴하기
		stage = null;
	}


	//////////////////////////////////////////////////////////////////////////////

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

	private Room CreateRoomObject()
	{
		GameObject roomObj = Instantiate(TilemapRefer_Prefab);
		Room roomScript = roomObj.GetComponent<Room>();

		roomScript.Setup(option);
		

		return roomScript;
	}

	private void CreateGround(Room room)
	{
		option.curTryCount = 0;
		while (option.maxTryCount > option.curTryCount)
		{
			if (MoveExplorerOnce(room))
			{
				++option.curTryCount;
			}
			else
			{
				break;
			}
		}
	}

	private bool MoveExplorerOnce(Room room)
	{ //Move Once
	  //1. Checking ground Count Ratio 
		
		int mapLen = option.areaSize.x + option.areaSize.y;
		if (option.areaFillRatio <= (float)tempTileTypeCount[Funcs.FlagToEnum((int)TilemapFlag.Ground)] / (float)mapLen)
		{
			return false;
		}

		ExploreCompany ec = exploreCompany;
		//2. fire explorer
		ec.fireCount = Random.Range(option.destroyCountRange.x, option.destroyCountRange.y + 1);
		List<Explorer> fired = new List<Explorer>();
		for (int i = 0; i < ec.explorers.Count; ++i)
		{
			if (fired.Count >= ec.fireCount || ec.explorers.Count - fired.Count <= option.ExplorerCountRange.x)
			{
				break;
			}

			if (UnityEngine.Random.value <= option.destroyPercent)
			{
				fired.Add(ec.explorers[i]);
			}
		}
		foreach (var fire in fired)
		{
			ec.explorers.Remove(fire);
		}
		fired = null;

		//3. respawn exploror
		ec.hireCount = Random.Range(option.respawnCountRange.x, option.respawnCountRange.y + 1);
		List<Explorer> respawnList = new List<Explorer>();
		for (int i = 0; i < ec.explorers.Count; ++i)
		{
			if (respawnList.Count >= ec.hireCount || respawnList.Count + ec.explorers.Count >= option.ExplorerCountRange.y)
			{
				break;
			}
			else
			{
				if (Random.value <= option.respawnPercent)
				{
					switch (option.ExplorerSpawnOption)
					{
						case NewExplorerPos.center:
							respawnList.Add(new Explorer(room.centerIndex));
							break;
						case NewExplorerPos.prePos:
							respawnList.Add(new Explorer(ec.explorers[i].index));
							break;
						case NewExplorerPos.Random:
							{
								int rand = Random.Range(0, 2);
								if (rand == 0)
								{
									respawnList.Add(new Explorer(room.centerIndex));
								}
								else
								{
									respawnList.Add(new Explorer(ec.explorers[i].index));
								}
							}
							break;
						default:
							break;
					}


				}
			}
		}
		ec.explorers.AddRange(respawnList);
		respawnList = null;

		int curExplorerCount = ec.explorers.Count;

		//4. set new direction
		for (int i = 0; i < ec.explorers.Count; ++i)
		{
			if (UnityEngine.Random.value <= option.newDirPercent)
			{
				Explorer temp = ec.explorers[i];
				temp.dir = Funcs.ToV2I(GetRandomDir());
				ec.explorers[i] = temp;
				//이렇게 스왑 해주는 이유
				//구조체로 이루어진 List등 자료구조에서
				//index로 접근하여 구조체를 Get하면
				//밸류카피로 가져와짐.
			}
		}

		//5. move And Create Ground Tile
		for (int i = 0; i < ec.explorers.Count; ++i)
		{
			Explorer temp = ec.explorers[i];
			temp.index += temp.dir;

			temp.index.x = Mathf.Clamp(temp.index.x, 1, option.areaSize.x - 2);
			temp.index.y = Mathf.Clamp(temp.index.y, 1, option.areaSize.y - 2);

			CreateTile(temp.index, TilemapFlag.Ground, room);
			ec.explorers[i] = temp;
		}

		return true;
	}

	private void CreateWall(Room room)
	{
		//List<Vector2Int> createIndexes = new List<Vector2Int>();

		for (int x = 0; x < option.areaSize.x; ++x)
		{
			for (int y = 0; y < option.areaSize.y; ++y)
			{
				Vector2Int index = new Vector2Int(x, y);

				if (room.tileStates[x, y] == TilemapFlag.None)
				{
					//Vector2 pos = room.GetPos(index);
					CreateTile(index, TilemapFlag.Wall,room);
					//SetTile(index, TilemapLayer.Cliff);
					CreateTile(index, TilemapFlag.Roof, room);
				}
			}
		}

		var shadowScript = room.tilemaps[TilemapFlag.Shadow].gameObject.GetComponent<MMTilemapShadow>();
		if (shadowScript)
		{
			shadowScript.UpdateShadows();
		}
	}

	private void ApplyTilePosOffset(Room room)
	{
		var pos = room.tilemaps[TilemapFlag.Roof].transform.position;
		pos.y += roofTileOffset;
		room.tilemaps[TilemapFlag.Roof].transform.position = pos;

		pos = room.tilemaps[TilemapFlag.Shadow].transform.position;
		pos.y -= shadowTileOffset;
		room.tilemaps[TilemapFlag.Shadow].transform.position = pos;
	}

	private void SetEnemySpawnPos(Room room)
	{
		int curEnemyCount = 0;

		List<Vector2Int> groundTileIndex = new List<Vector2Int>();

		for (int y = 0; y < option.areaSize.y; ++y)
		{
			for (int x = 0; x < option.areaSize.x; ++x)
			{
				if (room.tileStates[x, y] == TilemapFlag.Ground)
				{//이건 정말 '땅만' 있는경우만
					groundTileIndex.Add(new(x, y));
				}
			}
		}

		List<int> nums = new List<int>();

		int tryCount = 0;
		while (curEnemyCount < option.enemyCount && tryCount < option.maxTryCount)
		{
			int curIndex = Funcs.GetDontOverlapRandom(0, groundTileIndex.Count, ref nums);
			room.enemyPos.Add(room.GetPos(groundTileIndex[curIndex]));
			++curEnemyCount;
			++tryCount;
		}
	}

	private void SetShopRoom(Stage _stage)
	{
		if (ShopPrefab == null)
		{
			return;
		}
		
		int curShopCount = 0;
		while (curShopCount < shopCount)
		{
			GameObject shopObj = Instantiate(ShopPrefab, _stage.transform);
			Room shopSc = shopObj.GetComponent<Room>(); // 추후에 룸 상속받는 Shop새로만들기
			int rand = UnityEngine.Random.Range(0, _stage.rooms.Count);
			
			//앞뒤로 상점 없는거 체크 요망
			_stage.rooms.Insert(rand + 1, shopSc);
			shopObj.transform.SetSiblingIndex(rand + 1);
			shopObj.SetActive(false);
			
			++curShopCount;
		}
		
	}

	private void SetBossRoom(Stage _stage)
	{
		if (BossRoomPrefab == null)
		{
			return;
		}

		GameObject bossObj = Instantiate(BossRoomPrefab,_stage.transform);
		Room bossSc = bossObj.GetComponent<Room>();
		_stage.rooms.Add(bossSc);
		bossObj.transform.SetAsLastSibling();
		bossObj.SetActive(true);
	}

	//////////////////////////////////////////////////////////////////////////////

	private void CreateTile(Vector2Int index, TilemapFlag flag, Room room)
	{
		//var index = room.GetIndex(pos);

		//if (room.tileStates[index.x, index.y] != layer)
		//{

		if ((room.tileStates[index.x, index.y] & flag) != 0)
		{
			return;
		}

			room.tileStates[index.x, index.y] |= flag;

			//int layer = 0;
			TileBase tile = null;
			Vector3 newPos = room.GetPos(index);
			//switch (state)
			//{

			//	case tileGridState.Ground:
			//		{
			//			layer = (int)TilemapLayer.Ground;
			//		}
			//		break;
			//	case tileGridState.Wall:
			//		{
			//			layer = (int)TilemapLayer.Wall;
			//		}
			//		break;
			//}

			//타일 선택하는거에서 추후에 추가적인 랜덤성 부여
			tile = TileResource[flag][0];
			//타일 선택하는거에서 추후에 추가적인 랜덤성 부여 

			room.tilemaps[flag].SetTile(Funcs.ToV3I(newPos), tile);
			++tempTileTypeCount[Funcs.FlagToEnum((int)flag)];
		//}

	}

	private Vector2 GetRandomDir()
	{
		return GetDir(Random.Range((int)ExplorerDir.Up, (int)ExplorerDir.None + 1));
	}

	private Vector2 GetDir(int num)
	{
		Vector2 val = Vector2Int.zero;

		switch ((ExplorerDir)num)
		{
			case ExplorerDir.Up:
				val = Vector2Int.up;
				break;
			case ExplorerDir.Right:
				val = Vector2Int.right;
				break;
			case ExplorerDir.Down:
				val = Vector2Int.down;
				break;
			case ExplorerDir.Left:
				val = Vector2Int.left;
				break;
			default:
				val = Vector2Int.zero;
				break;
		}
		val.x *= RoomOption.tileSize;
		return val;
	}

	//////////////////////////////////////////////////////////////////////////////

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
