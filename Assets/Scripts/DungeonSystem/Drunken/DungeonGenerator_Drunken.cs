using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Experimental.AI;


public enum TileState
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
    public Explorer(Vector2 _pos)
    {
        pos = _pos;
        dir = ExplorerDir.None;
	}
    public Vector2 pos;
    public ExplorerDir dir;
}

public class DungeonGenerator_Drunken : MonoBehaviour
{
    [Header("Boxes")]
    public Transform GroundBox;
    public Transform WallBox;

    [Header("Room Option")]
    public List<GameObject> GroundPrefabs;
    public List<GameObject> WallPrefabs;

    //Unity World unit Scale
    public Vector2Int areaSize;
    //해당 비율만큼 바닥 만들었으면 끝.
    [Range(0.25f, 0.75f)]
    public float areaFillRatio;

    [Tooltip("About wall is 1*1 of the grounds.")]
    //Ground사이에 1*1있는 Wall(불량화소) 무시하고 ground로 덮어쓸 확률
    [Range(0f, 1f)]
    public float ignorePercent;
    [Range(1000, 10000)]
    public int maxTryCount;


    [Space(10f)]
    [Header("Explorer Option")]
    [ReadOnly]
    public int curExplorerCount;
    public int startExplorerCount;
    public Vector2Int ExplorerCountRange;
    [Range(0f, 1f)]
    public float destoryPercent;
    public Vector2Int destoryCountRange; //한번에 최고 몇명까지 해고할지
    [ReadOnly]
    public int destoryCount;
    [Range(0f, 1f)]
    public float respawnPercent;
    public Vector2Int respawnCountRange;
    [ReadOnly]
    public int respawnCount;
    [Range(0f, 1f)]
    public float newDirPercent;

    [Space(10f)]
    [Header("Simulate Option")]
    [Range(0f, 1f)]
    public float loopWaitTime;


    [HideInInspector]
    public int tileSize;
    [ReadOnly]
    public List<Explorer> explorers;
    public TileState[,] tiles;
    [ReadOnly]
    public int groundCount;
    [HideInInspector]
    public List<GameObject> grounds;
    [ReadOnly]
    public int wallCount;
    [HideInInspector]
    public List<GameObject> walls;
    private Camera cam;



    public void Setup()
    {
        cam ??= Camera.main;

        if (GroundPrefabs == null || GroundPrefabs.Count == 0)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var renderer = tile.GetComponent<MeshRenderer>();
            var mat = renderer.material;
            mat.color = new Color(150 / 255, 75 / 255, 0f);
            //Brown
            renderer.material = mat;
            GroundPrefabs = new List<GameObject> { tile };
        }

        if (WallPrefabs == null || WallPrefabs.Count == 0)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var renderer = tile.GetComponent<MeshRenderer>();
            var mat = renderer.material;
            mat.color = new Color(1f, 1f, 1f);
            renderer.material = mat;
            WallPrefabs = new List<GameObject> { tile };
        }

        explorers = new List<Explorer>();
        for (int i = 0; i < startExplorerCount; i++)
        {
            explorers.Add(new Explorer());
        }

        tiles = new TileState[areaSize.x, areaSize.y];

        grounds = new List<GameObject>();
        walls = new List<GameObject>();
    }

    public void CreateGround()
    {
        StartCoroutine(GroundLoop());
    }
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
        if (areaFillRatio <= (float)groundCount / (float)tiles.Length)
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
                { respawnList.Add(new Explorer(explorers[i].pos)); }
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
                temp.dir = (ExplorerDir)(Random.Range((int)ExplorerDir.Up, (int)ExplorerDir.None));
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
            temp.pos += GetDir(temp.dir);
            Vector2 halfSize = Funcs.ToV2(areaSize) * 0.5f;
            temp.pos.x = Mathf.Clamp(temp.pos.x, -halfSize.x + 2, halfSize.x - 2);
            temp.pos.y = Mathf.Clamp(temp.pos.y, -halfSize.y + 2, halfSize.y - 2);

            var tile = CreateTile(temp.pos, TileState.Ground);
            if (tile != null)
            {
                ++groundCount;
                grounds.Add(tile);
            }
            explorers[i] = temp;
        }

        return true;
    }

    public void CreateWall()
    {
		for (int x = 0; x < areaSize.x - 1; ++x)
		{
			for (int y = 0; y < areaSize.y - 1; ++y)
			{
    //            x = (int)(x - (areaSize.x * 0.5f));
				//y = (int)(y - (areaSize.y * 0.5f));

				if (tiles[x, y] == TileState.Ground)
                {
                    Vector2Int index = new Vector2Int(x, y);

					if (tiles[x, y + 1] == TileState.None)
					{
						index.y += 1;
					}
					else if (tiles[x, y - 1] == TileState.None)
					{
						index.y -= 1;
					}
					else if (tiles[x + 1, y] == TileState.None)
					{
						index.x += 1;
					}
					else if (tiles[x - 1, y] == TileState.None)
					{
						index.x -= 1;
					}
                    Vector2 pos;
                    pos.x = (int)(index.x - (areaSize.x * 0.5f));
					pos.y = (int)(index.y - (areaSize.y * 0.5f));
                    walls.Add(CreateTile(pos, TileState.Wall));
                    wallCount = walls.Count;
				}
			}
		}
	}


    public Vector2Int GetDir(int num)
    {
        switch ((ExplorerDir)num)
        {
			case ExplorerDir.Up:
				return Vector2Int.up;
			case ExplorerDir.Right:
				return Vector2Int.right;
			case ExplorerDir.Down:
				return Vector2Int.down;
			case ExplorerDir.Left:
				return Vector2Int.left;
			default:
				return Vector2Int.zero;
		}
    }
	public Vector2Int GetDir(ExplorerDir num)
	{
		switch (num)
		{
			case ExplorerDir.Up:
				return Vector2Int.up;
			case ExplorerDir.Right:
				return Vector2Int.right;
			case ExplorerDir.Down:
				return Vector2Int.down;
			case ExplorerDir.Left:
				return Vector2Int.left;
			default:
				return Vector2Int.zero;
		}
	}

    private GameObject CreateTile(Vector2 pos, TileState state)
    {

        Vector2 sizeOffset = Funcs.ToV2(areaSize) * 0.5f;
        Vector2 index = new Vector2(pos.x, pos.y) + sizeOffset;
        //pos = new Vector2Int((int)realPos.x, (int)realPos.y);

        if (tiles[(int)index.x, (int)index.y] != state)
        {
            tiles[(int)index.x, (int)index.y] = state;

            GameObject tile = null;
            switch (state)
            {
                case TileState.Ground:
                    {
                        tile = Instantiate(GroundPrefabs[0], pos, Quaternion.identity, GroundBox);
                    }
                    break;
                case TileState.Wall:
                    {
                        tile = Instantiate(WallPrefabs[0], pos, Quaternion.identity, WallBox);
                    }
                    break;
            }
            return tile;
        }
        else return null;
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
    void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    MoveExplorer();
        //}
    }
}

