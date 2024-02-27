using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

using Debug = Potato.Debug;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Room_BSP))]
public class Room_BSP_Editor : Editor
{
	public override void OnInspectorGUI()
	{
		Room_BSP room = (Room_BSP)target;

		DrawDefaultInspector();

		if (GUILayout.Button("UpdateRect"))
		{
			room.UpdateRect();
			Debug.Log(room.rect);
		}
	}
}
#endif

public enum RoomType
{ 
	Start,
	Normal,
	Event,//추후 구체화
	Shop,
	Boss,
	End
}
public class Room_BSP : MonoBehaviour
{
	//[field: System.NonSerialized]
	[ReadOnly]
	public Rect rect;
	[ReadOnly]
	public int belongsIndex;
	public RoomType roomType;

	[Header("TileMap")]
	public Tilemap wallTM;
	public List<KeyValuePair<Tile, Vector2Int>> wallTiles = new List<KeyValuePair<Tile, Vector2Int>>();
	public Tilemap groundTM;


	[Header("Grid")] 
	public SpriteRenderer planeSR;
	public SpriteGrid grid;

	public void UpdateRect()
	{
		rect.xMin = transform.position.x;
		rect.yMin = transform.position.y;

		//이거 타일맵의 cellBound가 이상할경우
		//Inspector창에서 TileMap 컴포넌트 우측상단에 톱니바퀴 눌러서 compreess bound 해줘야함.

		wallTM.CompressBounds();
		Vector2 size;
		size.x = wallTM.cellBounds.size.x;
		size.y = wallTM.cellBounds.size.y;

		rect.size = size;
	}

	//public void SetPosition(Vector2 centrPos)
	//{
	//	transform.position = new Vector3(centrPos.x - rect.width * 0.5f, centrPos.y - rect.height * 0.5f);

	//	UpdateRect();
	//}


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

	public void DoorConstruction(Corridor_BSP cor)
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
		grid.UpdateGrid();
#endif
	}

	

	private void OnDestroy()
	{
		//linkedRooms = null;
	}
}
