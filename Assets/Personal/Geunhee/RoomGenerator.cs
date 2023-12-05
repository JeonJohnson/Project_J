using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Structs;
using JeonJohnson;

using UnityEditor.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using TreeEditor;


public struct RoomStatus
{ 


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
	public GameObject RoomPrefab;
	public GameObject CorridorPrefab;
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
	[ReadOnly]
	public int dividedCount;
	[ReadOnly]
	public int roomCount = 1;

	[ReadOnly]
	public int curCorridorDepth = 0;
	//public List<Room> rooms;

	[HideInInspector]
	public JeonJohnson.Tree<Room> roomTree;
	public List<Corridor> corridors;

	////public Vector2 expectLeastRoomSize, expectMostRoomSize;
	//public void Test()
	//{ 

	//}

	public void GeneratingRooms()
	{ 
	
	}







	/// 1. 구역 나누기
	//public void Divide_Old()
	//{
	//	if (rooms.Count >= maxRoomCount)
	//	{
	//		return;
	//	}

	//	List<Room> tempRooms = rooms.ToList(); 
	//	List<Room> newRooms = new List<Room>(); 

	//	foreach (var room in rooms)
	//	{
	//		if (tempRooms.Count >= maxRoomCount)
	//		{ //방 하나 나눴는데 만약 개수 넘어가는 경우 그냥 시마이
	//			break;
	//		}

	//		newRooms = DivideRoom(room);

	//		if (newRooms.Count != 0)
	//		{
	//			tempRooms.Remove(room); //복사한 리스트에서 제거하는거라서 문제 없음.
	//			Destroy(room.gameObject); //어차피 이거 넘어가면 쓸 일 없음.

	//			foreach (var newRoom in newRooms)
	//			{
	//				tempRooms.Add(newRoom);
	//			}
	//		}
	//		else
	//		{//더 나눌 경우 최소 사이즈보다 작아지거나 뭐 여타 다른 조건들로 인해 방이 안 만들어 진 경우


	//		}
	//	}

	//	rooms.Clear();
	//	rooms = null;
	//	rooms = tempRooms;

	//	newRooms.Clear();
	//	newRooms = null;

	//	++divideCount;

	//	System.Random rnd = new System.Random();
	//	var temp = rooms.OrderBy(a => rnd.Next()).ToList();
	//}
	public void Divieding()
	{
		int tryCount = divideTimes;

		if (dividedCount == divideTimes)
		{
			return;
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

	//private List<Room> DivideRoom_Old(Room room, int tryCount = 100)
	//{
	//	List<Room> tempRooms = new List<Room>();

	//	////0 = row (가로로 나누기)
	//	////1 = Col (세로로 나누기)
	//	int divideDir = room.transform.localScale.x / room.transform.localScale.y >= 1f ? 1 : 0;
	//	float divideRatio = Random.Range(minDivideRatio, maxDivideRatio);
	//	//자를 경우 사이즈부터 한번 체크해서 최소 크기보다 작은 경우 패스하고 다시 ㄱㄱ

	//	//=> 이거 어차피 만드는 부분에서도 특정 횟수 정해두고
	//	//그만큼 시도해도 안되면 안 나누는 방식일텐데
	//	//최소, 최대 맵 사이즈는 그냥 실제 맵 까는 (여백 버리는) 과정에서 체크하는게 나을듯
	//	//ㅇㅇ 그때 버리기?

	//	//0 = left / 1 = right
	//	//0 = top / 1 = bot
	//	Vector2[] newVertex = new Vector2[2];
	//	Room[] newRoom = new Room[2];
	//	CornerPos[] corner = new CornerPos[2];

	//	if (divideDir == 0)
	//	{//가로로 나눌 때 
	//		//Left
	//		newVertex[0] = new Vector2(room.cornerPos.LT.x,
	//								(room.cornerPos.LT.y - room.cornerPos.LB.y) * divideRatio + room.cornerPos.LB.y);
	//		//Right
	//		newVertex[1] = new Vector2(room.cornerPos.RT.x,
	//								(room.cornerPos.RT.y - room.cornerPos.RB.y) * divideRatio + room.cornerPos.RB.y);

	//		corner[0] = new CornerPos(room.cornerPos.LT, room.cornerPos.RT, newVertex[1], newVertex[0]);
	//		corner[1] = new CornerPos(newVertex[0], newVertex[1], room.cornerPos.RB, room.cornerPos.LB);

	//		for (int i = 0; i < 2; ++i)
	//		{
	//			newRoom[i] = CreateRoom(corner[i]);
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

	//		newRoom[0] = CreateRoom(new CornerPos(room.cornerPos.LT, newVertex[0], newVertex[1], room.cornerPos.LB));
	//		newRoom[1] = CreateRoom(new CornerPos(newVertex[0], room.cornerPos.RT, room.cornerPos.RB, newVertex[1]));
	//	}

	//	foreach (var item in newRoom)
	//	{
	//		tempRooms.Add(item);
	//	}

	//	return tempRooms;
	//}

	private Room CreateRoom(CornerPos corner, int treeIndex)
	{
		Vector2 size = new Vector2(Vector2.Distance(corner.LT, corner.RT), Vector2.Distance(corner.LT, corner.LB));
		Vector2 pos = new Vector2(corner.LT.x + size.x * 0.5f, corner.LT.y - size.y * 0.5f);

		GameObject newRoom = Instantiate(RoomPrefab);
		newRoom.name += $"({treeIndex})";
		newRoom.transform.SetParent(transform);
		newRoom.transform.position = pos;
		newRoom.transform.localScale = new Vector2(Mathf.FloorToInt(size.x), Mathf.FloorToInt(size.y)) /** 0.95f*/;

		Room roomScript = newRoom.GetComponent<Room>();
		roomScript.cornerPos.CalcCorner(newRoom.transform);

		newRoom.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f,1f), Random.Range(0f,1f) ,Random.Range(0f,1f), 0.5f);

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

		room.transform.position = new Vector2(x, y);
		room.transform.localScale = new Vector2(w, h);
	}

