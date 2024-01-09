using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Tilemaps;

using UnityEngine;
using UnityEngine.Tilemaps;


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
    public Transform areaTr;
    public Camera cam;
    public GameObject tilemapOriginPrefab;

    [Space(10f)]
    //[Header("Boxes")]
    //public Transform GroundBox;
    //public Transform WallBox;

    [Space(10f)]
    [Header("Tilemap")]
    public Tilemap groundTilemap;
    public Tilemap propTilemap;
    public Tilemap wallTilemap;

    [Space(10f)]
    [Header("Room Option")]
    //public List<GameObject> GroundPrefabs;
    //public List<GameObject> WallPrefabs;
    
    public List<Tile> GroundTiles;
    public List<Tile> WallTiles;
    public List<Tile> CliffTiles;

    //[Range(1,10)]
    //public int roomCount;
    //[ReadOnly]
    //public int curCreateNum;
    public GameObject rooms;
    
    [Space(7.5f)]
    [ReadOnly]
    public int groundTilesCount;
    [ReadOnly]
    public int wallTilesCount;
    [ReadOnly]
    public int tileSize = 1;
    //Unity World unit Scale
    public Vector2Int areaSize;
    public Vector2 pivotPos;
    [ReadOnly]
    public Vector2 originPos;
    [ReadOnly]
    public Vector2 centerPos;
    [ReadOnly]
    public Vector2Int gridLength;

    //해당 비율만큼 바닥 만들었으면 끝.
    [Range(0.25f, 0.75f)]
    public float areaFillRatio;

    [Tooltip("About wall is 1*1 of the grounds.")]
    //Ground사이에 1*1있는 Wall(불량화소) 무시하고 ground로 덮어쓸 확률
    [Range(0f, 1f)]
    public float ignorePercent;
    [Range(1000, 10000)]
    public int maxTryCount;

    enum NewExplorerPos { center, prePos };

    [Space(10f)]
    [Header("Explorer Option")]
    [SerializeField]
    private NewExplorerPos ExplorerSpawnOption;

    [Space(7.5f)]
    [ReadOnly]
    public int curExplorerCount;
    public int startExplorerCount;
    public Vector2Int ExplorerCountRange;

    [Space(7.5f)]
    [Range(0f, 1f)]
    public float destoryPercent;
    public Vector2Int destoryCountRange; //한번에 최고 몇명까지 해고할지
    [ReadOnly]
    public int destoryCount;

    [Space(7.5f)]
    [Range(0f, 1f)]
    public float respawnPercent;
    public Vector2Int respawnCountRange;
    [ReadOnly]
    public int respawnCount;

    [Space(7.5f)]
    [Range(0f, 1f)]
    public float newDirPercent;

    [Space(10f)]
    [Header("Simulate Option")]
    [Range(0f, 1f)]
    public float loopWaitTime;

    [Space(10f)]
    [Header("Display Varis")]


    //Internal Varis
    Vector2Int centerIndex;
    private Vector2 areaHalfSize;
    private float tileHalfSize;
    
    private List<Explorer> explorers;
    private tileGridState[,] tileGrid;
    //private GameObject[,] tileObjs;
    //private List<GameObject> grounds;
    //private List<GameObject> walls;
    //Internal Varis

    public void Setup()
    {
        cam ??= Camera.main;

        //if (GroundPrefabs == null || GroundPrefabs.Count == 0)
        //{
        //    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //    var renderer = tile.GetComponent<MeshRenderer>();
        //    var mat = renderer.material;
        //    mat.color = new Color(150 / 255, 75 / 255, 0f);
        //    //Brown
        //    renderer.material = mat;
        //    GroundPrefabs = new List<GameObject> { tile };
        //}

        //if (WallPrefabs == null || WallPrefabs.Count == 0)
        //{
        //    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //    var renderer = tile.GetComponent<MeshRenderer>();
        //    var mat = renderer.material;
        //    mat.color = new Color(1f, 1f, 1f);
        //    renderer.material = mat;
        //    WallPrefabs = new List<GameObject> { tile };
        //}

        gridLength.x = Mathf.RoundToInt(areaSize.x / tileSize);
        gridLength.y = Mathf.RoundToInt(areaSize.y / tileSize);
        tileGrid = new tileGridState[gridLength.x, gridLength.y];

        areaHalfSize = Funcs.ToV2(gridLength) * 0.5f;
        tileHalfSize = tileSize * 0.5f;

        centerPos = pivotPos + new Vector2(gridLength.x * 0.5f, gridLength.y * 0.5f);
        originPos = new Vector2(pivotPos.x + tileHalfSize, pivotPos.y + tileHalfSize);

        areaTr.localScale = new Vector3(gridLength.x, gridLength.y);
        areaTr.localPosition = new Vector3(gridLength.x * 0.5f, gridLength.y * 0.5f);

        cam.transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
        cam.orthographicSize = gridLength.x <= gridLength.y ? gridLength.y * 0.5f : gridLength.x * 0.5f;
        cam.orthographicSize += 1;


        centerIndex = new Vector2Int((int)(gridLength.x * 0.5f), (int)(gridLength.y * 0.5f));
        explorers = new List<Explorer>();
        for (int i = 0; i < startExplorerCount; i++)
        {
            explorers.Add(new Explorer(centerIndex));
        }

        //rooms = new GameObject[roomCount];
        //groundTilemap = new Tilemap[roomCount];
        //wallTilemap = new Tilemap[roomCount];

        //for (int i = 0; i < roomCount; ++i)
        //{
            GameObject obj = Instantiate(tilemapOriginPrefab);
            groundTilemap = obj.transform.Find("Ground").GetComponent<Tilemap>();
            wallTilemap = obj.transform.Find("Wall").GetComponent<Tilemap>();
        //}

        //grounds = new List<GameObject>();
        //groundCount = grounds.Count;

        //walls = new List<GameObject>();
        //wallCount = walls.Count;
    }


    public void CreateRoom()
    {
        //for (int i = 0; i < roomCount; ++i)
        //{
        //    curCreateNum = i;
            CreateGround();
            CreateWall();
            CreateCliff();
        //}
    
    }

    public void CreateGround()
    {
            int curTryCount = 0;
            while (curTryCount < maxTryCount)
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
        //StartCoroutine(GroundLoop());
    }
    //public void CreateGround_Immediately()
    //{
    //    for (int i = 0; i < roomCount; ++i)
    //    {
    //        int curTryCount = 0;

    //        while (curTryCount < maxTryCount)
    //        {
    //            if (MoveExplorer())
    //            {
    //                ++curTryCount;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //}
    public IEnumerator GroundLoop()
    {
        int curTryCount = 0;

        while (curTryCount < maxTryCount)
        {
            if (MoveExplorer())
            {
                ++curTryCount;
                yield return new WaitForSeconds(loopWaitTime);
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
        if (areaFillRatio <= (float)groundTilesCount / (float)tileGrid.Length)
        {
            return false;
        }

        //2. fire explorer
        destoryCount = Random.Range(destoryCountRange.x, destoryCountRange.y + 1);
        List<Explorer> fired = new List<Explorer>();
        for (int i = 0; i < explorers.Count; ++i)
        {
            if (fired.Count >= destoryCount || explorers.Count - fired.Count <= ExplorerCountRange.x)
            {
                break;
            }

            if (UnityEngine.Random.value <= destoryPercent)
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

			/*var tile = */CreateTile(temp.index, tileGridState.Ground);
            //if (tile != null)
            //{
            //    ++groundCount;
            //    grounds.Add(tile);
            //}
            explorers[i] = temp;
        }

        return true;
    }

    public void CreateWall()
    {
        
		for (int x = 0; x < gridLength.x - 1; ++x)
		{
			for (int y = 0; y < gridLength.y - 1; ++y)
			{
                    List<Vector2Int> createIndex = new List<Vector2Int>();

                    Vector2Int index = new Vector2Int(x, y);

					if (tileGrid[x, y] == tileGridState.None)
					{
						createIndex.Add(index);
					}


					//if (tileGrid[x, y + 1] == tileGridState.None)
					//{
					//                   Vector2Int newIndex = index;
					//                   newIndex.y += 1;
					//                   createIndex.Add(newIndex);
					//}

					//               if (tileGrid[x, y - 1] == tileGridState.None)
					//{
					//                   Vector2Int newIndex = index;
					//                   newIndex.y -= 1;
					//                   createIndex.Add(newIndex);
					//               }

					//               if (tileGrid[x + 1, y] == tileGridState.None)
					//{
					//                   Vector2Int newIndex = index;
					//                   newIndex.x += 1;
					//                   createIndex.Add(newIndex);
					//               }

					//               if (tileGrid[x - 1, y] == tileGridState.None)
					//{
					//                   Vector2Int newIndex = index;
					//                   newIndex.x -= 1;
					//                   createIndex.Add(newIndex);
					//               }

					foreach (var item in createIndex)
					{
                        Vector2 pos = GetPos(item);
                        /*var wall = */CreateTile(pos, tileGridState.Wall);
                        //if (wall != null)
                        //{
                        //    walls.Add(wall);
                        //}
                        //wallCount = walls.Count;
                    }
				
			}
		}
	}

    public void CreateCliff()
    {
        List<Vector2Int> createIndex = new List<Vector2Int>();

        for (int y = 1; y < gridLength.y - 1; ++y)
        {
            for (int x = 0; x < gridLength.x - 1; ++x)
            {
                if (tileGrid[x, y] == tileGridState.Wall)
                {
                    Vector2Int index = new Vector2Int(x, y);

					if (tileGrid[x, y - 1] == tileGridState.Ground)
					{
						Vector2Int newIndex = index;
						newIndex.y -= 1;
						createIndex.Add(newIndex);
					}

				}
            }
        }

        foreach (var item in createIndex)
        {
            Vector2 pos = GetPos(item);
            wallTilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), CliffTiles[0]);
            wallTilesCount++;
        }
    }


    //  public Vector2Int GetDir(int num)
    //  {
    //      switch ((ExplorerDir)num)
    //      {
    //	case ExplorerDir.Up:
    //		return Vector2Int.up;
    //	case ExplorerDir.Right:
    //		return Vector2Int.right;
    //	case ExplorerDir.Down:
    //		return Vector2Int.down;
    //	case ExplorerDir.Left:
    //		return Vector2Int.left;
    //	default:
    //		return Vector2Int.zero;
    //}
    //  }

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
    public Vector2 GetPos(Vector2Int index)
    {
        if (index.x < 0 | index.y < 0 | index.x >= gridLength.x | index.y >= gridLength.y)
        {
            return new Vector2(float.MinValue, float.MinValue);
        }

        Vector2 pos = pivotPos + new Vector2(index.x * tileSize + tileHalfSize, index.y * tileSize + tileHalfSize);

        return pos;
    }

    public Vector2Int GetIndex(Vector2 pos)
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

			//GameObject tile = null;

			switch (state)
			{
				case tileGridState.Ground:
					{
                        groundTilemap.SetTile(new Vector3Int((int)pos.x,(int)pos.y,0), GroundTiles[0]);
                        groundTilesCount++;
						//tile = Instantiate(GroundPrefabs[0], pos, Quaternion.identity, GroundBox);
					}
					break;
				case tileGridState.Wall:
					{
                        wallTilemap.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), WallTiles[0]);
                        wallTilesCount++;
                        //tile = Instantiate(WallPrefabs[0], pos, Quaternion.identity, WallBox);
                    }
					break;
			}

			//tile.gameObject.name += $"{index.x} , {index.y}";
			//return tile;
		}
		//else return null;
	}


	public void Reset()
	{

        //for (int i = 0; i < roomCount; ++i)
        //{
        //    groundTilemap[i].ClearAllTiles();
        //    wallTilemap[i].ClearAllTiles();
        //    GameObject.Destroy(rooms[i]);
        //}

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        GameObject.Destroy(rooms);

        //foreach (var item in walls)
        //{
        //          if (Application.isPlaying)
        //          { GameObject.Destroy(item); }
        //          else { DestroyImmediate(item); }
        //}

        //      foreach (var item in grounds)
        //      {
        //          if (Application.isPlaying)
        //          { GameObject.Destroy(item); }
        //          else { DestroyImmediate(item); }
        //      }

        Setup();
    }



	private void Awake()
	{
        Setup();
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame

    //public Vector2 testPos;
    //public Vector2Int testIndex;
    void Update()
    {
		//if (Input.GetKeyDown(KeyCode.A))
		//{
		//	Debug.Log("pos : " + GetPos(testIndex));
		//}

		//if (Input.GetKeyDown(KeyCode.S))
		//{
		//	Debug.Log("Index : " + GetIndex(testPos));
		//}
	}
}

