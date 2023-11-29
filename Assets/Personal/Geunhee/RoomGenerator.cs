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
    public int minRoomCount, maxRoomCount;
	[Range(0f, 1f)]
	public float minDivideRatio, maxDivideRatio;


	[Space(10f)]
	[Header("Display Vals")]
    //public int curRoomCount;
	public int divideCount;
	public List<Room> rooms;
	public void Divide()
	{
		if (rooms.Count >= maxRoomCount)
		{
			return;
		}

		List<Room> newRooms = new List<Room>();


		foreach (var room in rooms)
		{
			var tempRooms = DivideRoom(room);

			foreach (var tempRoom in tempRooms)
			{
				newRooms.Add(tempRoom);

				if (newRooms.Count >= maxRoomCount)
				{
					break;
				}
			}
			Destroy(room.gameObject);

			if (newRooms.Count >= maxRoomCount)
			{
				break;
			}
		}
		rooms.Clear();
		rooms = newRooms;

		System.Random rnd = new System.Random();
		var temp = rooms.OrderBy(a => rnd.Next()).ToList();
		++divideCount;
	}

	private List<Room> DivideRoom(Room room)
	{
		List<Room> tempRooms = new List<Room>();

		////0 = row (가로로 나누기)
		////1 = Col (세로로 나누기)
		float aspectRatio = room.transform.localScale.x / room.transform.localScale.y;
		int divideDir = aspectRatio >= 1f ? 1 : 0;
		float divideRatio = Random.Range(minDivideRatio, maxDivideRatio);
		Debug.Log(divideRatio);

		//자를 경우 사이즈 부터 한번 체크해서 최소 크기보다 작은 경우 패스하고 다시 ㄱㄱ


		//테스트용
		if (divideDir == 0)
		{//가로로 나눌 때 
			Vector2 newRightVertex =
					new Vector2(room.cornerPos.RT.x,
					(room.cornerPos.RT.y - room.cornerPos.RB.y) * divideRatio + room.cornerPos.RB.y);
			Vector2 newLeftVertex =
					new Vector2(room.cornerPos.LT.x,
					(room.cornerPos.LT.y - room.cornerPos.LB.y) * divideRatio +room.cornerPos.LB.y);

			Room up = CreateRoom(new CornerPos(room.cornerPos.LT, room.cornerPos.RT, newRightVertex, newLeftVertex));
			Room down = CreateRoom(new CornerPos(newLeftVertex, newRightVertex, room.cornerPos.RB, room.cornerPos.LB));

			tempRooms.Add(up);
			tempRooms.Add(down);
		}
		else
		{
			Vector2 newTopVertex =
					new Vector2((room.cornerPos.RT.x - room.cornerPos.LT.x) * divideRatio + room.cornerPos.LT.x,
					room.cornerPos.LT.y);
			Vector2 newBotVertex =
					new Vector2((room.cornerPos.RB.x - room.cornerPos.LB.x) * divideRatio + room.cornerPos.LB.x,
					room.cornerPos.LB.y);

			Room left = CreateRoom(new CornerPos(room.cornerPos.LT, newTopVertex, newBotVertex, room.cornerPos.LB));
			Room right = CreateRoom(new CornerPos(newTopVertex, room.cornerPos.RT, room.cornerPos.RB, newBotVertex));

			tempRooms.Add(left);
			tempRooms.Add(right);
		}


		return tempRooms;
	}

	private Room CreateRoom(CornerPos corner)
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


	private void Awake()
	{
		rooms = new List<Room>();

		//GameObject firstRoom = new GameObject();
		//firstRoom.transform.SetParent(transform);
		//firstRoom.transform.localScale = dungeonSize;

		//firstRoom.AddComponent<SpriteRenderer>();
		//rooms.Add(firstRoom.AddComponent<Room>());

		GameObject firstRoom = Instantiate(RoomPrefab);
		firstRoom.transform.SetParent(transform);
		firstRoom.transform.localScale = dungeonSize;
		Room roomScript = firstRoom.GetComponent<Room>();
		roomScript.cornerPos.CalcCorner(firstRoom.transform);
		

		rooms.Add(roomScript);
	}


	private void Start()
	{
		
	}
	

	private void Update()
	{
		
	}




}
