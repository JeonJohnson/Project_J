using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using AYellowpaper.SerializedCollections;

using Structs;
using JeonJohnson;
using Unity.VisualScripting;
using System;

using Random = UnityEngine.Random;



public enum eDirection
{ 
	Vertical,
	Horizon,
	End
}

//1. BSP (이진 공간 분할법) 알고리즘을 이용하여 특정 조건까지 나누기.
//BSP -> 특정 공간을 재귀 함수로 나눌때 트리 구조를 만들어 재귀를 돌림.
//2. 나눠진 공간내에서 여백을 주고 실질적인 방 사이즈를 정해줌.
//3. 만들어진 방들에서 시작 지점 정해주기
//4. 들로네 삼각 분할 알고리즘을 통하여 인접한 방들 연결하기
//5. 최소 스패닝 트리를 구상하여 통로 정리
//6. 직각,수직의 선으로 정리하기

//=> 이런 방식의 문제점. 최소, 최대 방크기가 기준이 되기 애매함
public class RoomGenerator : MonoBehaviour
{



	//for Test
	[Header("Setting Vals")]
	public Vector2 wholeAreaSize; //나눠지지 않은, 온전한 던전의 초기 넓이 값.
	[Header("Prefabs")]
	[SerializedDictionary("Room Prefab", "Map Size")]
	public SerializedDictionary<GameObject, Vector2Int> RoomPrefabs;
	public GameObject AreaPrefab;
	public GameObject CorridorPrefab;

	[Header("Boxes")]
	public Transform areaBox;
	public Transform roomBox;
	public Transform corridorBox;

	public int splitTimes;//분할 하고 싶은 횟수
	public int splitTryCount;
	[Range(0.1f, 0.5f)]
	public float minSplitRatio;
	[Range(0.5f, 0.9f)]
	public float maxSplitRatio;
	public float centerPosOffset;
	//public float corridorWidth;



	[Space(10f)]
	[Header("Display Vals")]
	[ReadOnly]
	public Vector2Int minRoomSize;
	[ReadOnly]
	public Vector2Int maxRoomSize;
	[ReadOnly]
	public int curSplitCount = 0;
	[ReadOnly]
	public int curRoomCount = 0;
	[ReadOnly]
	public int curCorridorDepth = 0;

	[HideInInspector]
	private List<Room> roomList = null;
	private JeonJohnson.Tree<Area> areaTree = null;
	private List<Corridor> corridors = null;



	public void Initialize()
	{//0. 초기화

		curSplitCount = 0;
		curRoomCount = 0;
		curCorridorDepth = 0;

		roomList ??= new List<Room>();
		areaTree ??= new Tree<Area>();
		corridors ??= new List<Corridor>();

		//밑에 배열 안쓰고 일일이 박스 만든건
		//타 파트 분들도 직관적으로 알 수 있도록 하기 위해서

		//유니티에서는 ?? 연산자 웬만하면 쓰지말라함.
		//null 비교 연산 자체도 오버로딩 되있는거라서 그런듯
		//걍 한줄로 처리하자

		//areaBox = areaBox ?? new GameObject("Area Box").transform;
		//roomBox = roomBox ?? new GameObject("Room Box").transform;
		//corridorBox ??= new GameObject("Corridor Box").transform;

		if (!areaBox) areaBox = new GameObject("Area Box").transform;
		if (!roomBox) roomBox = new GameObject("Room Box").transform;
		if (!corridorBox) corridorBox = new GameObject("Corridor Box").transform;

		Vector2Int minSize = new Vector2Int(int.MaxValue, int.MaxValue);
		Vector2Int maxSize = new Vector2Int(int.MinValue, int.MinValue);
		foreach (var prefab in RoomPrefabs)
		{
			//int curPrefabSize = 0;
			if (prefab.Value == Vector2Int.zero)
			{
				Vector2 tempSize = prefab.Key.transform.localScale;
				RoomPrefabs[prefab.Key] = new Vector2Int((int)tempSize.x, (int)tempSize.y);
			}

			if (prefab.Value.x < minSize.x)
			{
				minSize.x = prefab.Value.x;
			}

			if (prefab.Value.x > maxSize.x)
			{
				maxSize.x = prefab.Value.x;
			}


			if (prefab.Value.y < minSize.y)
			{
				minSize.y = prefab.Value.y;
			}

			if (prefab.Value.y > maxSize.y)
			{
				maxSize.y = prefab.Value.y;
			}
		}

		minRoomSize = minSize;
		maxRoomSize = maxSize;
	}

	#region 1.Area Setting
	public void CreateWholeArea()
	{//1. 큰 공간 하나 만들기
		Area area = CreateArea(Vector2.zero, wholeAreaSize, areaTree.Count);

		areaTree.SetRootNode(new TreeNode<Area>(area, 0, 0));
	}

