using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class Room_Isaac : MonoBehaviour 
{

	[SerializedDictionary("TileLayer", "Timemap")]
	public SerializedDictionary<TilemapLayer, Tilemap> tilemaps;
	public SerializedDictionary<TilemapLayer, TileBase> TileResource;

	[SerializeField]
	private NavMeshSurface navSurface;

	public RoomShape_Isaac shape;
    public Vector2Int[] indexes; //실제 위치 
	
	//Dic<방 인덱스(실제 위치), List<방 방향>>
	public Dictionary<Vector2Int, List<Vector2Int>> doors;



	public void Initialize(RoomShape_Isaac _shape, Vector2Int[] _index)
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

	public void Initialize(RoomShape_Isaac _shape, Vector2Int _index)
	{

		doors = new Dictionary<Vector2Int, List<Vector2Int>>();
		shape = _shape;
		indexes = new Vector2Int[1] { _index };

		doors.Add(_index, new List<Vector2Int>());
	}

	public void UpdateTiles()
	{
		switch (shape)
		{
			case RoomShape_Isaac.One:
				{
					Tilemap ground;
					TileBase tile;

					tilemaps.TryGetValue(TilemapLayer.Ground,out ground);
					TileResource.TryGetValue(TilemapLayer.Ground, out tile);

					int xMax = 14, yMax = 8;
					for (int x = 1; x < xMax; ++x)
					{
						for (int y = 1; y < yMax; ++y)
						{
							ground.SetTile(new(x, y, 0), tile);
						}
					}

					tilemaps.TryGetValue(TilemapLayer.Wall, out ground);
					TileResource.TryGetValue(TilemapLayer.Wall, out tile);


					for (int x = 0; x <= xMax; ++x)
					{
						for (int y = 0; y <=yMax; ++y)
						{
							if (x == 0 || x == xMax || y == 0 || y == yMax)
							{ ground.SetTile(new(x, y, 0), tile); }
						}
					}


					tilemaps.TryGetValue(TilemapLayer.Cliff, out ground);
					TileResource.TryGetValue(TilemapLayer.Cliff, out tile);

					foreach (var dir  in doors)
					{
						foreach (var pos  in dir.Value)
						{
							Vector2Int doorPos = new Vector2Int(xMax/2, yMax/2);

							doorPos += pos * new Vector2Int(xMax/2, yMax/2);

							{ ground.SetTile(new(doorPos.x, doorPos.y, 0), tile); }
						}
					}

				}
				break;
			case RoomShape_Isaac.Two_Ver:
				{ 

				}
				break;
			case RoomShape_Isaac.Two_Hor:
				{ 
				
				}
				break;
			case RoomShape_Isaac.Nieun:
				{ 
				
				}
				break;
			case RoomShape_Isaac.Nieun_Mirror:
				{ 
				
				}
				break;
			case RoomShape_Isaac.Giyeok:
				{ 
				
				}
				break;
			case RoomShape_Isaac.Giyeok_Mirror:
				{ 
				
				}
				break;
			case RoomShape_Isaac.Four:
				{ 
				
				}
				break;
			
		}


	}

	private void Awake()
	{
		

	}

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

}
