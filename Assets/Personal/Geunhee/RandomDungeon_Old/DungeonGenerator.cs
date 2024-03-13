using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


using AYellowpaper;
using AYellowpaper.SerializedCollections;
using MoreMountains.Tools;

//public enum tileGridState
//{
//	//땅인지 바닥인지 파악하기 위해
//	None,
//	Ground,
//	Wall
//}




public enum GenerateType
{ 
	Drunken,
	
	End
}

public class DungeonGenerator : MonoBehaviour
{
    public GameObject TilemapReferPrefab;


	[Space(10f)]
	public SerializedDictionary<Stage,List<Room>> stageDic;




	private void Awake()

	{
		
	}
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
