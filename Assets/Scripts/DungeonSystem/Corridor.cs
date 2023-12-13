using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Corridor : MonoBehaviour
{

	public Rect rect;

	public Tilemap tileMap;
	public SpriteGrid grid;

	public List<Room> linkedRooms;

	private void Awake()
	{
		linkedRooms = new List<Room>();
	}
}
