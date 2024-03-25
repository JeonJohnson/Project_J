using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using AYellowpaper;
using AYellowpaper.SerializedCollections;
using MoreMountains.Tools;
using System;

//스테이지

public enum RoomFormType
{ 
    Drunken,
    Isaac,
	End
}

[Flags]
public enum TilemapFlag
{
	None = 0,
	Ground = 1 << 0, //2^0 =  1
	Prop = 1 << 1, //2^1 = 2 
	Shadow = 1 << 2, //2^2 = 4
	Wall  = 1 << 3, //2^3 = 8
	Roof = 1 << 4,
	
}


public abstract class StageGenerator : MonoBehaviour
{
	[Header("Simulation Option")]
	public Transform areaTr;
	public Camera cam;

	[Space(10f)]
	//[Header("Result")]
	public GameObject StagePrefab;
	[ReadOnly]
	public Stage stage;
	//[ReadOnly]
	//public Room curRoom;
	//[ReadOnly]
	//public List<Room> rooms;



	[Space(10f)]
	[Header("Generator Option")]
	public GameObject TilemapRefer_Prefab;
	public SerializedDictionary<TilemapFlag, List<TileBase>> TileResource;

	[Space(7.5f)]
	[Tooltip("Exclude Boss&Shop Room Count, Only Normal Room")]
	public int roomCount; //Exclude Boss&Shop Room Count, Only Normal Room

	[Space(7.5f)]
	public GameObject ShopPrefab;
	public int shopCount;
	
	[Space(7.5f)]
	public GameObject BossRoomPrefab;

	//[ReadOnly]
	//public RoomFormType roomForm;


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


	protected abstract void Setup();

	public abstract Room CreateOneRoom();
	
	public abstract Stage CreateStage();

	public abstract void ResetStage();

	protected virtual void Awake()
	{
		//rooms = new List<Room>();
	}

	protected virtual void Start()
	{
		
	}
	protected virtual void Update()
	{
		
	}

}
