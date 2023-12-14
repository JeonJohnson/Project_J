using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{

	public List<Demo_Room> rooms;

	public Demo_Room curRoom;



	private void Awake()
	{
		
	}

	private void Start()
	{
		curRoom = rooms[0];
		//curRoom에서 
	}


	private void Update()
	{
			
	}

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		base.OnSceneChanged(scene, mode);
	}
}
