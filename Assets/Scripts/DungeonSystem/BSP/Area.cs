using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
	public int index;
	public Rect rect;

	public SpriteGrid frame;

	[SerializeField]
	private Room assignedRoom = null;

	public Room AssignedRoom
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
