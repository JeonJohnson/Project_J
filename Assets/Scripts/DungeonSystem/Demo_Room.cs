
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public enum eRandomGenList
{ 
    Player,
    Item,
    Monster,
    Boss,
    End
}


public class Demo_Room : MonoBehaviour
{
    [SerializeField]
   private GameObject enterance;
	[SerializeField]
	private GameObject exit;

	//[SerializeField]
	//private TilemapRenderer randomEleRdr;


	public Tilemap randomTM;

    List<Vector2>[] randomGenList;
    //public List<Vector2> monsterGenPos;
    //public List<Vector2> itemGenPos;
    //public List<Vector2> bossGenPos;

    public int curMonsterCount;


    /// <summary>
    /// True == Open / False == Close
    /// </summary>
    /// <param name="openclose"></param>
    public void ExitDoor(bool openclose)
    { 
        if(exit== null) { return; }

        TilemapRenderer tmRdr = exit.GetComponent < TilemapRenderer>();
        CompositeCollider2D col = exit.GetComponent<CompositeCollider2D>();

        col.isTrigger = openclose;
        tmRdr.enabled = !openclose;
    }

    public void EnteranceDoor(bool openclose)
    {
        if (enterance == null) { return; }


		TilemapRenderer tmRdr = enterance.GetComponent<TilemapRenderer>();
		CompositeCollider2D col = enterance.GetComponent<CompositeCollider2D>();

		col.isTrigger = openclose;
		tmRdr.enabled = !openclose;
	}




    private void SearchRandomGenTile()
    {
        BoundsInt bound = randomTM.cellBounds;
        TileBase[] allTile = randomTM.GetTilesBlock(bound);

        for (int y = 0; y < bound.size.y; ++y)
        {
            for (int x = 0; x < bound.size.x; ++x)
            {
                TileBase tileBase = allTile[x + (y * bound.size.x)];
                Tile tile;
                if (tileBase != null)
                {
                    //tile = tileBase as Tile;
                    string tileName = tileBase.name;

                    for (int i = 0; i < (int)eRandomGenList.End; ++i)
                    {
                        string str = Funcs.GetEnumName<eRandomGenList>(i);

                        if (tileName.Contains(str))
                        {
                            var worldPos = randomTM.CellToWorld(new(x, y, 0));
                            randomGenList[i].Add(new Vector2(worldPos.x, worldPos.y));
                        }
                    }

                }
            }
        }

        curMonsterCount = randomGenList[(int)eRandomGenList.Monster].Count;
    }

    public void RandomObjectGen()
    {
        for (int i = 0; i < randomGenList.Length; ++i)
        {
            Vector2 pos;
            switch ((eRandomGenList)i)
            {
                case eRandomGenList.Player:
                    {
                        //정민아 여기서 플레이어 생성
                        
                    }
                    break;
                case eRandomGenList.Item:
					{
                        //정민아 여기서 아이템 랜덤 생성
                        foreach (var item in randomGenList[i])
                        {
                            pos = item;
                        }
					}
                    break;
                case eRandomGenList.Monster:
                    { 
                        //정민아 여기서 몬스터 랜덤 생성
                    }
                    break;
                case eRandomGenList.Boss:
                    { 
                        //정민아 여기서 보스 몬스터 랜덤 생성.
                    }
                    break;
                default:
                    break;
            }
        }
    
    }

	private void Awake()
	{
        randomGenList = new List<Vector2>[(int)eRandomGenList.End];

        for(int i  = 0; i < randomGenList.Length; ++i)
        {
            randomGenList[i] = new List<Vector2>();
        }

        SearchRandomGenTile();

        randomTM.GetComponent<TilemapRenderer>().enabled = false;
	}

	void Start()
    {
        

    }

    void Update()
    {
        
    }
}
