using System.Collections;
using System.Collections.Generic;

using UnityEngine;




public class Stage : MonoBehaviour
{
	public List<Room> rooms;

	[ReadOnly]
	public int curRoomIndex;
	public Room curRoom;

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