	public void SplitArea_End()
	{
		int tryNumber = splitTimes;

		if (curSplitCount == splitTimes)
		{
			//return false;
		}
		else if (curSplitCount > splitTimes)
		{
			//ResetAreas();
		}
		else if (curSplitCount > splitTimes)
		{
			tryNumber -= curSplitCount;
		}

		for (int i = 0; i < splitTimes; ++i)
		{
			SplitArea_Once();
		}
	}

	public void SplitArea_Once()
	{
		if (areaTree == null || areaTree.Count == 0)
		{
			Debug.Log("초기화가 되지 않았습니다. 다시 실행해 주세요");
		}

		var nodes = areaTree.GetLeafNodes();
		int passCount = 0;
		foreach (var node in nodes)
		{
			var newArea = Spliting(node.Value,splitTryCount);

			if (newArea != null)
			{
				areaTree.AddNode(node, newArea[0], newArea[1]);
			}
			else { passCount += 1; }
		}

		if (nodes.Count != passCount)
		{
			++curSplitCount;
		}
		
	}

	private List<Area> Spliting(Area area, int tryCount = 100)
	{
		if (area.rect.width <= minRoomSize.x || area.rect.height <= minRoomSize.y)
		{
			return null;
		}

		int curTry = 1;
		List<Area> splitedArea = new List<Area>();
		Rect[] newRect = new Rect[2] { Rect.zero, Rect.zero };

		//0 = split by horizontal (가로로 나누기)
		//1 = split by vertical (세로로 나누기)
		eDirection splitDir = area.rect.width / area.rect.height >= 1f ? eDirection.Vertical : eDirection.Horizon;

		do
		{//잘라보고 사이즈 최소 사이즈 충족이 되면 실제로 방 만들기
			float splitRatio = UnityEngine.Random.Range(minSplitRatio, maxSplitRatio);

			switch (splitDir)
			{
				case eDirection.Vertical:
					{
						//세로로 나누는 경우 왼쪽 -> 오른쪽 순서

						Rect left = new Rect(area.rect);
						left.width *= splitRatio;

						float rightWidth = area.rect.width * (1f - splitRatio);
						Rect right = new Rect(area.rect.xMin + area.rect.width * splitRatio, area.rect.yMin, rightWidth, area.rect.height);

						newRect[0] = left;
						newRect[1] = right;
					}
					break;
				case eDirection.Horizon:
					{
						//가로로 나누는 경우 위쪽 -> 아래쪽 순서
						float topHeight = area.rect.height * (1f - splitRatio);
						Rect top = new Rect(area.rect.xMin, area.rect.yMax - topHeight, area.rect.width, topHeight);

						Rect bot = new Rect(area.rect);
						bot.height *= splitRatio;

						newRect[0] = top;
						newRect[1] = bot;
					}
					break;
				default:
					break;
			}

			if (newRect[0].width > minRoomSize.x + centerPosOffset
				&& newRect[1].width > minRoomSize.x + centerPosOffset
				&& newRect[0].height > minRoomSize.y + centerPosOffset
				&& newRect[1].height > minRoomSize.y + centerPosOffset)
			{
				break;
			}
			++curTry;
		}
		while (curTry < tryCount);

		if (curTry >= tryCount)
		{
			return null;
		}

		Color randomColor = new Color(Random.value, Random.value, Random.value);
		for (int i = 0; i < 2; ++i)
		{
			splitedArea.Add(CreateArea(newRect[i], areaTree.Count + i, randomColor));
		}

		return splitedArea;
	}

	private Area CreateArea(Vector2 pos, Vector2 size, int index, Color? color = null)
	{
		GameObject areaObj = Instantiate(AreaPrefab);

		areaObj.name = $"Area_{index}";
		areaObj.transform.position = pos;
		areaObj.transform.localScale = size;
		areaObj.transform.SetParent(areaBox);

		Area areaScript = areaObj.GetComponent<Area>();
		areaScript.SetRect();
		areaScript.frame.UpdateGrid();
		areaScript.index = index;

		areaScript.frame.mySR.color = color ?? Color.white;

		return areaScript;
	}

	private Area CreateArea(Rect rect, int index, Color? color = null)
	{
		if (rect == Rect.zero)
		{
			return null;
		}

		GameObject areaObj = Instantiate(AreaPrefab);
		areaObj.name = $"Area_{index}";
		areaObj.transform.position = rect.center;
		areaObj.transform.localScale = new Vector2(rect.width, rect.height);
		areaObj.transform.SetParent(areaBox);

		Area areaScript = areaObj.GetComponent<Area>();
		areaScript.SetRect();
		areaScript.frame.UpdateGrid();
		areaScript.index = index;

		areaScript.frame.mySR.color = color ?? Color.white;

		return areaScript;
	}
	#endregion

	#region 2.Assigning Room
	public void AssignRooms()
	{
		foreach (var area in areaTree.GetLeafNodes())
		{
			roomList.Add(CreateFitRoom(area.Value,area.Value.frame.mySR.color));
		}
	}

