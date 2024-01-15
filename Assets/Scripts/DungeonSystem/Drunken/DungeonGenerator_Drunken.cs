using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Tilemaps;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;
using MoreMountains.Tools;

public enum tileGridState
{ 
    None,
    Ground,
    Wall
}

public enum ExplorerDir
{
    Up,
    Right,
    Down,
    Left,
    None
}

public enum TilemapLayer
{ 
    Ground,
    Prop,
    Shadow,
    Cliff,
    Wall,
    End
}

public struct Explorer
{
    public Explorer(Vector2Int _index)
    {
        index = _index;
        dir = Vector2Int.zero;
	}
    
    public Vector2Int index;
    public Vector2Int dir;
}

public class DungeonGenerator_Drunken : MonoBehaviour
{
	#region ForTest
	public Transform areaTr;
	public Camera cam;
	public GameObject PortalPrefab;
    public List<GameObject> portals;
	#endregion

	public GameObject tilemapOriginPrefab;
	public SerializedDictionary<TilemapLayer, List<TileBase>> TileResource;
    
	[Space(10f)]
	[ReadOnly]
    public Tilemap[] recentTilemaps = new Tilemap[(int)TilemapLayer.End];
    [ReadOnly]
    public int[] recentTilesCount = new int[(int)TilemapLayer.End];
	[ReadOnly]
	public List<Room_Drunken> rooms;
	[Space(7.5f)]
    [ReadOnly]
    public int tileSize = 1;
	[ReadOnly]
	public Vector2 pivotPos;
    [ReadOnly]
    public Vector2 originPos;
    [ReadOnly]
    public Vector2 centerPos;
    [ReadOnly]
    public Vector2Int gridLength;

	//Internal Varis
	Vector2Int centerIndex;
	private Vector2 areaHalfSize;
	private float tileHalfSize;

	private tileGridState[,] tileGrid;

	private List<Explorer> explorers;
	private int destoryCount;
	private int respawnCount;
	//Internal Varis


	#region Generation option
	[Space(10f)]
    [Header("Generation Option")]
	[Range(1, 10)]
	public int createRoomCount;
	public Vector2Int areaSize;

	//해당 비율만큼 바닥 만들었으면 끝.
	[Range(0.25f, 0.75f)]
	public float areaFillRatio;
	//[Tooltip("About wall is 1*1 of the grounds.")]
	[Range(10000, 9999999)]
	public int maxTryCount;
	[ReadOnly]
	public int curTryCount;

    [Range(0.1f, 0.75f)]
    public float wallTileOffset;

    [Space(7.5f)]
	[SerializeField]
    private NewExplorerPos ExplorerSpawnOption;

	enum NewExplorerPos { center, prePos };
	[Space(7.5f)]
	public int startExplorerCount;
    public Vector2Int ExplorerCountRange;
	[ReadOnly]
	public int curExplorerCount;

	[Space(7.5f)]
    [Range(0f, 1f)]
    public float destroyPercent;
    public Vector2Int destroyCountRange; //한번에 최고 몇명까지 해고할지

    [Space(7.5f)]
    [Range(0f, 1f)]
    public float respawnPercent;
    public Vector2Int respawnCountRange;
    
    [Space(7.5f)]
    [Range(0f, 1f)]
    public float newDirPercent;
	#endregion


	public void Setup()
    {
		#region AreaSetting
		gridLength.x = Mathf.RoundToInt(areaSize.x / tileSize);
        gridLength.y = Mathf.RoundToInt(areaSize.y / tileSize);
        tileGrid = new tileGridState[gridLength.x, gridLength.y];

        areaHalfSize = Funcs.ToV2(gridLength) * 0.5f;
        tileHalfSize = tileSize * 0.5f;

        centerPos = pivotPos + new Vector2(gridLength.x * 0.5f, gridLength.y * 0.5f);
		centerIndex = new Vector2Int((int)(gridLength.x * 0.5f), (int)(gridLength.y * 0.5f));
        originPos = new Vector2(pivotPos.x + tileHalfSize, pivotPos.y + tileHalfSize);

        if (areaTr != null)
        {
            areaTr.localScale = new Vector3(gridLength.x, gridLength.y);
            areaTr.localPosition = new Vector3(gridLength.x * 0.5f, gridLength.y * 0.5f);
        }
		#endregion

		#region camSetting
		cam ??= Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
            cam.orthographicSize = gridLength.x <= gridLength.y ? gridLength.y * 0.5f : gridLength.x * 0.5f;
            cam.orthographicSize += 1;
        }
		#endregion
		
