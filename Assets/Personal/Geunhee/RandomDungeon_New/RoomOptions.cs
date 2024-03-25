using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoomOption : ScriptableObject
{
	[ReadOnly]
	public Vector3 pivotPos = Vector3.zero;
	public Vector2Int areaSize;
	[ReadOnly]
	public const float tileSize = 1;
}




//[CreateAssetMenu(fileName = "IsaacRoomOption", menuName = "Scriptable Object/RoomOption/Isaac")]
//public class RoomOption_Isaac : RoomOption
//{

//}