	private Room CreateFitRoom(Area area, Color color)
	{
		var areaSize = area.rect.size;

		List<GameObject> availablePrefabs = new List<GameObject>();

		int differMin= int.MaxValue;

		//1. 사이즈 체크
		foreach (var prefab in RoomPrefabs)
		{
			Vector2 size = prefab.Key.transform.localScale;

			if (areaSize.x >= size.x + centerPosOffset && areaSize.y >= size.y + centerPosOffset)
			{
				availablePrefabs.Add(prefab.Key);

				float tempDiffer = Mathf.Abs(size.x * size.y - areaSize.x * areaSize.y);
				
				if (differMin > tempDiffer)
				{
					differMin = Mathf.CeilToInt(tempDiffer);
				}
			}
		}

		if(availablePrefabs.Count <= 0) 
		{
			Debug.Log("사이즈에 맞는 방을 찾지 못했습니다.");
			return null;
		}

		//2. 가장 사이즈 차이가 안나는거 찾기
			//대신 이 조건을 넣으면 맵 사이즈를 어떻게 설정하느냐에 따라서
			//작은 맵들은 안쓰일 경우가 많을꺼임;
			//그래서 일단 뺌
		//availablePrefabs.RemoveAll(x =>
		//(x.transform.localScale.x * x.transform.localScale.y) +differMin <= (areaSize.x * areaSize.y) );

		int rand = Random.Range(0, availablePrefabs.Count);

		GameObject newRoomObj = Instantiate(availablePrefabs[rand]);
		newRoomObj.name = $"Room_in ({area.index}) Area";
		newRoomObj.transform.position = area.rect.center;

		Room newRoomScript = newRoomObj.GetComponent<Room>();
		if(newRoomScript.mySR) newRoomScript.mySR.color = color;
		newRoomScript.belongsIndex = area.index;

		newRoomObj.transform.SetParent(roomBox);

		area.AssignedRoom = newRoomScript;

		return newRoomScript;
	}


	#endregion

	#region 3.Room Position Calibrate
	public void CalibratePosition()
	{
		foreach (var item in areaTree.GetLeafNodes())
		{
			Calibrate(item.Value, item.Value.AssignedRoom);
		}
	}

	private void Calibrate(Area area, Room origin)
	{
		Vector2 pos = Vector2.zero;

		int w = (int)origin.transform.localScale.x;
		int h = (int)origin.transform.localScale.y;

		float xMin = area.transform.position.x - (area.rect.width * 0.5f) + (w * 0.5f) + centerPosOffset;
		float xMax = area.transform.position.x + (area.rect.width * 0.5f) - (w * 0.5f) - centerPosOffset - 0.5f;
		pos.x = xMin >= xMax ? area.transform.position.x : Random.Range(xMin, xMax);

		float yMin = area.transform.position.y - (area.rect.height * 0.5f) + (h * 0.5f) + centerPosOffset;
		float yMax = area.transform.position.y + (area.rect.height * 0.5f) - (h * 0.5f) - centerPosOffset - 0.5f;
		pos.y = yMin >= yMax ? area.transform.position.y : Random.Range(yMin, yMax);
		//pos.y = Random.Range(yMin, yMax);

		pos.x = w % 2 == 0 ? Mathf.FloorToInt(pos.x) : Mathf.FloorToInt(pos.x) + 0.5f;
		pos.y = h % 2 == 0 ? Mathf.FloorToInt(pos.y) : Mathf.FloorToInt(pos.y) + 0.5f;

		origin.transform.position = pos;

		origin.UpdateRect();
	}

	#endregion

	#region 4.Connecting Room / Create Corridor

	public void ConnectingRoom()
	{ 
		for(int i = curSplitCount; i >= 0; i--) 
		{
			ConnectSiblingRoom(i);
		}
	}

	

