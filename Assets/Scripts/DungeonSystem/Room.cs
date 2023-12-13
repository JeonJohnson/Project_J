using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

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

	public SpriteRenderer planeSR;
	public SpriteGrid grid;

	public Tilemap tileMap;

	//public void SetScale(Vector2 size)
	//{ 
	//	transform.localScale = size;
	//	grid.UpdateGrid();
	//}

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
		rect.size = planeSR.transform.localScale;
		rect.center = new Vector2(transform.position.x + rect.size.x * 0.5f , transform.position.y + rect.size.y * 0.5f);

	}

	private void Awake()
	{
		//linkedRooms = new List<Room>();

		if(!grid) grid = GetComponentInChildren<SpriteGrid>();	
		if(!planeSR) planeSR = GetComponent<SpriteRenderer>();

		UpdateRect();

		grid.UpdateGrid();
	}

	

	private void OnDestroy()
	{
		//linkedRooms = null;
	}
}
