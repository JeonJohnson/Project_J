using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room_Isaac 
{
    public Room_Isaac(RoomShape_Isaac _shape, Vector2Int _index) 
    {

		doors = new Dictionary<Vector2Int, List<Vector2Int>>();
		shape = _shape;
		indexes = new Vector2Int[1] { _index };

		doors.Add(_index, new List<Vector2Int>());
	}

	public Room_Isaac(RoomShape_Isaac _shape, Vector2Int[] _index)
	{
		doors = new Dictionary<Vector2Int, List<Vector2Int>>();
		
		shape = _shape;
		indexes = new Vector2Int[_index.Length];
		//c#에서 배열은 레퍼카피임
		for (int i = 0; i < _index.Length; ++i)
		{
			indexes[i] = _index[i];
			doors.Add(_index[i], new List<Vector2Int>());
		}
	}

	public RoomShape_Isaac shape;
    public Vector2Int[] indexes;

	//KeyValuePair<인덱스, 방 방향>
	public Dictionary<Vector2Int, List<Vector2Int>> doors;
	//Dic<방 인덱스, List<방 방향>>
   
}