	private void ConnectSiblingRoom(int depth)
	{
		var list = areaTree.GetCertainDepthNodes(depth);
		//var rooms = roomList.FindAll(x => x.belongsIndex == depth);

		for (int i = 0; i < list.Count; i += 2)
		{
			Room[] rooms = new Room[2];

			if (depth == 0)
			{ //루트 노드의 경우
				break;
			}
			else if (depth == curSplitCount)
			{
				rooms[0] = list[i].Value.AssignedRoom;
				rooms[1] = list[i + 1].Value.AssignedRoom;
			}
			else
			{
				var nearestRooms = GetNearestRooms(list[i], list[i + 1]);
				rooms[0] = nearestRooms[0];
				rooms[1] = nearestRooms[1];
			}

			Vector2[] min = new Vector2[2], max = new Vector2[2];

			for (int k = 0; k < 2; ++k)
			{
				Vector2 pos = rooms[k].transform.position;
				Vector2 size = rooms[k].transform.localScale;

				min[k].x = pos.x - (size.x * 0.5f);
				min[k].y = pos.y - (size.y * 0.5f);

				max[k].x = pos.x + (size.x * 0.5f);
				max[k].y = pos.y + (size.y * 0.5f);

				//Debug.Log(rooms[k].gameObject.name + min[k] + max[k]);
			}

			Rect corridorRect = Rect.zero;
			eDirection relate = eDirection.End;

			if (max[0].y > min[1].y && min[0].y < max[1].y)
			{
				relate = eDirection.Horizon;

				if (max[0].y >= max[1].y && min[0].y <= min[1].y)
				{//3번
					corridorRect.xMin = max[0].x;
					corridorRect.xMax = min[1].x;
					corridorRect.yMin = min[1].y;
					corridorRect.yMax = max[1].y;
					Debug.Log("3번");

				}
				else if (max[0].y < max[1].y && min[0].y > min[1].y)
				{//4번
					corridorRect.xMin = max[0].x;
					corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
					corridorRect.yMin = min[0].y;
					corridorRect.height = max[0].y - min[0].y;
					Debug.Log("4번");
				}
				else if (max[0].y < max[1].y)
				{//1번
					corridorRect.xMin = max[0].x;
					corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
					corridorRect.yMin = min[1].y;
					corridorRect.height = Mathf.Abs(max[0].y - min[1].y);
					Debug.Log("1번");
				}
				else if (min[0].y > min[1].y)
				{//2번
					corridorRect.xMin = max[0].x;
					corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
					corridorRect.yMin = min[0].y;
					corridorRect.height = Mathf.Abs(min[0].y - max[1].y);
					Debug.Log("2번");
				}
			}
			else if (max[0].x > min[1].x && min[0].x < max[1].x)
			{
				relate = eDirection.Vertical;

				if (min[0].x <= min[1].x && max[0].x >= max[1].x)
				{//3번
					corridorRect.xMin = min[1].x;
					corridorRect.xMax = max[1].x;

					corridorRect.yMin = max[1].y;
					corridorRect.yMax = min[0].y;

					Debug.Log("3번");
				}
				else if (min[0].x > min[1].x && max[0].x < min[1].x)
				{//4번
					corridorRect.xMin = min[0].x;
					corridorRect.xMax = max[0].x;

					corridorRect.yMin = max[1].y;
					corridorRect.yMax = min[0].y;

					Debug.Log("4번");
				}
				else if (min[0].x > min[1].x)
				{//1번

					corridorRect.xMin = min[0].x;
					corridorRect.xMax = max[1].x;

					corridorRect.yMin = max[1].y;
					corridorRect.yMax = min[0].y;
					Debug.Log("1번");
				}
				else if (max[0].x < max[1].x)
				{//2번

					corridorRect.xMin = min[1].x;
					corridorRect.xMax = max[0].x;
					corridorRect.yMin = max[1].y;
					corridorRect.yMax = min[0].y;

					Debug.Log("2번");
				}
			}

			var corridor = CreateCorridor(corridorRect, relate,rooms);

			if (corridor != null)
			{
				corridors.Add(corridor);
			}

		}

	}

	private Room[] GetNearestRooms(TreeNode<Area> leftNode, TreeNode<Area> rightNode)
	{
		if (leftNode == null | rightNode == null)
		{
			return null;
		}

		List<TreeNode<Area>> leftChildren = new List<TreeNode<Area>>();
		List<TreeNode<Area>> rightChildren = new List<TreeNode<Area>>();

		leftNode.GetLeafChildren(leftNode, ref leftChildren);
		rightNode.GetLeafChildren(rightNode, ref rightChildren);

		float dist = float.MaxValue;
		int indexLeft = 0, indexRight = 0;
		for (int i = 0; i < leftChildren.Count; ++i)
		{
			for (int k = 0; k < rightChildren.Count; ++k)
			{
				float tempDist = Vector2.Distance(leftChildren[i].Value.transform.position, rightChildren[k].Value.transform.position);

				if (tempDist < dist)
				{
					dist = tempDist;
					indexLeft = i;
					indexRight = k;
				}
			}
		}

		return new Room[2] { leftChildren[indexLeft].Value.AssignedRoom, rightChildren[indexRight].Value.AssignedRoom };
	}