	//// 양옆(형제 노드)들의 방끼리 이어주기 
	//// 모든 depth 에서.
	public void ConnectingRooms()
	{
		int fullDepth = dividedCount;

		for (int i = fullDepth; i > 0; i--)
		{
			var list = roomTree.GetCertainDepthNodes(i);

			for (int k = 0; k < list.Count; k += 2)
			{
				Room olderRoom = null;
				Room youngerRoom = null;

				if (i == fullDepth)
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
				//Room olderRoom = list[k].Value;
				//Room youngerRoom = list[k + 1].Value;

				Vector2 olderRoomPos = olderRoom.transform.position;
				Vector2 youngerRoomPos = youngerRoom.transform.position;

				//왼쪽(형 노드)방의 세로(y값)을 기준으로 일단 선 하나
				Vector2 startPos = new Vector2(olderRoomPos.x, olderRoomPos.y);
				Vector2 endPos = new Vector2(youngerRoomPos.x, olderRoomPos.y);
				GameObject corridor1 = CreateCorridor(startPos, endPos);

				//오른쪽(동생 노드)방의 가로(x값)을 기준으로 일단 선 하나 더
				Vector2 startPos2 = new Vector2(youngerRoomPos.x, olderRoomPos.y);
				Vector2 endPos2 = new Vector2(youngerRoomPos.x, youngerRoomPos.y);
				GameObject corridor2 = CreateCorridor(startPos2, endPos2);

				corridors.Add(corridor1.GetComponent<Corridor>());
				corridors.Add(corridor2.GetComponent<Corridor>());

				olderRoom.linkedRooms.Add(youngerRoom);
				youngerRoom.linkedRooms.Add(olderRoom);
			}
		}
	}


	public void ConnectSiblingRoom()
	{
		int i = curCorridorDepth;

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
			//Room olderRoom = list[k].Value;
			//Room youngerRoom = list[k + 1].Value;

			Vector2 olderRoomPos = olderRoom.transform.position;
			Vector2 youngerRoomPos = youngerRoom.transform.position;

			//왼쪽(형 노드)방의 세로(y값)을 기준으로 일단 선 하나
			Vector2 startPos = new Vector2(olderRoomPos.x, olderRoomPos.y);
			Vector2 endPos = new Vector2(youngerRoomPos.x, olderRoomPos.y);
			GameObject corridor1 = CreateCorridor(startPos, endPos);

			//오른쪽(동생 노드)방의 가로(x값)을 기준으로 일단 선 하나 더
			Vector2 startPos2 = new Vector2(youngerRoomPos.x, olderRoomPos.y);
			Vector2 endPos2 = new Vector2(youngerRoomPos.x, youngerRoomPos.y);
			GameObject corridor2 = CreateCorridor(startPos2, endPos2);

			corridors.Add(corridor1.GetComponent<Corridor>());
			corridors.Add(corridor2.GetComponent<Corridor>());

			olderRoom.linkedRooms.Add(youngerRoom);
			youngerRoom.linkedRooms.Add(olderRoom);
		}

		curCorridorDepth--;
	}

	private Room[] GetNearChildrenNode(TreeNode<Room> leftNode, TreeNode<Room> rightNode)
	{
		if (leftNode.LeftNode == null | rightNode.LeftNode == null)
		{
			return null;
		}

		
		Room[] leftChildren  = { leftNode.LeftNode.Value, leftNode.RightNode.Value };
		Room[] rightChildren = { rightNode.LeftNode.Value, rightNode.RightNode.Value };

		float dist = float.MaxValue;
		int indexLeft = 0, indexRight= 0; 
		for (int i = 0; i < 2; ++i)
		{
			for (int k = 0; k < 2; ++k)
			{
				float tempDist = Vector2.Distance(leftChildren[i].transform.position, rightChildren[k].transform.position);

				if(tempDist < dist)
				{
					dist = tempDist;
					indexLeft = i;
					indexRight = k;
				}
			}
		}

		return new Room[2] { leftChildren[indexLeft], rightChildren[indexRight] };
	}

	private GameObject CreateCorridor(Vector2 startPos, Vector2 endPos)
	{
		//일단은 따로 만들고 나중에 하나로 합치기 
		float w = Mathf.Abs(startPos.x -  endPos.x);
		float h = Mathf.Abs(startPos.y - endPos.y);
		Vector2 centerPos = (startPos + endPos) * 0.5f;

		GameObject corridorObj = Instantiate(CorridorPrefab);

		corridorObj.transform.position = centerPos;
		corridorObj.transform.localScale = new Vector2(Mathf.Clamp(w,1,w), Mathf.Clamp(h,1,h));

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

		foreach (var item in corridors)
		{
			Destroy(item.gameObject);
		}
		corridors.Clear();
		corridors = new List<Corridor>();
			
		CreateInitDungeon();
	}


	private void CreateInitDungeon()
	{
		GameObject roomObj = Instantiate(RoomPrefab);
		roomObj.transform.SetParent(transform);
		roomObj.transform.localScale = dungeonSize;
		Room room = roomObj.GetComponent<Room>();
		room.cornerPos.CalcCorner(roomObj.transform);
		roomObj.name += "(0)";
		//rooms.Add(roomScript);
		roomTree = new Tree<Room>(room);

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
		
	}


	private void OnDestroy()
	{
		roomTree = null;
	}

}
