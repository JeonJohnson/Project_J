using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using UnityEngine.UIElements;

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

public class DungeonGenerator_Isaac : MonoBehaviour
{

	public static List<Vector2Int>[] PivotByRoomShape =
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
		//   Four, // 2*2

		new List<Vector2Int>() { Vector2Int.zero}, //One
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right }, // - 
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up}, // |
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up }, //ㄴ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new (1,1)}, //역ㄴ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, new(1,-1)},//ㄱ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.up, new(1,1) }, //역ㄱ
		new List<Vector2Int> { Vector2Int.zero, Vector2Int.right, Vector2Int.up, new(1,1) }, //네모네모
	};

    public static int[] maxDoorCount =
    {
        4,  
        6,  6, 
        8, 8, 8, 8,
        8
    };

    
	//public GameObject GridPrefab;

	//public SpriteGrid areaGrid;
	public Vector2Int areaSize;
    public Vector2 pivotPos;
    private Vector2 originPos;
    
    public Vector2Int roomCountRange;
    [ReadOnly]
    public int curRoomCount;

    public SerializedDictionary<RoomShape_Isaac, int> countPerRoomType;


    public Vector2Int defaultRoomSize; //1*1짜리 방 사이즈
    private Vector2 roomHalfSize;
    public Room_Isaac[,] rooms;

    public void Setup()
    {
        roomHalfSize = new Vector2(defaultRoomSize.x * 0.5f, defaultRoomSize.y * 0.5f);

        originPos = pivotPos + roomHalfSize;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