	private Corridor CreateCorridor(Rect rect, eDirection relateDir, Room[] rooms)
	{
		//switch (relateDir)
		//{
		//	case eDirection.Vertical:
		//		{
		//			if (rect.width < CorridorPrefab.transform.localScale.x)
		//			{
		//				return null;
		//			}
		//		}
		//		break;
		//	case eDirection.Horizon:
		//		if (rect.height < CorridorPrefab.transform.localScale.x)
		//		{
		//			return null;
		//		}
		//		break;
		//	case eDirection.End:
		//		break;
		//	default:
		//		break;
		//}


		GameObject corridorObj = Instantiate(CorridorPrefab);

		Corridor script = corridorObj.GetComponent<Corridor>();



		Vector2 pos = Vector2.zero;
		Vector2 size = Vector2.zero;
		float width = corridorObj.transform.localScale.x;

		if (relateDir == eDirection.Horizon)
		{
			pos.x = rect.center.x;
			pos.y = rect.yMin + (Random.Range(Mathf.FloorToInt(width* 0.5f), (int)rect.height));
			pos.y += (int)width % 2 != 0 ? 0.5f : 0f;
			size = rect.size;
			size.y = CorridorPrefab.transform.localScale.x;
		}
		else
		{
			//pos.x = rect.xMin + Random.Range(0, (int)rect.width) + 0.5f;

			pos.x = rect.xMin + (Random.Range(Mathf.FloorToInt(width * 0.5f), (int)rect.width));
			pos.x += (int)width % 2 != 0 ? 0.5f : 0f;

			pos.y = rect.center.y;
			size = rect.size;
			size.x = CorridorPrefab.transform.localScale.x;
		}

		corridorBox.transform.position = pos;
		corridorBox.transform.localScale = size;
		script.grid.UpdateGrid();

		corridorObj.transform.SetParent(corridorBox);
		corridorObj.name = $"Corridor {rooms[0]}-{rooms[1]}";

		for (int i = 0; i < 2; ++i) 
		{
			script.linkedRooms.Add(rooms[i]);
		}

		return script;
	}

	#endregion

	public void GeneratingRandomDungeon()
	{

		//if (Divieding())
		//{
		//	ShrinkRooms();
		//	ConnectingRooms();
		//}
		//else
		//{
		//	ResetRooms();
		//	GeneratingRooms();
		//}
	}

	#region Old(before Clean)
	///// 1. 구역 나누기
	//public bool Divieding()
	//{
	//	int tryCount = divideTimes;

	//	if (dividedCount == divideTimes)
	//	{
	//		return false;
	//	}
	//	else if (dividedCount > divideTimes)
	//	{
	//		ResetRooms();
	//	}
	//	else if (dividedCount < divideTimes)
	//	{
	//		tryCount -= dividedCount;
	//	}


	//	for (int i = 0; i < tryCount; ++i)
	//	{
	//		Divied();
	//	}

	//	return true;
	//}
	//public void Divied()
	//{

	//	foreach (var node in roomTree.GetLeafNodes())
	//	{
	//		var newRooms = DiviedRoom(node.Value);

	//		roomTree.AddNode(node ,newRooms[0], newRooms[1]);

	//		node.Value.gameObject.SetActive(false);
	//	}

	//	++dividedCount;
	//	++curCorridorDepth;

	//	roomCount = roomTree.GetLeafNodes().Count;
	//}
	//private List<Room> DiviedRoom(Room room)
	//{
	//	List<Room> tempRooms = new List<Room>();

	//	////0 = row (가로로 나누기)
	//	////1 = Col (세로로 나누기)
	//	int divideDir = room.transform.localScale.x / room.transform.localScale.y >= 1f ? 1 : 0;
	//	float divideRatio = Random.Range(minDivideRatio, maxDivideRatio);

	//	//0 = left / 1 = right
	//	//0 = top / 1 = bot
	//	Vector2[] newVertex = new Vector2[2];
	//	CornerPos[] corner = new CornerPos[2];
	//	Room[] newRoom = new Room[2];

	//	if (divideDir == 0)
	//	{//가로로 나눌 때 
	//	 //Left
	//		newVertex[0] = new Vector2(room.cornerPos.LT.x,
	//								(room.cornerPos.LT.y - room.cornerPos.LB.y) * divideRatio + room.cornerPos.LB.y);
	//		//Right
	//		newVertex[1] = new Vector2(room.cornerPos.RT.x,
	//								(room.cornerPos.RT.y - room.cornerPos.RB.y) * divideRatio + room.cornerPos.RB.y);

	//		corner[0] = new CornerPos(room.cornerPos.LT, room.cornerPos.RT, newVertex[1], newVertex[0]);
	//		corner[1] = new CornerPos(newVertex[0], newVertex[1], room.cornerPos.RB, room.cornerPos.LB);

	//		for (int i = 0; i < 2; ++i)
	//		{
	//			newRoom[i] = CreateRoom(corner[i],roomTree.Count+ i);
	//		}
	//	}
	//	else
	//	{
	//		newVertex[0] =
	//				new Vector2((room.cornerPos.RT.x - room.cornerPos.LT.x) * divideRatio + room.cornerPos.LT.x,
	//				room.cornerPos.LT.y);
	//		newVertex[1] =
	//				new Vector2((room.cornerPos.RB.x - room.cornerPos.LB.x) * divideRatio + room.cornerPos.LB.x,
	//				room.cornerPos.LB.y);

	//		newRoom[0] = CreateRoom(new CornerPos(room.cornerPos.LT, newVertex[0], newVertex[1], room.cornerPos.LB), roomTree.Count);
	//		newRoom[1] = CreateRoom(new CornerPos(newVertex[0], room.cornerPos.RT, room.cornerPos.RB, newVertex[1]), roomTree.Count + 1);
	//	}

