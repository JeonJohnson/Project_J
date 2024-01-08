using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area_BSP : MonoBehaviour
{
	public int index;
	public Rect rect;

	public SpriteGrid frame;

	[SerializeField]
	private Room_BSP assignedRoom = null;

	public Room_BSP AssignedRoom
	{
		get { return assignedRoom; }
		set { assignedRoom = value; }
	}
	public void SetRect()
	{
		rect.size = transform.localScale;
		rect.center = transform.position;
	}
	private void Awake()
	{
		
	}
}
