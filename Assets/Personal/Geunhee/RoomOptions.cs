using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoomOption : ScriptableObject
{
	[ReadOnly]
	public Vector3 pivotPos = Vector3.zero;
	public Vector2Int areaSize;
	[ReadOnly]
	public float tileSize = 1;
}


[CreateAssetMenu(fileName = "DrunkenRoonOption", menuName = "Scriptable Object/RoomOption/Drunken")]
public class RoomOption_Drunken : RoomOption
{
	//public Vector2Int areaSize;
	public int enemyCount;

	[Space(7.5f)]
	//해당 비율만큼 바닥 만들었으면 끝.
	[Range(0.25f, 0.75f)]
	public float areaFillRatio;
	[Range(10000, 9999999)]
	public int maxTryCount;
	[ReadOnly]
	public int curTryCount;

	[Space(7.5f)]
	[SerializeField]
	public NewExplorerPos ExplorerSpawnOption;

	[Space(10f)]
	public List<Explorer> explorers;
	public int destoryCount;
	public int respawnCount;
	[ReadOnly]
	public int curExplorerCount;

	[Space(7.5f)]
	public int startExplorerCount;
	public Vector2Int ExplorerCountRange;

	[Space(7.5f)]
	[Range(0f, 1f)]
	public float destroyPercent;
	public Vector2Int destroyCountRange; //한번에 최고 몇명까지 해고할지

	[Space(7.5f)]
	[Range(0f, 1f)]
	public float respawnPercent;
	public Vector2Int respawnCountRange;

	[Space(7.5f)]
	[Range(0f, 1f)]
	public float newDirPercent;
}


[CreateAssetMenu(fileName = "IsaacRoomOption", menuName = "Scriptable Object/RoomOption/Isaac")]
public class RoomOption_Isaac : RoomOption
{

}


