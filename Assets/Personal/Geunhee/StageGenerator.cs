using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;
using MoreMountains.Tools;

//스테이지

public enum RoomFormType
{ 
    Drunken,
    Isaac,
	End
}

public enum TilemapLayer
{
	//맵 Gameobject내에서 Tilemap을 깔 오브젝트 종류
	Ground,
	Prop,
	Shadow,
	Wall,
	Roof,
	End,


	Cliff
}


public abstract class StageGenerator : MonoBehaviour
{
	[Header("Simulation Option")]
	public Transform areaTr;
	public Camera cam;

	[Space(10f)]
	[Header("Result")]
	[ReadOnly]
	public Stage stage;
	[ReadOnly]
	public Room curRoom;
	[ReadOnly]
	public List<Room> rooms;

	//Internal Varis
	//protected int tileSize = 1;
	protected float tileHalfSize;
	
	protected Vector2 areaHalfSize;
	
	//protected Vector2 pivotPos;
	//protected Vector2 originPos;
	//protected Vector2 centerPos;

	//protected Vector2Int gridLength;
	//protected tileGridState[,] tileGrid;
	//protected Vector2Int centerIndex;
	//Internal Varis


	[Space(10f)]
	[Header("Generator Option")]
	public GameObject TilemapRefer_Prefab;
	public SerializedDictionary<TilemapLayer, List<TileBase>> TileResource;

	[Space(7.5f)]
	public GameObject ShopPrefab;
	public int shopCount;
	
	[Space(7.5f)]
	public GameObject BossRoomPrefab;

	//[ReadOnly]
	//public RoomFormType roomForm;
	[Tooltip("Exclude Boss&Shop Room Count, Only Normal Room")]
	public int roomCount; //Exclude Boss&Shop Room Count, Only Normal Room

	protected abstract void Setup();

	public abstract Room CreateOneRoom();
	
	public abstract void CreateRooms();

	public abstract void ResetStage();

	protected virtual void Awake()
	{
		
	}

	protected virtual void Start()
	{
		
	}
	protected virtual void Update()
	{
		
	}

}
