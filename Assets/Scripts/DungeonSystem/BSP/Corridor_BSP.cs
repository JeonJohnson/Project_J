using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Corridor_BSP : MonoBehaviour
{
	public Rect rect;

	public eDirection dir;
	
	public List<Room_BSP> linkedRooms;
	
	public Tilemap wallTileMap;
	public Tilemap wayTileMap;
	public Tile[]	tiles;
	//타일 까는거는 나중에 외부에서 데이터 가지고 있다가 스테이지에 맞는 타일 깔아 주는 걸로 바꿔야함.

	public Transform planeTr;
	public SpriteGrid grid;

	public void SetRect(Rect _rect)
	{
		rect = _rect;
		transform.position = rect.position;
		
		planeTr.transform.localScale = new Vector2(rect.width, rect.height);
		planeTr.transform.localPosition = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
		grid.UpdateGrid();
		//실제 타일맵 컴포넌트로 타일 깔아주기
		SetTiles();
	}

	public void SetTiles()
	{
		switch (dir)
		{
			case eDirection.Vertical:
				{
					for (int y = 0; y < rect.height; ++y)
					{
						for (int x = 0; x < rect.width; ++x)
						{
							Tilemap curTilemap = x == 1 ? wayTileMap : wallTileMap;

							curTilemap.SetTile(new Vector3Int(x, y, 0), tiles[x]);
						}
					}
				}
				break;
			case eDirection.Horizon:
				{
					for (int y = 0; y < rect.height; ++y)
					{
						for (int x = 0; x < rect.width; ++x)
						{
							Tilemap curTilemap = y == 1 ? wayTileMap : wallTileMap;

							curTilemap.SetTile(new Vector3Int(x, y, 0), tiles[y]);
						}
					}
				}
				break;
			default:
				break;
		}

		wallTileMap.gameObject.GetComponent<CompositeCollider2D>().GenerateGeometry();

	}
	

	private void Awake()
	{
		linkedRooms = new List<Room_BSP>();
	}
}
