using System.Collections;
using System.Collections.Generic;

using UnityEngine;




public class Stage : MonoBehaviour
{
	[ReadOnly]
	public int curRoomIndex;
	[ReadOnly]
	public Room curRoom;

	[Space(10f)]
	public List<Room> rooms;
	[Space(7.5f)]
	public List<Room> normalRooms;
	public Room bossRoom;
	public Room shopRoom;

	public void Initialize()
	{ 
		
	
	}

	public virtual void NextRoom()
	{
		curRoom.Cleanup();



	
	}

	private void Awake()
	{
		rooms = new List<Room>();	
	}
}