	//	foreach (var item in newRoom)
	//	{
	//		tempRooms.Add(item);
	//	}

	//	return tempRooms;
	//}
	//private Room CreateRoom(CornerPos corner, int treeIndex)
	//{
	//	Vector2 size = new Vector2(Vector2.Distance(corner.LT, corner.RT), Vector2.Distance(corner.LT, corner.LB));
	//	Vector2 pos = new Vector2(corner.LT.x + size.x * 0.5f, corner.LT.y - size.y * 0.5f);

	//	GameObject newRoom = Instantiate(TestRoomPrefab);
	//	newRoom.name += $"({treeIndex})";
	//	newRoom.transform.SetParent(transform);
	//	newRoom.transform.position = pos;
	//	newRoom.transform.localScale = new Vector2(Mathf.FloorToInt(size.x), Mathf.FloorToInt(size.y)) /** 0.95f*/;

	//	Room roomScript = newRoom.GetComponent<Room>();
	//	roomScript.cornerPos.CalcCorner(newRoom.transform);
	//	roomScript.roomIndex = treeIndex;

	//	newRoom.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f,1f), Random.Range(0f,1f) ,Random.Range(0f,1f), 0.5f);

	//	SpriteGrid grid = newRoom.GetComponentInChildren<SpriteGrid>();
	//	grid.UpdateGrid();

	//	return roomScript;
	//}


	///2. 나눈 구역 보다 작게 방 만들기
	//public void ShrinkRooms()
	//{
	//	foreach (var node in roomTree.GetLeafNodes())
	//	{
	//		Shrink(node.Value);
	//	}
	//}


	//private void Shrink(Room room)
	//{
	//	Vector2 size = room.transform.localScale;
	//	Vector2 pos = room.transform.position;
	//	int w = Mathf.FloorToInt(Random.Range(size.x * 0.5f, size.x - xOffset));
	//	int h = Mathf.FloorToInt(Random.Range(size.y * 0.5f, size.y - yOffset));

	//	float x = Random.Range(pos.x - size.x * 0.5f + w * 0.5f, pos.x + size.x * 0.5f - w * 0.5f);
	//	float y = Random.Range(pos.y - size.y * 0.5f + h * 0.5f, pos.y + size.y * 0.5f - h * 0.5f);

	//	x = Mathf.FloorToInt(x);
	//	y = Mathf.FloorToInt(y);

	//	x = w % 2 == 0 ? x : x + 0.5f;
	//	y = h % 2 == 0 ? y : y + 0.5f;

	//	room.transform.position = new Vector2(x, y);
	//	room.transform.localScale = new Vector2(w, h);

	//	room.cornerPos.CalcCorner(room.transform);

	//	SpriteGrid grid = room.GetComponentInChildren<SpriteGrid>();
	//	grid.UpdateGrid();

	//}

	////// 양옆(형제 노드)들의 방끼리 이어주기 
	////// 모든 depth 에서.
	//public void ConnectingRooms()
	//{
	//	int fullDepth = dividedCount;

	//	for (int i = fullDepth; i > 0; i--)
	//	{
	//		ConnectSiblingRoom(i);
	//	}
	//}

	//public void NewConnectSiblingRoom(int depth)
	//{
	//	//겹치는 부분 (충돌은 아님ㅎ)에서 통로 만들기 

	//	//코드는 나중에 정리하기 ㅋㅋ
	//	var list = roomTree.GetCertainDepthNodes(depth);

	//	for (int k = 0; k < list.Count; k += 2)
	//	{

	//		Room[] rooms = new Room[2];
	//		//Room olderRoom = null;
	//		//Room youngerRoom = null;

	//		if (depth == dividedCount)
	//		{//Leaf Nodes 일 경우
	//			rooms[0] = list[k].Value;
	//			rooms[1] = list[k + 1].Value;
	//		}
	//		else if (depth == 0)
	//		{//Root 일 경우

	//			break;
	//		}
	//		else
	//		{ //그외 칭긔 칭긔
	//		  //하단 노드 4개를 비교해서 가장 가까운 2개 연결 해보기

	//			var nearestRooms = GetNearChildrenNode(list[k], list[k + 1]);

	//			rooms[0] = nearestRooms[0];
	//			rooms[1] = nearestRooms[1];
	//		}

	//		Vector2[] min = new Vector2[2], max = new Vector2[2];

	//		for (int j = 0; j < 2; ++j)
	//		{
	//			Vector2 pos = rooms[j].transform.position;
	//			Vector2 size = rooms[j].transform.localScale;

	//			min[j].x = pos.x - (size.x * 0.5f);
	//			min[j].y = pos.y - (size.y * 0.5f);

	//			max[j].x = pos.x + (size.x * 0.5f);
	//			max[j].y = pos.y + (size.y * 0.5f);

