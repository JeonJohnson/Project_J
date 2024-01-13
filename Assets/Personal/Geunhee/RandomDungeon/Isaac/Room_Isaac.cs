using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room_Isaac 
{
    public Room_Isaac(RoomShape_Isaac _shape, Vector2Int _index) 
    {
		shape = _shape;
		indexes = new Vector2Int[1] { _index };
		//shape = _shape;

		//List<Vector2Int> pivotOffset = Defines.PivotByRoomShape[(int)shape];
		//int rand = Random.Range(0, pivotOffset.Count);
		//Vector2Int pivotIndex = _index + pivotOffset[rand];


		//switch (_shape)
		//{
		//	case RoomShape_Isaac.One:
		//		{
		//			indexes = new Vector2Int[1];
		//			indexes[0] = _index;
		//		}
		//		break;
		//	case RoomShape_Isaac.Two_Ver:
		//		{
		//			indexes = new Vector2Int[2];
		//			int rand = Random.Range(0, indexes.Length);
					
		//		}
		//			break;
		//	case RoomShape_Isaac.Two_Hor:
		//		{

		//		}
		//		break;
		//	case RoomShape_Isaac.Nieun:
		//		break;
		//	case RoomShape_Isaac.Nieun_Mirror:
		//		break;
		//	case RoomShape_Isaac.Giyeok:
		//		break;
		//	case RoomShape_Isaac.Giyeok_Mirror:
		//		break;
		//	case RoomShape_Isaac.Four:
		//		break;


		//	case RoomShape_Isaac.End:
		//			default:
		//				break;
		//			}
	}

	public Room_Isaac(RoomShape_Isaac _shape, Vector2Int[] _index)
	{
		shape = _shape;
		indexes = new Vector2Int[_index.Length];
		//c#에서 배열은 레퍼카피임
		for (int i = 0; i < _index.Length; ++i)
		{
			indexes[i] = _index[i];
		}
	}




	public RoomShape_Isaac shape;

    public Vector2Int[] indexes;

	//public List<KeyValuePair<Vector2Int, Vector2Int>> doors;
	//KeyValuePair<인덱스, 방 방향>
	public Dictionary<Vector2Int, List<Vector2Int>> doors;
	//Dic<방 인덱스, List<방 방향>>
   
}
