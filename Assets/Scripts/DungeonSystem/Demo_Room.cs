
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Demo_Room : MonoBehaviour
{

    public GameObject enterance;
    public GameObject exit;

    public Tilemap randomTM;
    public List<Vector2> monsterGenPos;
    public List<Vector2> itemGenPos;
    public List<Vector2> bossGenPos;

    public int curMonsterCount;

    private void SearchRandomTile()
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
                    tile = tileBase as Tile;

                    string temp = tile.sprite.name;
                }
            }
        }
    
    }


    void Start()
    {
        SearchRandomTile();
    }

    void Update()
    {
        
    }
}