	//			Debug.Log(rooms[j].gameObject.name + min[j] + max[j]);
	//			//min[j] = pos - size * 0.5f;
	//			//max[j] = pos + size * 0.5f;
	//		}

	//		Rect corridorRect = Rect.zero;
	//		BesideRelate relate = BesideRelate.End;


	//		if (max[0].y > min[1].y && min[0].y < max[1].y)
	//		{
	//			relate = BesideRelate.Horizon;

	//			if (max[0].y >= max[1].y && min[0].y <= min[1].y)
	//			{//3번
	//				corridorRect.xMin = max[0].x;
	//				corridorRect.xMax = min[1].x;
	//				//corridorRect.width = Mathf.Abs(min[1].x - max[0].x);
	//				corridorRect.yMin = min[1].y;
	//				corridorRect.yMax = max[1].y;
	//				//corridorRect.height = max[1].y - min[1].y;
	//				Debug.Log("3번");

	//			}
	//			else if (max[0].y < max[1].y && min[0].y > min[1].y)
	//			{//4번
	//				corridorRect.xMin = max[0].x;
	//				corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
	//				corridorRect.yMin = min[0].y;
	//				corridorRect.height = max[0].y - min[0].y;
	//				Debug.Log("4번");
	//			}
	//			else if (max[0].y < max[1].y)
	//			{//1번
	//				corridorRect.xMin = max[0].x;
	//				corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
	//				corridorRect.yMin = min[1].y;
	//				corridorRect.height = Mathf.Abs(max[0].y - min[1].y);
	//				Debug.Log("1번");
	//			}
	//			else if (min[0].y > min[1].y)
	//			{//2번
	//				corridorRect.xMin = max[0].x;
	//				corridorRect.width = Mathf.Abs(max[0].x - min[1].x);
	//				corridorRect.yMin = min[0].y;
	//				corridorRect.height = Mathf.Abs(min[0].y - max[1].y);
	//				Debug.Log("2번");
	//			}
	//			//corridorRect.xMin = max[0].x < min[1].x ? max[0].x : max[1].x;
	//			//corridorRect.width = Mathf.Abs((max[0].x < max[1].x ? min[1].x : min[0].x) - corridorRect.x);

	//			//corridorRect.yMax = max[0].y <= max[1].y ?  max[0].y : max[1].y;
	//			//corridorRect.height = Mathf.Abs((min[0].y > min[1].y ? min[0].y : min[1].y) -corridorRect.y);
	//		}
	//		else if (max[0].x > min[1].x && min[0].x < max[1].x)
	//		{
	//			relate = BesideRelate.Vertical;

	//			if (min[0].x <= min[1].x && max[0].x >= max[1].x)
	//			{//3번
	//				corridorRect.xMin = min[1].x;
	//				corridorRect.xMax = max[1].x;

	//				corridorRect.yMin = max[1].y;
	//				corridorRect.yMax = min[0].y;

	//				Debug.Log("3번");
	//			}
	//			else if (min[0].x > min[1].x && max[0].x < min[1].x)
	//			{//4번
	//				corridorRect.xMin = min[0].x;
	//				corridorRect.xMax = max[0].x;

	//				corridorRect.yMin = max[1].y;
	//				corridorRect.yMax = min[0].y;

	//				Debug.Log("4번");
	//			}
	//			else if (min[0].x > min[1].x)
	//			{//1번

	//				corridorRect.xMin = min[0].x;
	//				corridorRect.xMax = max[1].x;

	//				corridorRect.yMin = max[1].y;
	//				corridorRect.yMax = min[0].y;
	//				Debug.Log("1번");
	//			}
	//			else if (max[0].x < max[1].x)
	//			{//2번

	//				corridorRect.xMin = min[1].x;
	//				corridorRect.xMax = max[0].x;
	//				corridorRect.yMin = max[1].y;
	//				corridorRect.yMax = min[0].y;

	//				Debug.Log("2번");
	//			}

	//		}

	//		Debug.Log(corridorRect);

	//		var corridor = CreateCorridor(corridorRect, depth, relate);


	//		GameObject corridorBox = new GameObject("CorridorBox");
	//		corridorBox.transform.SetParent(this.transform);
	//		corridorBox.name += $"({rooms[0].roomIndex} - {rooms[1].roomIndex})";
	//		corridor.transform.SetParent(corridorBox.transform);
	//		corridorBoxes.Add(corridorBox);


	//		corridors.Add(corridor.GetComponent<Corridor>());


	//		rooms[0].linkedRooms.Add(rooms[1]);
	//		rooms[1].linkedRooms.Add(rooms[0]);
	//	}


	//	curCorridorDepth += curCorridorDepth != 0 ? -1 : 0;
	//}

	//private Room[] GetNearChildrenNode(TreeNode<Room> leftNode, TreeNode<Room> rightNode)
	//{
	//	if (leftNode.LeftNode == null | rightNode.LeftNode == null)
	//	{
	//		return null;
	//	}

