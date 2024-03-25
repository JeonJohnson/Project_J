using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
	

	[Space(7.5f)]
	[SerializeField]
	public NewExplorerPos ExplorerSpawnOption;

	//[Space(10f)]
	//[HideInInspector]
	//public List<Explorer> explorers;
	//[ReadOnly]
	//public int destoryCount;
	//[ReadOnly]
	//public int respawnCount;
	//[ReadOnly]
	//public int curExplorerCount;

	[Space(7.5f)]
	[Range(0f, 1f)]
	public float newDirPercent;

	[Space(10f)]
	public int startExplorerCount;
	public Vector2Int ExplorerCountRange;

	[Space(10f)]
	[Range(0f, 1f)]
	public float respawnPercent;
	public Vector2Int respawnCountRange;

	[Space(10f)]
	[Range(0f, 1f)]
	public float destroyPercent;
	public Vector2Int destroyCountRange; //한번에 최고 몇명까지 해고할지




}