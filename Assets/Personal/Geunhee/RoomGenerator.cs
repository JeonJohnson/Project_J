using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Structs;
using UnityEditor.Rendering;
using Unity.Collections.LowLevel.Unsafe;


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

public class RoomGenerator : MonoBehaviour
{
	//for Test
	[Header("Setting Vals")]
	public Vector2 dungeonSize;
	public GameObject RoomPrefab;
    public int maxRoomCount;
	[Range(0f, 1f)]
	public float minDivideRatio, maxDivideRatio;
	public Vector2 minRoomSize, maxRoomSize;


	[Space(10f)]
	[Header("Display Vals")]
	public int divideCount;
	public List<Room> rooms;

	public void Divide()
	{
		if (rooms.Count >= maxRoomCount)
		{
			return;
		}

		
		List<Room> tempRooms = rooms.ToList(); 
		List<Room> newRooms = new List<Room>(); 

		foreach (var room in rooms)
		{
			if (tempRooms.Count >= maxRoomCount)
			{ //방 하나 나눴는데 만약 개수 넘어가는 경우 그냥 시마이
				break;
			}

			newRooms = DivideRoom(room);

			if (newRooms.Count != 0)
			{
				tempRooms.Remove(room); //복사한 리스트에서 제거하는거라서 문제 없음.
				Destroy(room.gameObject); //어차피 이거 넘어가면 쓸 일 없음.

				foreach (var newRoom in newRooms)
				{
					tempRooms.Add(newRoom);
				}
			}
			else
			{//더 나눌 경우 최소 사이즈보다 작아지거나 뭐 여타 다른 조건들로 인해 방이 안 만들어 진 경우


			}
		}

		rooms.Clear();
		rooms = null;
		rooms = tempRooms;

		newRooms.Clear();
		newRooms = null;

		++divideCount;

		System.Random rnd = new System.Random();
		var temp = rooms.OrderBy(a => rnd.Next()).ToList();
	}

	private List<Room> DivideRoom(Room room, int tryCount = 100)
	{
		List<Room> tempRooms = new List<Room>();

		////0 = row (가로로 나누기)
		////1 = Col (세로로 나누기)
		int divideDir = room.transform.localScale.x / room.transform.localScale.y >= 1f ? 1 : 0;
		float divideRatio = Random.Range(minDivideRatio, maxDivideRatio);
		//자를 경우 사이즈 부터 한번 체크해서 최소 크기보다 작은 경우 패스하고 다시 ㄱㄱ

		//0 = left / 1 = right
		Vector2[] newVertex = new Vector2[2];

		//0 = top / 1 = bot
		Room[] newRoom = new Room[2];	

		if (divideDir == 0)
		{//가로로 나눌 때 
			//Left
			newVertex[0] = new Vector2(room.cornerPos.LT.x,
									(room.cornerPos.LT.y - room.cornerPos.LB.y) * divideRatio + room.cornerPos.LB.y);

			//Right
			newVertex[1] = new Vector2(room.cornerPos.RT.x,
									(room.cornerPos.RT.y - room.cornerPos.RB.y) * divideRatio + room.cornerPos.RB.y);

			//Top
			newRoom[0] = CreateRoom(new CornerPos(room.cornerPos.LT, room.cornerPos.RT, newVertex[1], newVertex[0]));
			//Bot
			newRoom[1] = CreateRoom(new CornerPos(newVertex[0], newVertex[1], room.cornerPos.RB, room.cornerPos.LB));
		}
		else
		{
			newVertex[0] = 
					new Vector2((room.cornerPos.RT.x - room.cornerPos.LT.x) * divideRatio + room.cornerPos.LT.x,
					room.cornerPos.LT.y);
			newVertex[1] = 
					new Vector2((room.cornerPos.RB.x - room.cornerPos.LB.x) * divideRatio + room.cornerPos.LB.x,
					room.cornerPos.LB.y);

			newRoom[0] = CreateRoom(new CornerPos(room.cornerPos.LT, newVertex[0], newVertex[1], room.cornerPos.LB));
			newRoom[1] = CreateRoom(new CornerPos(newVertex[0], room.cornerPos.RT, room.cornerPos.RB, newVertex[1]));
		}


		foreach (var item in newRoom)
		{
			tempRooms.Add(item);
		}

		return tempRooms;
	}

	private Room CreateRoom(CornerPos corner/*, int tempRoomsCount*/)
	{
		Vector2 size = new Vector2(Vector2.Distance(corner.LT, corner.RT), Vector2.Distance(corner.LT, corner.LB));
		Vector2 pos = new Vector2(corner.LT.x + size.x * 0.5f, corner.LT.y - size.y * 0.5f);

		GameObject newRoom = Instantiate(RoomPrefab);
		newRoom.name += Random.Range(-5, 6).ToString();
		newRoom.transform.SetParent(transform);
		newRoom.transform.position = pos;
		newRoom.transform.localScale = new Vector2(Mathf.FloorToInt(size.x), Mathf.FloorToInt(size.y));

		Room roomScript = newRoom.GetComponent<Room>();
		roomScript.cornerPos.CalcCorner(newRoom.transform);

		newRoom.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f,1f), Random.Range(0f,1f) ,Random.Range(0f,1f));

		return roomScript;
	}


	private void CreateInitDungeon()
	{
		GameObject firstRoom = Instantiate(RoomPrefab);
		firstRoom.transform.SetParent(transform);
		firstRoom.transform.localScale = dungeonSize;
		Room roomScript = firstRoom.GetComponent<Room>();
		roomScript.cornerPos.CalcCorner(firstRoom.transform);


		rooms.Add(roomScript);
	}

	private void Awake()
	{
		rooms = new List<Room>();
		divideCount = 0;

		CreateInitDungeon();
	}


	private void Start()
	{
		
	}
	

	private void Update()
	{
		
	}




}
