using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{

	public SpriteGrid grid;

	public List<Room> linkedRooms;

	private void Awake()
	{
		linkedRooms = new List<Room>();
	}
}
