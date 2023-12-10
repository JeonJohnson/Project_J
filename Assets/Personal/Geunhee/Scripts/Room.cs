﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
	public int roomIndex;

	public List<Room> linkedRooms;

	public SpriteRenderer gridRenderer;

	private void Awake()
	{
		linkedRooms = new List<Room>();
	}

	

	private void OnDestroy()
	{
		linkedRooms = null;
	}
}
