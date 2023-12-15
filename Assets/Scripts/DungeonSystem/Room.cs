using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum RoomType
{ 
	Start,
	Normal,
	Event,//추후 구체화
	Shop,
	Boss,
	End
}
public class Room : MonoBehaviour
{
	public Rect rect;
	public int belongsIndex;
	public RoomType roomType;



	public Tilemap wallTM;
	public List<KeyValuePair<Tile, Vector2Int>> wallTiles = new List<KeyValuePair<Tile, Vector2Int>>();
	public Tilemap groundTM;






	public SpriteRenderer planeSR;
	public SpriteGrid grid;

	public void SetPosition(Vector2 pos)
	{
		transform.position = new Vector3(pos.x - rect.width * 0.5f, pos.y - rect.height * 0.5f);

		UpdateRect();
	}

	public void SetPosition(int x, int y)
	{
		Vector2 pos = new(x, y);
		transform.position = new Vector3(pos.x - rect.width * 0.5f, pos.y - rect.height * 0.5f);

		UpdateRect();
	}

	

	public void UpdateRect()
	{
#if UNITY_EDITOR
		rect.size = planeSR.transform.localScale;
#endif
		rect.center = new Vector2(transform.position.x + rect.size.x * 0.5f , transform.position.y + rect.size.y * 0.5f);

	}


	public void FindWallTiles()
	{
		for (int y = 0; y < rect.height; ++y)
		{
			for (int x = 0; x < rect.width; ++x)
			{
				Tile tile = wallTM.GetTile<Tile>(new Vector3Int(x, y, 0));
				
				if (tile != null)
				{
					KeyValuePair<Tile, Vector2Int> temp = new(tile, new(x, y));
					wallTiles.Add(temp);
				}

			}
		}
	
	}

	public void DoorConstruction(Corridor cor)
	{
		//벽 타일 중에 통로 +2 사이즈 만큼과 닿는 타일들 지우기
		Rect corRect = cor.rect;

		corRect.xMin -= cor.dir == eDirection.Horizon ? 1 : 0;
		corRect.xMax += cor.dir == eDirection.Horizon ? 1 : 0;

		corRect.yMin -= cor.dir == eDirection.Vertical ? 1 : 0;
		corRect.yMax += cor.dir == eDirection.Vertical ? 1 : 0;


		foreach (var item in wallTiles)
		{
			if (corRect.Contains(wallTM.CellToWorld(new (item.Value.x, item.Value.y, 0))))
			{
				wallTM.SetTile(new(item.Value.x, item.Value.y), null);
			}
		}

	}

	private void Awake()
	{
		//linkedRooms = new List<Room>();

#if UNITY_EDITOR
		if(!grid) grid = GetComponentInChildren<SpriteGrid>();	
		if(!planeSR) planeSR = GetComponent<SpriteRenderer>();
#endif
		UpdateRect();

#if UNITY_EDITOR
		grid.UpdateGrid();
#endif
	}

	

	private void OnDestroy()
	{
		//linkedRooms = null;
	}
}
