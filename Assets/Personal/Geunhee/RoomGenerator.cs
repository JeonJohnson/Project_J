using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using AYellowpaper.SerializedCollections;

using Structs;
using JeonJohnson;




public struct RoomStatus
{ 


}

public enum BesideRelate
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
	public Vector2 dungeonSize;
	[SerializedDictionary("Room Prefab", "Map Size")]
	public SerializedDictionary<GameObject, Vector2Int> unitPrefab;
	public GameObject TestRoomPrefab;
	public GameObject TestCorridorPrefab;
	//public int maxRoomCount;

	[Range(0f, 1f)]
	public float minDivideRatio, maxDivideRatio;
	public int divideTimes;
	[Tooltip("구역 줄여서 방 생성할때, 최소 최대 비율")]
	public float xOffset, yOffset;


	//[Tooltip("These Values are Inclusive")]
	//public Vector2 minRoomSize, maxRoomSize;


	[Space(10f)]
	[Header("Display Vals")]
	//[ReadOnly]
	public int dividedCount;
	[ReadOnly]
	public int roomCount = 1;

	[ReadOnly]
	public int curCorridorDepth = 0;
	//public List<Room> rooms;

	[HideInInspector]
	public JeonJohnson.Tree<Room> roomTree;
	public List<Corridor> corridors;
	public List<GameObject> corridorBoxes;
	////public Vector2 expectLeastRoomSize, expectMostRoomSize;
	//public void Test()
	//{ 

	//}

	public void GeneratingRooms()
	{

		if (Divieding())
		{
			ShrinkRooms();
			ConnectingRooms();
		}
		else
		{
			ResetRooms();
			GeneratingRooms();
		}
	}


	/// 1. 구역 나누기
	public bool Divieding()
	{
		int tryCount = divideTimes;

		if (dividedCount == divideTimes)
		{
			return false;
		}
		else if (dividedCount > divideTimes)
		{
			ResetRooms();
		}
		else if (dividedCount < divideTimes)
		{
			tryCount -= dividedCount;
		}


		for (int i = 0; i < tryCount; ++i)
		{
			Divied();
		}

		return true;
	}
	public void Divied()
	{

		foreach (var node in roomTree.GetLeafNodes())
		{
			var newRooms = DiviedRoom(node.Value);

			roomTree.AddNode(node ,newRooms[0], newRooms[1]);

			node.Value.gameObject.SetActive(false);
		}

		++dividedCount;
		++curCorridorDepth;

		roomCount = roomTree.GetLeafNodes().Count;
	}

	private List<Room> DiviedRoom(Room room)
	{
		List<Room> tempRooms = new List<Room>();

		////0 = row (가로로 나누기)
		////1 = Col (세로로 나누기)
		int divideDir = room.transform.localScale.x / room.transform.localScale.y >= 1f ? 1 : 0;
		float divideRatio = Random.Range(minDivideRatio, maxDivideRatio);

		//0 = left / 1 = right
		//0 = top / 1 = bot
		Vector2[] newVertex = new Vector2[2];
		CornerPos[] corner = new CornerPos[2];
		Room[] newRoom = new Room[2];

		if (divideDir == 0)
		{//가로로 나눌 때 
		 //Left
			newVertex[0] = new Vector2(room.cornerPos.LT.x,
									(room.cornerPos.LT.y - room.cornerPos.LB.y) * divideRatio + room.cornerPos.LB.y);
			//Right
			newVertex[1] = new Vector2(room.cornerPos.RT.x,
									(room.cornerPos.RT.y - room.cornerPos.RB.y) * divideRatio + room.cornerPos.RB.y);

			corner[0] = new CornerPos(room.cornerPos.LT, room.cornerPos.RT, newVertex[1], newVertex[0]);
			corner[1] = new CornerPos(newVertex[0], newVertex[1], room.cornerPos.RB, room.cornerPos.LB);

			for (int i = 0; i < 2; ++i)
			{
				newRoom[i] = CreateRoom(corner[i],roomTree.Count+ i);
			}
		}
		else
		{
			newVertex[0] =
					new Vector2((room.cornerPos.RT.x - room.cornerPos.LT.x) * divideRatio + room.cornerPos.LT.x,
					room.cornerPos.LT.y);
			newVertex[1] =
					new Vector2((room.cornerPos.RB.x - room.cornerPos.LB.x) * divideRatio + room.cornerPos.LB.x,
					room.cornerPos.LB.y);

			newRoom[0] = CreateRoom(new CornerPos(room.cornerPos.LT, newVertex[0], newVertex[1], room.cornerPos.LB), roomTree.Count);
			newRoom[1] = CreateRoom(new CornerPos(newVertex[0], room.cornerPos.RT, room.cornerPos.RB, newVertex[1]), roomTree.Count + 1);
		}

		foreach (var item in newRoom)
		{
			tempRooms.Add(item);
		}

		return tempRooms;
	}


	private Room CreateRoom(CornerPos corner, int treeIndex)
	{
		Vector2 size = new Vector2(Vector2.Distance(corner.LT, corner.RT), Vector2.Distance(corner.LT, corner.LB));
		Vector2 pos = new Vector2(corner.LT.x + size.x * 0.5f, corner.LT.y - size.y * 0.5f);

		GameObject newRoom = Instantiate(TestRoomPrefab);
		newRoom.name += $"({treeIndex})";
		newRoom.transform.SetParent(transform);
		newRoom.transform.position = pos;
		newRoom.transform.localScale = new Vector2(Mathf.FloorToInt(size.x), Mathf.FloorToInt(size.y)) /** 0.95f*/;

		Room roomScript = newRoom.GetComponent<Room>();
		roomScript.cornerPos.CalcCorner(newRoom.transform);
		roomScript.roomIndex = treeIndex;

		newRoom.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f,1f), Random.Range(0f,1f) ,Random.Range(0f,1f), 0.5f);
		
		SpriteGrid grid = newRoom.GetComponentInChildren<SpriteGrid>();
		grid.UpdateGrid();

		return roomScript;
	}


	///2. 나눈 구역 보다 작게 방 만들기
	public void ShrinkRooms()
	{
		foreach (var node in roomTree.GetLeafNodes())
		{
			Shrink(node.Value);
		}
	}


	private void Shrink(Room room)
	{
		Vector2 size = room.transform.localScale;
		Vector2 pos = room.transform.position;
		int w = Mathf.FloorToInt(Random.Range(size.x * 0.5f, size.x - xOffset));
		int h = Mathf.FloorToInt(Random.Range(size.y * 0.5f, size.y - yOffset));

		float x = Random.Range(pos.x - size.x * 0.5f + w * 0.5f, pos.x + size.x * 0.5f - w * 0.5f);
		float y = Random.Range(pos.y - size.y * 0.5f + h * 0.5f, pos.y + size.y * 0.5f - h * 0.5f);

		x = Mathf.FloorToInt(x);
		y = Mathf.FloorToInt(y);

		x = w % 2 == 0 ? x : x + 0.5f;
		y = h % 2 == 0 ? y : y + 0.5f;

		room.transform.position = new Vector2(x,y);
		room.transform.localScale = new Vector2(w, h);

		room.cornerPos.CalcCorner(room.transform);

		SpriteGrid grid = room.GetComponentInChildren<SpriteGrid>();
		grid.UpdateGrid();

	}

	//// 양옆(형제 노드)들의 방끼리 이어주기 
	//// 모든 depth 에서.
	public void ConnectingRooms()
	{
		int fullDepth = dividedCount;

		for (int i = fullDepth; i > 0; i--)
		{
			ConnectSiblingRoom(i);
		}
	}


	public void ConnectSiblingRoom(int i)
	{
		var list = roomTree.GetCertainDepthNodes(i);

		for (int k = 0; k < list.Count; k += 2)
		{
			Room olderRoom = null;
			Room youngerRoom = null;

			if (i == dividedCount)
			{//Leaf Nodes 일 경우
				olderRoom = list[k].Value;
				youngerRoom = list[k + 1].Value;
			}
			else if (i == 0)
			{//Root 일 경우

				break;
			}
			else
			{ //그외 칭긔 칭긔
			  //하단 노드 4개를 비교해서 가장 가까운 2개 연결 해보기

				var nearestRooms = GetNearChildrenNode(list[k], list[k + 1]);

				olderRoom = nearestRooms[0];
				youngerRoom = nearestRooms[1];
			}

			Vector2 olderRoomPos = olderRoom.transform.position;
			Vector2 youngerRoomPos = youngerRoom.transform.position;

			//왼쪽(형 노드)방의 세로(y값)을 기준으로 일단 선 하나
			Vector2 startPos = new Vector2(olderRoomPos.x, olderRoomPos.y);
			Vector2 endPos = new Vector2(youngerRoomPos.x, olderRoomPos.y);
			GameObject corridor1 = CreateCorridor(startPos, endPos, i);

			//오른쪽(동생 노드)방의 가로(x값)을 기준으로 일단 선 하나 더
			Vector2 startPos2 = new Vector2(youngerRoomPos.x, olderRoomPos.y);
			Vector2 endPos2 = new Vector2(youngerRoomPos.x, youngerRoomPos.y);
			GameObject corridor2 = CreateCorridor(startPos2, endPos2, i);

			GameObject corridorBox = new GameObject("CorridorBox");
			corridorBox.transform.SetParent(this.transform);
			corridorBox.name += $"({olderRoom.roomIndex} - {youngerRoom.roomIndex})";
			corridor1.transform.SetParent(corridorBox.transform);
			corridor2.transform.SetParent(corridorBox.transform);
			corridorBoxes.Add(corridorBox);


			corridors.Add(corridor1.GetComponent<Corridor>());
			corridors.Add(corridor2.GetComponent<Corridor>());


			olderRoom.linkedRooms.Add(youngerRoom);
			youngerRoom.linkedRooms.Add(olderRoom);
		}


		curCorridorDepth += curCorridorDepth != 0 ? - 1 : 0;
	}
	public void NewConnectSiblingRoom(int depth)
	{
		//겹치는 부분 (충돌은 아님ㅎ)에서 통로 만들기 

		//코드는 나중에 정리하기 ㅋㅋ
		var list = roomTree.GetCertainDepthNodes(depth);

		for (int k = 0; k < list.Count; k += 2)
		{

			Room[] rooms = new Room[2];
			//Room olderRoom = null;
			//Room youngerRoom = null;

			if (depth == dividedCount)
			{//Leaf Nodes 일 경우
				rooms[0] = list[k].Value;
				rooms[1] = list[k + 1].Value;
			}
			else if (depth == 0)
			{//Root 일 경우

				break;
			}
			else
			{ //그외 칭긔 칭긔
			  //하단 노드 4개를 비교해서 가장 가까운 2개 연결 해보기

				var nearestRooms = GetNearChildrenNode(list[k], list[k + 1]);

				rooms[0] = nearestRooms[0];
				rooms[1] = nearestRooms[1];
			}

			Vector2[] min = new Vector2[2], max = new Vector2[2];

			for (int j = 0; j < 2; ++j)
			{
				Vector2 pos = rooms[j].transform.position;
				Vector2 size = rooms[j].transform.localScale;

				min[j].x = pos.x - (size.x * 0.5f);
				min[j].y = pos.y - (size.y * 0.5f);

				max[j].x = pos.x + (size.x * 0.5f);
				max[j].y = pos.y + (size.y * 0.5f);

				Debug.Log(rooms[j].gameObject.name + min[j] + max[j]);
				//min[j] = pos - size * 0.5f;
				//max[j] = pos + size * 0.5f;
			}

			Rect corridorRect = Rect.zero;
			BesideRelate relate = BesideRelate.End;
		
		
			if (max[0].y > min[1].y && min[0].y < max[1].y)
			{
				relate = BesideRelate.Horizon;

				if (max[0].y >= max[1].y && min[0].y <= min[1].y)
				{//3번
					corridorRect.xMin = max[0].x;
					corridorRect.xMax = min[1].x;
					//corridorRect.width = Mathf.Abs(min[1].x - max[0].x);
					corridorRect.yMin = min[1].y;
					corridorRect.yMax = max[1].y;
					//corridorRect.height = max[1].y - min[1].y;
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
				//corridorRect.xMin = max[0].x < min[1].x ? max[0].x : max[1].x;
				//corridorRect.width = Mathf.Abs((max[0].x < max[1].x ? min[1].x : min[0].x) - corridorRect.x);

				//corridorRect.yMax = max[0].y <= max[1].y ?  max[0].y : max[1].y;
				//corridorRect.height = Mathf.Abs((min[0].y > min[1].y ? min[0].y : min[1].y) -corridorRect.y);
			}
			else if ( max[0].x > min[1].x && min[0].x < max[1].x)
			{
				relate = BesideRelate.Vertical;

				if (min[0].x <= min[1].x && max[0].x >= max[1].x)
				{//3번
					corridorRect.xMin = min[1].x;
					corridorRect.xMax = max[1].x;

					corridorRect.yMin = max[1].y;
					corridorRect.yMax = min[0].y;

					Debug.Log("3번");
				}
				else if (min[0].x >= min[1].x && max[0].x <= min[1].x)
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

				
				//corridorRect.xMin = min[0].x <= min[1].x ? min[0].x : min[1].x;
				//corridorRect.width = Mathf.Abs((max[0].x >= max[1].x ? max[1].x : max[0].x) - corridorRect.x);
				//corridorRect.yMin = min[0].y > max[1].y ? min[0].y : min[1].y;
				//corridorRect.height =  Mathf.Abs((min[0].y > max[1].y ? max[1].y : max[0].y) - corridorRect.y);


			}
			else 
			{ //아예 떨어져 있는 경우
				//원래 방법대로
				
			}

			Debug.Log(corridorRect);

			var corridor =  CreateCorridor(corridorRect, depth,relate);


			GameObject corridorBox = new GameObject("CorridorBox");
			corridorBox.transform.SetParent(this.transform);
			corridorBox.name += $"({rooms[0].roomIndex} - {rooms[1].roomIndex})";
			corridor.transform.SetParent(corridorBox.transform);
			corridorBoxes.Add(corridorBox);


			corridors.Add(corridor.GetComponent<Corridor>());


			rooms[0].linkedRooms.Add(rooms[1]);
			rooms[1].linkedRooms.Add(rooms[0]);
		}


		curCorridorDepth += curCorridorDepth != 0 ? -1 : 0;
	}

	private Room[] GetNearChildrenNode(TreeNode<Room> leftNode, TreeNode<Room> rightNode)
	{
		if (leftNode.LeftNode == null | rightNode.LeftNode == null)
		{
			return null;
		}

		List<TreeNode<Room>> leftChildren = new List<TreeNode<Room>>();
		List<TreeNode<Room>> rightChildren = new List<TreeNode<Room>>();

		leftNode.GetAllChildren(leftNode, ref leftChildren);
		rightNode.GetAllChildren(rightNode, ref rightChildren);

		float dist = float.MaxValue;
		int indexLeft = 0, indexRight= 0; 
		for (int i = 0; i < leftChildren.Count; ++i)
		{
			for (int k = 0; k < rightChildren.Count; ++k)
			{
				float tempDist = Vector2.Distance(leftChildren[i].Value.transform.position, rightChildren[k].Value.transform.position);

				if(tempDist < dist)
				{
					dist = tempDist;
					indexLeft = i;
					indexRight = k;
				}
			}
		}

		return new Room[2] { leftChildren[indexLeft].Value, rightChildren[indexRight].Value };
	}

	private GameObject CreateCorridor(Vector2 startPos, Vector2 endPos, int depth)
	{
		//일단은 따로 만들고 나중에 하나로 합치기 
		float w = Mathf.Abs(startPos.x -  endPos.x);
		float h = Mathf.Abs(startPos.y - endPos.y);
		Vector2 centerPos = (startPos + endPos) * 0.5f;

		GameObject corridorObj = Instantiate(TestCorridorPrefab);

		corridorObj.transform.position = centerPos;
		corridorObj.transform.localScale = new Vector2(Mathf.Clamp(w,1,w), Mathf.Clamp(h,1,h));

		var renderer = corridorObj.GetComponent<SpriteRenderer>();
		renderer.color = depth == 4 ? Color.white : Defines.rainbow[depth];

		return corridorObj;
	}

	private GameObject CreateCorridor(Rect rect, int depth, BesideRelate relate)
	{
		GameObject corridorObj = Instantiate(TestCorridorPrefab);

		Vector2 pos = Vector2.zero;
		Vector2 size = Vector2.zero;
		//float ratio = rect.width / rect.height;

		if (relate == BesideRelate.Horizon)
		{
			pos.x = rect.center.x;
			pos.y = rect.yMin + (Random.Range(0, (int)rect.height) + 0.5f);
			size = rect.size;
			size.y = 1f;
		}
		else
		{
			pos.x = rect.xMin + Random.Range(0, (int)rect.width) + 0.5f;
			pos.y = rect.center.y;
			size = rect.size;
			size.x = 1f;
		}


		corridorObj.transform.position = pos;
		corridorObj.transform.localScale = size;

		var renderer = corridorObj.GetComponent<SpriteRenderer>();
		renderer.color = depth == 4 ? Color.white : Defines.rainbow[depth];

		SpriteGrid grid = corridorObj.GetComponentInChildren<SpriteGrid>();
		grid.UpdateGrid();

		return corridorObj;
	}


	public void ResetRooms()
	{
		foreach (var node in roomTree.nodeList)
		{
			Destroy(node.Value.gameObject);
			node.Value = null;
			node.MotherNode = null;
			node.SiblingNode = null;
			node.LeftNode = null;
			node.RightNode = null;
		}

		dividedCount = 0;
		curCorridorDepth = 0;

		foreach (var item in corridorBoxes)
		{
			Destroy(item);
		}
		corridors.Clear();
		corridors = new List<Corridor>();
			
		CreateInitDungeon();
	}


	private void CreateInitDungeon()
	{
		GameObject roomObj = Instantiate(TestRoomPrefab);
		roomObj.transform.SetParent(transform);
		roomObj.transform.localScale = dungeonSize;
		Room room = roomObj.GetComponent<Room>();
		room.cornerPos.CalcCorner(roomObj.transform);
		roomObj.name += "(0)";
		//rooms.Add(roomScript);
		roomTree = new Tree<Room>(room);

		SpriteGrid grid = roomObj.GetComponentInChildren<SpriteGrid>();
		grid.UpdateGrid();

		roomCount = 1;
	}


	private void Awake()
	{
		dividedCount = 0;

		CreateInitDungeon();
	}


	private void Start()
	{
		
	}
	

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GeneratingRooms();
		}
	}


	private void OnDestroy()
	{
		roomTree = null;
	}

}
