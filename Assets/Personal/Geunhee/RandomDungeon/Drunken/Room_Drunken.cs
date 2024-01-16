using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;

public class Room_Drunken : MonoBehaviour
{
	public Vector3 centerPos;

	public SerializedDictionary<TilemapLayer, Tilemap> tilemaps;
}
