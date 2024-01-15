using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using UnityEngine.UIElements;
using UnityEngine.Analytics;
using System.Net.Http.Headers;
using System.Linq.Expressions;

public enum RoomShape_Isaac
{ 
    One, //1*1
    Two_Ver, // ─
    Two_Hor, // │
    Nieun, // └
    Nieun_Mirror, //┘
	Giyeok, // ┐
	Giyeok_Mirror, //┌
    Four, // 2*2
    End
}

public static class RoomIndexes
{
	public static List<Vector2Int>[] one = { new List<Vector2Int> { Vector2Int.zero } };

	public static List<Vector2Int>[] two_ver =
	{
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.left }
	};
	public static List<Vector2Int>[] two_hor =
	{
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.down }
	};
		

	public static List<Vector2Int>[] Nieun =
	{
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up },
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, new Vector2Int(-1, 1) },
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.down, new(1, -1) }
	};
	public static List<Vector2Int>[] Nieun_M =
	{
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new(1, 1) },
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, Vector2Int.up },
	new List<Vector2Int> { Vector2Int.zero, Vector2Int.down, new(-1, -1) }
	};

	public static List<Vector2Int>[] Giyeok =
	{ 
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new(1, -1) },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, Vector2Int.down },
		new List<Vector2Int> { Vector2Int.zero, new(-1, 1), Vector2Int.up }
	};
	public static List<Vector2Int>[] Giyeok_M =
	{ 
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.down },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, Vector2Int.down },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up, new(1, 1) }
	};

	public static List<Vector2Int>[] Four =
	{
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.down, new (1,-1) },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, new(-1,-1), Vector2Int.down },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up, new Vector2Int(1,1) },
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.left, new(-1,-1), Vector2Int.up }
	}; 
}


public class DungeonGenerator_Isaac : MonoBehaviour
{

	public GameObject TestTilePrefab;
	public List<TestTile_Isaac> testTiles = new List<TestTile_Isaac>();

    public List<Vector2Int>[] PivotByRoomShape =
    {
		//아이작 방 생성에서
		//방을 랜덤 형태로 정할 때
		//여러 형태 내에서 어떠한 Cell을 기준으로 방을 만들 것인가
        //랜덤으로 정하기 위해서
		//좌우(중 왼쪽), 상하(중 아래) 순서로 우선순위.

		//	   One, //1*1
		//   Two_Ver, // ─
		//   Two_Hor, // │
		//   Nieun, // └
		//   Nieun_Mirror, //┘
		//	Giyeok, // ┐
		//	Giyeok_Mirror, //┌
		//   Four, // 2*2`

		new List<Vector2Int>() { Vector2Int.zero}, //One
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right }, // - 
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up}, // |
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up }, //ㄴ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new (1,1)}, //역ㄴ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new(1,-1)},//ㄱ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up, new(1,1) }, //역ㄱ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up, new(1,1) } //네모네모
	};

	public List<Vector2Int>[][] IndexesByRoomShape =
	{
		RoomIndexes.one,
		RoomIndexes.two_ver,
		RoomIndexes.two_hor,
		RoomIndexes.Nieun,
		RoomIndexes.Nieun_M,
		RoomIndexes.Giyeok,
		RoomIndexes.Giyeok_M,
		RoomIndexes.Four
	};

	public Vector2Int[] randomDir = 
	{
		Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up
    };

