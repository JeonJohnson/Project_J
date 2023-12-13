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

//[System.Serializable]
//public class CornerPos
//{
//	public CornerPos(/*Transform tr*/)
//	{
//		//ReCalc(tr);
//	}

//	public CornerPos(Vector2 lt, Vector3 rt, Vector2 rb, Vector2 lb)
//	{ 
//		LT = lt;
//		RT = rt;
//		RB = rb;
//		LB = lb;
//	}
	
//	public Vector2 Size
//	{ 
//		get
//		{return new Vector2(Vector2.Distance(LT, RT), Vector2.Distance(LT, LB)); }
		
//	}

//	public void CalcCorner(Transform tr)
//	{
//		Vector2 center = tr.position;

//		float halfX = tr.localScale.x * 0.5f;
//		float halfY = tr.localScale.y * 0.5f;

//		LT = new Vector2(center.x - halfX, center.y + halfY);
//		RT = new Vector2(center.x + halfX, center.y + halfY);
//		RB = new Vector2(center.x + halfX, center.y - halfY);
//		LB = new Vector2(center.x - halfX, center.y - halfY);
//	}
	
//	public Vector2 LT, RT, RB, LB;
//}
public class Room : MonoBehaviour
{
	public Rect rect;
	public int belongsIndex;
	public RoomType roomType;

	public SpriteRenderer mySR;
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
		rect.size = mySR.transform.localScale;
		rect.center = new Vector2(transform.position.x + rect.size.x * 0.5f , transform.position.y + rect.size.y * 0.5f);

	}

	private void Awake()
	{
		//linkedRooms = new List<Room>();

		if(!grid) grid = GetComponentInChildren<SpriteGrid>();	
		if(!mySR) mySR = GetComponent<SpriteRenderer>();

		UpdateRect();

		grid.UpdateGrid();
	}

	

	private void OnDestroy()
	{
		//linkedRooms = null;
	}
}