	//	List<TreeNode<Room>> leftChildren = new List<TreeNode<Room>>();
	//	List<TreeNode<Room>> rightChildren = new List<TreeNode<Room>>();

	//	leftNode.GetAllChildren(leftNode, ref leftChildren);
	//	rightNode.GetAllChildren(rightNode, ref rightChildren);

	//	float dist = float.MaxValue;
	//	int indexLeft = 0, indexRight = 0;
	//	for (int i = 0; i < leftChildren.Count; ++i)
	//	{
	//		for (int k = 0; k < rightChildren.Count; ++k)
	//		{
	//			float tempDist = Vector2.Distance(leftChildren[i].Value.transform.position, rightChildren[k].Value.transform.position);

	//			if (tempDist < dist)
	//			{
	//				dist = tempDist;
	//				indexLeft = i;
	//				indexRight = k;
	//			}
	//		}
	//	}

	//	return new Room[2] { leftChildren[indexLeft].Value, rightChildren[indexRight].Value };
	//}

	//private GameObject CreateCorridor(Vector2 startPos, Vector2 endPos, int depth)
	//{
	//	//일단은 따로 만들고 나중에 하나로 합치기 
	//	float w = Mathf.Abs(startPos.x - endPos.x);
	//	float h = Mathf.Abs(startPos.y - endPos.y);
	//	Vector2 centerPos = (startPos + endPos) * 0.5f;

	//	GameObject corridorObj = Instantiate(TestCorridorPrefab);

	//	corridorObj.transform.position = centerPos;
	//	corridorObj.transform.localScale = new Vector2(Mathf.Clamp(w, 1, w), Mathf.Clamp(h, 1, h));

	//	var renderer = corridorObj.GetComponent<SpriteRenderer>();
	//	renderer.color = depth == 4 ? Color.white : Defines.rainbow[depth];

	//	return corridorObj;
	//}

	//private GameObject CreateCorridor(Rect rect, int depth, BesideRelate relate)
	//{
	//	GameObject corridorObj = Instantiate(TestCorridorPrefab);

	//	Vector2 pos = Vector2.zero;
	//	Vector2 size = Vector2.zero;
	//	//float ratio = rect.width / rect.height;

	//	if (relate == BesideRelate.Horizon)
	//	{
	//		pos.x = rect.center.x;
	//		pos.y = rect.yMin + (Random.Range(0, (int)rect.height) + 0.5f);
	//		size = rect.size;
	//		size.y = 1f;
	//	}
	//	else
	//	{
	//		pos.x = rect.xMin + Random.Range(0, (int)rect.width) + 0.5f;
	//		pos.y = rect.center.y;
	//		size = rect.size;
	//		size.x = 1f;
	//	}


	//	corridorObj.transform.position = pos;
	//	corridorObj.transform.localScale = size;

	//	var renderer = corridorObj.GetComponent<SpriteRenderer>();
	//	renderer.color = depth == 4 ? Color.white : Defines.rainbow[depth];

	//	SpriteGrid grid = corridorObj.GetComponentInChildren<SpriteGrid>();
	//	grid.UpdateGrid();

	//	return corridorObj;
	//}


	//public void ResetRooms()
	//{
	//	foreach (var node in roomTree.nodeList)
	//	{
	//		Destroy(node.Value.gameObject);
	//		node.Value = null;
	//		node.MotherNode = null;
	//		node.SiblingNode = null;
	//		node.LeftNode = null;
	//		node.RightNode = null;
	//	}

	//	dividedCount = 0;
	//	curCorridorDepth = 0;

	//	foreach (var item in corridorBoxes)
	//	{
	//		Destroy(item);
	//	}
	//	corridors.Clear();
	//	corridors = new List<Corridor>();

	//	CreateInitDungeon();
	//}
	#endregion


	public void ResetDungeon()
	{
		
	}

	public void ResetArea()
	{
		for (int i = 0; i < areaBox.transform.childCount; ++i)
		{
			Destroy(areaBox.transform.GetChild(i).gameObject);
		}

		areaTree = new Tree<Area>();

		Debug.Log("구역 초기화 완료");
	}

	public void ResetRooms()
	{
		for (int i = 0; i < roomBox.transform.childCount; ++i)
		{
			Destroy(roomBox.transform.GetChild(i).gameObject);
		}

		roomList = new();

		Debug.Log("Room 초기화 완료");
	}

	public void ResetCorridors()
	{
	
	}

	public void SetActiveAllArea(bool active)
	{

	}

	public void SetActiveAllRooms(bool active) 
	{
	
	}


	

	private void Awake()
	{
		Initialize();
	}




	private void Update()
	{

		//Zoom();

		if (Input.GetKeyDown(KeyCode.Space))
		{
			GeneratingRandomDungeon();
		}
	}


	private void OnDestroy()
	{
		areaTree = null;
	}

}