public static int[] maxDoorCount =
    {
        4,  //0
        6,  6,  //1,2
        8, 8, 8, 8, //3,4,5,6
        8 //7
    };

    
	//public GameObject GridPrefab;

	//public SpriteGrid areaGrid;
	public Vector2Int areaSize;
    public Vector2 pivotPos;
    private Vector2 originPos;
    
    public Vector2Int roomMinMaxCount;
    [ReadOnly]
    public int curRoomCount;

    //public SerializedDictionary<RoomShape_Isaac, int> countPerRoomType;


    public Vector2Int defaultRoomSize; //1*1짜리 방 사이즈
    private Vector2 roomHalfSize;
    public Room_Isaac[,] rooms;
    public Room_Isaac GetRoom(Vector2Int index)
    {
        return rooms[index.x, index.y];
    }


	public List<Room_Isaac> recentRooms;


	//1. 랜덤 위치에 첫방 만들기
	//2. 방문 랜덤 개수로 만들기
	//3. 해당 방향으로 넘어가서 랜덤 형태의 방만들기
	//3-1. 어디를 중점으로 둘지 판단하기
	//3-2. 
	public void Setup()
    {
        roomHalfSize = new Vector2(defaultRoomSize.x * 0.5f, defaultRoomSize.y * 0.5f);

        originPos = pivotPos + roomHalfSize;

		recentRooms = new List<Room_Isaac>();

        rooms = new Room_Isaac[areaSize.x, areaSize.y];
        for (int y = 0; y < areaSize.y; ++y)
        {
            for (int x = 0; x < areaSize.x; ++x)
            {
                rooms[x, y] = null;
            }
        }

    }

    public void SetStartRoom()
    { 
        Vector2Int randIndex = new Vector2Int(Random.Range(0,areaSize.x), Random.Range(0,areaSize.y));
        
        Room_Isaac startRoom = new Room_Isaac(RoomShape_Isaac.One, randIndex);
		rooms[randIndex.x, randIndex.y] = startRoom;
		curRoomCount = 1;
		CreateTestTile(randIndex, curRoomCount);
		SetRandomDoor(startRoom);

		recentRooms.Add(startRoom);
	}

	public void CreateAnotherRoom()
	{
		List<Room_Isaac> newRooms = new List<Room_Isaac>();
		foreach (var item in recentRooms)
		{
			var newRoom = CreateRoom(item);
			if (newRoom != null)
			{
				newRooms.Add(newRoom);
				SetRandomDoor(newRoom);
			}
			
		}

		if (newRooms.Count != 0)
		{
			recentRooms.Clear();
			recentRooms = newRooms;
		}
	}


	private Room_Isaac CreateRoom(Room_Isaac room)
	{
		foreach (var pair in room.doors)
		{
			foreach (var dir in pair.Value)
			{
				Vector2Int targetIndex = pair.Key + dir;

				//1. 모양 랜덤으로 정하기
				List<int> shapeRandomList = new List<int>();
				int shapeRandom;
				Room_Isaac newRoom;
				//1. 탈출조건 1)특정 모양으로 만들 수 있을 때
				//1. 탈출조건 2)모든 모양으로 만들 수 없을 때
				while (true)
				{
					shapeRandom = GetDontOverlapRandom(0, (int)RoomShape_Isaac.End, ref shapeRandomList);

					if (shapeRandom == int.MinValue)
					{ break; }

					//2. 여러 모양에서 기준을 어디에 두느냐에 따른 인덱스들
					var indexSet = IndexesByRoomShape[shapeRandom];

					List<int> indexSetRandomList = new List<int>();
					int indexSetRandom;
					//2. 다음 루프 조건 1) 해당 인덱스에 다른 방이 있는 경우,
					//2. 다름 루프 조건 2) 인덱스를 벗어나는 경우
					//2. 탈출 조건 1) 둘다 해당 안되서 설치가 가능한 경우
					//2. 탈출 조건 2) 모든 경우에서 불가능한 경우 다음 모양으로 ㄱㄱ
					while (true)
					{
					RETRY_INDEXSET:
						indexSetRandom = GetDontOverlapRandom(0, indexSet.Length, ref indexSetRandomList);

						if (indexSetRandom == int.MinValue)
						{
							break;
						}

						var indexes = indexSet[indexSetRandom];

						for(int n = 0; n <indexes.Count; ++n)
						{
							indexes[n] += targetIndex;
							if ((indexes[n].x >= rooms.GetLength(0) || indexes[n].y >= rooms.GetLength(1))
								|| (indexes[n].x < 0 || indexes[n].y < 0)
								|| (GetRoom(indexes[n]) != null))
							{
								goto RETRY_INDEXSET;
							}
						}

						newRoom = new Room_Isaac((RoomShape_Isaac)shapeRandom, indexes.ToArray());
						++curRoomCount;

						foreach (var roomIndex in newRoom.indexes)
						{
							rooms[roomIndex.x, roomIndex.y] = newRoom;
							CreateTestTile(roomIndex, curRoomCount);
						}
						return newRoom;
					}
				}
			}
		}

		return null;
	}

	private TestTile_Isaac CreateTestTile(Vector2Int index,int roomNum)
	{
		GameObject newObj = Instantiate(TestTilePrefab, GetPos(index), Quaternion.identity, transform);
		TestTile_Isaac script = newObj.GetComponent<TestTile_Isaac>();

		script.text.text = $"{roomNum}";

		testTiles.Add(script);

		return script;
	}

	//private bool IsSuitableIndex(Vector2Int index)
	//{
		


	//}
	/// <summary>
	/// max is exclusive
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max">  </param>
	/// <param name="arr"></param>
	//private int GetDontOverlapRandom(int minInclude, int maxExclude, ref int[] arr)
	//   {
	//       int val = int.MinValue;

	//       while (true)
	//       {
	//           if (arr.Length >= maxExclude - minInclude)
	//           {
	//               break;
	//           }

	//           int rand = Random.Range(minInclude, maxExclude);

	//           foreach (var item in arr)
	//           {
	//               if (item == rand)
	//               {
	//                   break;
	//               }
	//           }
	//           val = rand;
	//       }

	//	return val;
	//}

	private int GetDontOverlapRandom(int minInclude, int maxExclude, ref List<int> arr)
	{
		int val = int.MinValue;
		if (arr.Count >= maxExclude - minInclude)
		{
			return val;
		}

	RETRY:
		int rand = Random.Range(minInclude, maxExclude);

		foreach (var item in arr)
		{
			if (item == rand)
			{
				goto RETRY;
			}
		}

		val = rand;
		arr.Add(val);
		return val;
	}

	private void SetRandomDoor(Room_Isaac room)
    {
		//순서대로 처리하지말고
		//그냥 모든 곳 순차적으로 돌아보면서
		//문 설치가능한 곳들 모아뒀다가
		//랜덤 개수만큼 가져가기 ㅋㅋ
		//c'ex ㅋㅋ
		//어차피 원하는 개수만큼 설치 하지 못해도
		//더 설치 할 수 있는지 아닌지 확인하려면 다 돌아야함 ㅋㅋ
		//무적권 다 돌아봐야한다는 단점이 있지만은
		//개발 빠를듯 ㅋㅋ

		//for (int i = 0; i < room.indexes.Length; ++i)
		//{ 



		//}

		//1. 해당 방 자체의 문 개수 정해주기
		int maxRoomDoorCount = Random.Range(1, maxDoorCount[(int)room.shape] + 1);
		int leftRoomDoorCount = maxRoomDoorCount;
		//1. 탈출 조건 1) 원하는 문의 개수를 다 채운 경우

		//2. 해당 방 내에서 테스트할 Cell 정해주기
		List<int> cellIndexList = new List<int>();
		int cellIndex;
		cellIndex = GetDontOverlapRandom(0, room.indexes.Length, ref cellIndexList);
		//2. 탈출 조건 2) 모든 Cell을 둘러본 경우

		while (!(leftRoomDoorCount == 0 || cellIndex == int.MinValue)) //탈출 조건 1|2
		{
			//cellIndex = GetDontOverlapRandom(0, room.indexes.Length, ref cellIndexList);
			//3. 현재 Cell에서 설치할 문 개수.
			int cellDoorCount = Random.Range(0, leftRoomDoorCount + 1);
			int curCellDoorCount = 0;
			List<int> cellDoorDirList = new List<int>();
			int cellDoorDir;
			do
			{ //3. 탈출 조건
			  //3-a. 현재 Cell에서 원하는 개수만큼 설치하거나
			  //3-b. 다 돌려 봤을때 더 설치 못하는 경우
				cellDoorDir = GetDontOverlapRandom(0, randomDir.Length, ref cellDoorDirList);

				if (cellDoorDir == int.MinValue)
				{
					//3-b. 다 돌려 본 경우
					break;
				}

				var randDir = randomDir[cellDoorDir];
				Vector2Int targetIndex = room.indexes[cellIndex] + randDir;

				if ((targetIndex.x >= rooms.GetLength(0) || targetIndex.y >= rooms.GetLength(1))
					|| (targetIndex.x < 0 || targetIndex.y < 0)
					|| (GetRoom(targetIndex) == room))
				{
					continue;
				}
				else
				{
					room.doors[room.indexes[cellIndex]].Add(randDir);
					++curCellDoorCount;
				}
			}
			while (curCellDoorCount < cellDoorCount);//3-a.원하는 개수만큼 설치 한 경우
			leftRoomDoorCount -= curCellDoorCount;
		}
	}

	private void SetRandomShape(Room_Isaac room)
    { 
		
    }

    //private Room_Isaac CreateRandomRoom(Vector2Int index)
    //{ 
    
    //}

    //private Vector2Int[] GetIndexes(RoomShape_Isaac shape, Vector2Int index)
    //{ 
    
    //}


	public Vector2 GetPos(Vector2Int index)
    {
        if (index.x < 0 | index.y < 0 | index.x >= areaSize.x | index.y >= areaSize.y)
        {
            return new Vector2(float.MinValue, float.MinValue);
        }

        return originPos + new Vector2(index.x * defaultRoomSize.x , index.y * defaultRoomSize.y);
    }

    public Vector2Int GetIndex(Vector2 pos)
    {
        Vector2 offsetPos = pos - originPos;
        Vector2 index = new Vector2(offsetPos.x / defaultRoomSize.x, offsetPos.y / defaultRoomSize.y);

        if (index.x < 0f | index.y < 0f | index.x >= areaSize.x | index.y >= areaSize.y)
        {
            return new Vector2Int(int.MinValue, int.MinValue);
        }
        
        return new Vector2Int((int)index.x, (int)index.y);
    }


    // Start is called before the first frame update
    void Start()
    {
		Setup();
		SetStartRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
