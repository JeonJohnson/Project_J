using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CornerPos
{
	public CornerPos(/*Transform tr*/)
	{
		//ReCalc(tr);
	}

	public CornerPos(Vector2 lt, Vector3 rt, Vector2 rb, Vector2 lb)
	{ 
		LT = lt;
		RT = rt;
		RB = rb;
		LB = lb;
	}
	
	public Vector2 Size
	{ 
		get
		{return new Vector2(Vector2.Distance(LT, RT), Vector2.Distance(LT, LB)); }
		
	}

	public void CalcCorner(Transform tr)
	{
		Vector2 center = tr.position;

		float halfX = tr.localScale.x * 0.5f;
		float halfY = tr.localScale.y * 0.5f;

		LT = new Vector2(center.x - halfX, center.y + halfY);
		RT = new Vector2(center.x + halfX, center.y + halfY);
		RB = new Vector2(center.x + halfX, center.y - halfY);
		LB = new Vector2(center.x - halfX, center.y - halfY);
	}
	
	public Vector2 LT, RT, RB, LB;
}
public class Room : MonoBehaviour
{
	public CornerPos cornerPos = new CornerPos();

	public List<Room> linkedRooms;

	public int roomIndex;

	public SpriteRenderer gridRenderer;
	//public void UpdateGrid()
	//{
	//	Vector2 size = transform.localScale;
	//	size.x = 1 / size.x;
	//	size.y = 1 / size.y;
	//	gridRenderer.transform.localScale = size;
	//	gridRenderer.size = transform.localScale;
	//}

	private void Awake()
	{
		linkedRooms = new List<Room>();
	}

	public void Start()
	{
		//cornerPos = new CornerPos();
		
	}

	private void OnDestroy()
	{
		cornerPos = null;
	}
}
