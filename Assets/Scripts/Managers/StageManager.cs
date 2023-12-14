using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{

	public List<Demo_Room> rooms;

	public int curRoomIndex;
	

	public int curMonsterCount; //정민아 이거랑 UI랑 연결하면 될거 같애
	public void OnMonsterDeath()
	{//정민아 몬스터 죽을 때 마다 호출해줘

		rooms[curRoomIndex].curMonsterCount -= 1;

		if (rooms[curRoomIndex].curMonsterCount <= 0)
		{ //정민아 여기가 그 방 클리어 조건이야

			if (rooms[curRoomIndex] == rooms[rooms.Count - 1])
			{ //이건 보스방 클리어 조건

			}
			else
			{ //일반방 클리어 조건
				rooms[curRoomIndex].ExitDoor(true);
				rooms[Mathf.Clamp(curRoomIndex + 1, 0, rooms.Count - 1)].EnteranceDoor(true);


			}
		}
	}

	public void OnPlayerEnterNewRoom()
	{
		//정민아 방 하나 클리어 한 이후
		//다음방 문(Door)에 투명 콜라이더 달아두고 그거 통과했을때 호출해주면 될거같애
		curRoomIndex = Mathf.Clamp(curRoomIndex + 1, 0, rooms.Count - 1);
		rooms[curRoomIndex].EnteranceDoor(false);
		curMonsterCount = rooms[curRoomIndex].curMonsterCount;
	}


	private void Awake()
	{
		
	}

	private void Start()
	{
		curRoomIndex = 0;
		

		foreach (var room in rooms)
		{
			room.RandomObjectGen();
			//첫방부터 시작, 플레이어부터 조건 체크시작 하기 때문에 플레이어 부터 만들꺼임.
		//걱정 ㄴㄴ
		}
	}


	private void Update()
	{
			
	}

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		base.OnSceneChanged(scene, mode);
	}
}
