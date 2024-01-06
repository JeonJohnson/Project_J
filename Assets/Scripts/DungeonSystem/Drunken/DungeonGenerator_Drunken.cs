using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Experimental.AI;


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
    public SpriteRenderer areaRd;

    [Header("Room Option")]
    public List<GameObject> GroundPrefabs;
    public List<GameObject> WallPrefabs;

    [ReadOnly]
    public int tileSize = 1;
    //Unity World unit Scale
    public Vector2Int areaSize;
    [ReadOnly]
    public Vector2 originPos;
    [ReadOnly]
    public Vector2 tileGridSize;
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


    
    [ReadOnly]
    public List<Explorer> explorers;
    
    public tileGridState[,] tileGrid;
    
    [HideInInspector]
    public List<GameObject> grounds;
    [ReadOnly]
    public int groundCount;
    [HideInInspector]
    public List<GameObject> walls;
    [ReadOnly]
    public int wallCount;

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

        grounds = new List<GameObject>();
        groundCount = grounds.Count;
             
        walls = new List<GameObject>();
        wallCount = walls.Count;

        Vector2 halfArea = Funcs.ToV2(areaSize) * 0.5f;
		float halfTileSize = tileSize * 0.5f;

		originPos = new(-halfArea.x + halfTileSize, -halfArea.y + halfTileSize);

        tileGridSize = Funcs.ToV2(areaSize) / tileSize;
		tileGrid = new tileGridState[(int)tileGridSize.x, (int)tileGridSize.y];

        areaRd.transform.localScale = new Vector3(areaSize.x, areaSize.y);
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
        if (areaFillRatio <= (float)groundCount / (float)tileGrid.Length)
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
                temp.dir = (ExplorerDir)(Random.Range((int)ExplorerDir.Up, (int)ExplorerDir.None+1));
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

            //if (tileGridSize.x % 2 == 0)
            //{
            //    temp.pos.x -= 0.5f;
            //}

            //if (tileGridSize.y % 2 == 0)
            //{
            //    temp.pos.y -= 0.5f;
            //}

            //Vector2 halfSize = Funcs.ToV2(areaSize) * 0.5f;
            temp.pos.x = Mathf.Clamp(temp.pos.x, originPos.x + 2, -originPos.x - 2);
			temp.pos.y = Mathf.Clamp(temp.pos.y, originPos.y + 2, -originPos.y - 2);
            

            if (GetIndex(temp.pos).x == int.MinValue)
			{
				return false;
			}

			var tile = CreateTile(temp.pos, tileGridState.Ground);
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
		for (int x = 0; x < tileGridSize.x - 1; ++x)
		{
			for (int y = 0; y < tileGridSize.y - 1; ++y)
			{
				if (tileGrid[x, y] == tileGridState.Ground)
                {
                    List<Vector2Int> createIndex = new List<Vector2Int>();

                    Vector2Int index = new Vector2Int(x, y);

					if (tileGrid[x, y + 1] == tileGridState.None)
					{
                        Vector2Int newIndex = index;
                        newIndex.y += 1;
                        createIndex.Add(newIndex);
					}
					
                    if (tileGrid[x, y - 1] == tileGridState.None)
					{
                        Vector2Int newIndex = index;
                        newIndex.y -= 1;
                        createIndex.Add(newIndex);
                    }
					
                    if (tileGrid[x + 1, y] == tileGridState.None)
					{
                        Vector2Int newIndex = index;
                        newIndex.x += 1;
                        createIndex.Add(newIndex);
                    }
					
                    if (tileGrid[x - 1, y] == tileGridState.None)
					{
                        Vector2Int newIndex = index;
                        newIndex.x -= 1;
                        createIndex.Add(newIndex);
                    }

					foreach (var item in createIndex)
					{
                        Vector2 pos = GetPos(item);
                        var wall = CreateTile(pos, tileGridState.Wall);
                        if (wall != null)
                        {
                            walls.Add(wall);
                        }
                        wallCount = walls.Count;
                    }
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
	public Vector2 GetDir(ExplorerDir num)
	{
        Vector2 val = Vector2Int.zero;

		switch (num)
		{
			case ExplorerDir.Up:
                val = Vector2Int.up;
                break;
			case ExplorerDir.Right:
                val = Vector2Int.right;
                break;
			case ExplorerDir.Down:
                val = Vector2Int.down;
                break;
			case ExplorerDir.Left:
                val = Vector2Int.left;
                break;
			default:
                break;
		}

        if (tileGridSize.x % 2 == 0)
        {
            val.x *= 0.5f;
        }

        if (tileGridSize.y % 2 == 0)
        {
            val.y *= 0.5f;
        }

        return val;
	}

    public Vector2Int GetIndex(Vector2 pos)
    {
        Vector2 index = (pos - originPos) / tileSize;

        if (index.x < 0f | index.y < 0f | index.x >= tileGridSize.x | index.y >= tileGridSize.y)
        {
            return new Vector2Int(int.MinValue, int.MinValue);
        }

		return new Vector2Int((int)index.x, (int)index.y);
	}

	public Vector2 GetPos(Vector2Int index)
	{
        if (index.x < 0 | index.y < 0 | index.x >= tileGridSize.x | index.y >= tileGridSize.y)
        {
            return new Vector2(float.MinValue, float.MinValue);
        }

		return originPos + Funcs.ToV2(index) * tileSize;
	}

	private GameObject CreateTile(Vector2 pos, tileGridState state)
    {
        //Vector2 sizeOffset = Funcs.ToV2(areaSize) * 0.5f;

        //Vector2 index = new Vector2(pos.x, pos.y) + sizeOffset;
        var index = GetIndex(pos);
          
        if (tileGrid[index.x, index.y] != state)
        {
            tileGrid[index.x, index.y] = state;

            GameObject tile = null;
            switch (state)
            {
                case tileGridState.Ground:
                    {
                        tile = Instantiate(GroundPrefabs[0], pos, Quaternion.identity, GroundBox);
                    }
                    break;
                case tileGridState.Wall:
                    {
                        tile = Instantiate(WallPrefabs[0], pos, Quaternion.identity, WallBox);
                    }
                    break;

             
            }

            tile.gameObject.name += $"{index.x} , {index.y}";
            return tile;
        }
        else return null;
    }

	public void Reset()
	{
		foreach (var item in walls)
		{
            GameObject.Destroy(item);
		}

        foreach (var item in grounds)
        {
            GameObject.Destroy(item);
        }

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

    public Vector2 testPos;
    public Vector2Int testIndex;
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("pos : " + GetPos(testIndex));
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Debug.Log("Index : " + GetIndex(testPos));
		}
	}
}