        explorers = new List<Explorer>();
        for (int i = 0; i < startExplorerCount; i++)
        {
            explorers.Add(new Explorer(centerIndex));
        }

        rooms ??= new List<Room_Drunken>();
        
        GameObject obj = Instantiate(tilemapOriginPrefab);
        Room_Drunken roomScript = obj.GetComponent<Room_Drunken>();
        roomScript.centerPos = GetPos(centerIndex);
        rooms.Add(roomScript);

        for (int i = 0; i < recentTilemaps.Length; ++i)
        {
            TilemapLayer layer = (TilemapLayer)i;
            roomScript.tilemaps.TryGetValue(layer,out recentTilemaps[i]);
            recentTilesCount[i] = 0;
        }
	}


    public void CreateRoom()
    {
        Setup();
        CreateGround();
        CreateWall();
        //CreateCliff();
        var pos = recentTilemaps[(int)TilemapLayer.Wall].transform.position;
        pos.y += wallTileOffset;
        recentTilemaps[(int)TilemapLayer.Wall].transform.position = pos;
		CreateTestPortal();
    }

    public void GotoGameScene()
    {
        for (int i = 0; i < rooms.Count; ++i)
        {
            DontDestroyOnLoad(rooms[i].gameObject);
        }

		StageManager.Instance.rooms.AddRange(rooms);
        SceneManager.LoadScene(3);
    }

    public void CreateGround()
    {
		curTryCount = 0;
		while (maxTryCount > curTryCount)
		{
			if (MoveExplorer())
			{
				++curTryCount;
			}
			else
			{
				break;
			}
		}
	}
    public bool MoveExplorer()
    { //Move Once

        //1. Checking ground Count Ratio 
        if (areaFillRatio <= (float)recentTilesCount[(int)TilemapLayer.Ground] / (float)tileGrid.Length)
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
            temp.index.y = Mathf.Clamp(temp.index.y , 1, gridLength.y - 2);

			CreateTile(temp.index, tileGridState.Ground);
            explorers[i] = temp;
        }

        return true;
    }

	private void CreateWall()
	{
		for (int x = 0; x < gridLength.x; ++x)
		{
			for (int y = 0; y < gridLength.y; ++y)
			{
				List<Vector2Int> createIndex = new List<Vector2Int>();

				Vector2Int index = new Vector2Int(x, y);

				if (tileGrid[x, y] == tileGridState.None)
				{
					createIndex.Add(index);
				}

				foreach (var item in createIndex)
				{
					Vector2 pos = GetPos(item);
					CreateTile(pos, tileGridState.Wall);
                    SetTile(index, TilemapLayer.Cliff);
					//SetTile(index, TilemapLayer.Shadow);
				}
			}
		}
	}

	//private void CreateCliff()
 //   {
 //       List<Vector2Int> createIndex = new List<Vector2Int>();

 //       for (int y = 1; y < gridLength.y - 1; ++y)
 //       {
 //           for (int x = 0; x < gridLength.x - 1; ++x)
 //           {
 //               if (tileGrid[x, y] == tileGridState.Wall)
 //               {
 //                   Vector2Int index = new Vector2Int(x, y);

	//				if (tileGrid[x, y - 1] == tileGridState.Ground)
	//				{
	//					Vector2Int newIndex = index;
	//					newIndex.y -= 1;
	//					createIndex.Add(newIndex);
	//				}

	//			}
 //           }
 //       }

 //       foreach (var item in createIndex)
 //       {
 //           Vector2 pos = GetPos(item);
 //           wallTilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), CliffTiles[0]);
 //           wallTilesCount++;
 //       }
 //   }


    private void CreateShadow()
    { 
    
    }

    private void CreateTestPortal()
    {
        List<Vector2Int> groundIndex = new List<Vector2Int> ();
        //Set Player Spawn Pos & next Room Portal 
        for (int x = 0; x < gridLength.x; ++x)
        {
            for (int y = 0; y < gridLength.y; ++y)
            {
                if (tileGrid[x, y] == tileGridState.Ground && centerIndex != new Vector2Int(x,y))
                {
                    groundIndex.Add(new(x, y));
                }
            }
        }

        var index = groundIndex[Random.Range(0, groundIndex.Count)];

        Instantiate(PortalPrefab,GetPos(index), Quaternion.identity, rooms[rooms.Count-1].transform);
    }


    private Vector2Int GetRandomDir()
    {
        return GetDir(Random.Range((int)ExplorerDir.Up, (int)ExplorerDir.None + 1));
    }
	private Vector2Int GetDir(int num)
	{
        Vector2Int val = Vector2Int.zero;

		switch ((ExplorerDir)num)
		{
			case ExplorerDir.Up:
                val =  Vector2Int.up;
                break;
			case ExplorerDir.Right:
                val =  Vector2Int.right;
                break;
			case ExplorerDir.Down:
                val =  Vector2Int.down;
                break;
			case ExplorerDir.Left:
                val =  Vector2Int.left;
                break;
			default:
                val =  Vector2Int.zero;
                break;
		}
        val *= tileSize;
        return val;
	}

    private Vector2 GetPos(Vector2Int index)
    {
        if (index.x < 0 | index.y < 0 | index.x >= gridLength.x | index.y >= gridLength.y)
        {
            return new Vector2(float.MinValue, float.MinValue);
        }

        Vector2 pos = pivotPos + new Vector2(index.x * tileSize + tileHalfSize, index.y * tileSize + tileHalfSize);

        return pos;
    }

	private Vector2Int GetIndex(Vector2 pos)
    {
        Vector2 index = (pos - originPos) * tileSize;

        if (index.x < 0f | index.y < 0f | index.x >= gridLength.x | index.y >= gridLength.y)
        {
            return new Vector2Int(int.MinValue, int.MinValue);
        }

		return new Vector2Int((int)index.x, (int)index.y);
	}

	private void CreateTile(Vector2Int index, tileGridState state)
	{
		CreateTile(GetPos(index), state);
	}

	private void CreateTile(Vector2 pos, tileGridState state)
	{
		var index = GetIndex(pos);

		if (tileGrid[index.x, index.y] != state)
		{
			tileGrid[index.x, index.y] = state;

            int layer = 0;
            TileBase tile = null;
            Vector3Int newPos = new ((int)pos.x, (int)pos.y, 0);
			switch (state)
			{
                
				case tileGridState.Ground:
					{
                        layer = (int)TilemapLayer.Ground;
						//groundTilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y,0), GroundTiles[0]);
						//groundTilesCount++;
					}
					break;
				case tileGridState.Wall:
					{
						layer = (int)TilemapLayer.Wall;
						//wallTilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), WallTiles[0]);
						//wallTilesCount++;
					}
					break;
			}
			tile = TileResource[(TilemapLayer)layer][0];
			recentTilemaps[layer].SetTile(newPos, tile);
            ++recentTilesCount[layer];
		}
	}


	private void SetTile(Vector2Int index, TilemapLayer tileLayer)
	{
        Vector2 pos = GetPos(index);
		Vector3Int newPos = new((int)pos.x, (int)pos.y, 0);
	
		int layer = (int)TilemapLayer.Ground;
        TileBase tile = TileResource[tileLayer][0];

		recentTilemaps[layer].SetTile(newPos, tile);
		++recentTilesCount[layer];
	}



	public void Reset()
	{
        for (int i = 0; i < rooms.Count; ++i)
        {
            if (rooms[i] == null)
            {
                continue;
            }


            for (int k = 0; k < (int)TilemapLayer.End; ++k)
            {
                rooms[i].tilemaps[(TilemapLayer)k].ClearAllTiles();
			}

            if (Application.isPlaying)
            {
                GameObject.Destroy(rooms[i].gameObject);
            }
            else
            {
                DestroyImmediate(rooms[i].gameObject);  
			}
        }

        for(int i = 0; i < portals.Count; ++i)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(portals[i].gameObject);
            }
            else
            {
                DestroyImmediate(portals[i].gameObject);
            }
        }

		rooms = new List<Room_Drunken>();

		Setup();
    }



	private void Awake()
	{
        
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

