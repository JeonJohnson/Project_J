using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEditor.Rendering;

using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.SceneManagement;

//맵(룸)생성,유지보수
//룸들 끼리의 흐름 관리
//레벨적인 부분 관리

//다시 타이틀씬으로 넘어가면 룸들 없애주기

public class StageManager : Singleton<StageManager>
{
	public GameObject portal;

	public int portalRange;

	public List<Room_Drunken> rooms;
	public Room_Drunken bossRoom;
	public Room_Drunken curRoom;
	[ReadOnly]
	public int curRoomIndex;

	public Data<int>[] enemyCount;
	public Data<int> killCount; // 단순 기록용 0207JM

	public List<GameObject> bullets;
	public List<Enemy_DeadBody> deadbody;


	//public Data<int> curMonsterCount; //정민아 이거랑 UI랑 연결하면 될거 같애
	//public void OnMonsterDeath()
	//{//정민아 몬스터 죽을 때 마다 호출해줘
	//	rooms[curRoomIndex].curMonsterCount -= 1;
	//       curMonsterCount.Value = rooms[curRoomIndex].curMonsterCount;

	//       if (rooms[curRoomIndex].curMonsterCount <= 0)
	//	{ //정민아 여기가 그 방 클리어 조건이야

	//		if (rooms[curRoomIndex] == rooms[rooms.Count - 1])
	//		{ //이건 보스방 클리어 조건
	//               if (IngameController.Instance.gameStatus == IngameController.GameStatus.Playing)
	//               {UiController_Proto.Instance.ShowResultWindow(true, true);}
	//      }
	//		else
	//		{ //일반방 클리어 조건
	//			rooms[curRoomIndex].ExitDoor(true);
	//			rooms[Mathf.Clamp(curRoomIndex + 1, 0, rooms.Count - 1)].EnteranceDoor(true);
	//		}
	//	}
	//}

	//public void OnPlayerEnterNewRoom()
	//{
	//	//정민아 방 하나 클리어 한 이후
	//	//다음방 문(Door)에 투명 콜라이더 달아두고 그거 통과했을때 호출해주면 될거같애
	//	curRoomIndex = Mathf.Clamp(curRoomIndex + 1, 0, rooms.Count - 1);
	//	rooms[curRoomIndex].EnteranceDoor(false);
	//	curMonsterCount.Value = rooms[curRoomIndex].curMonsterCount;

	//	rooms[curRoomIndex].RandomObjectGen();
	//	StartCoroutine(CameraBoundEffect());
	//   }

	//private IEnumerator CameraBoundEffect()
	//{
	//       IngameController.Instance.Player.aimController.cinemachineConfiner.m_BoundingVolume = rooms[curRoomIndex].cameraCollider;
	//       IngameController.Instance.Player.aimController.cinemachineConfiner.m_Damping = 2f;
	//	yield return new WaitForSeconds(2f);
	//       IngameController.Instance.Player.aimController.cinemachineConfiner.m_Damping = 0.08f;
	//       yield return new WaitForSeconds(1f);
	//       IngameController.Instance.Player.aimController.cinemachineConfiner.m_Damping = 0.01f;
	//       yield return new WaitForSeconds(1f);
	//       IngameController.Instance.Player.aimController.cinemachineConfiner.m_Damping = 0f;
	//       //IngameController.Instance.Player.aimController.cinemachineConfiner.m_Damping = 0f;
	//   }

	//private void Awake()
	//{S

	//}

	//private void Start()
	//{
	//	curRoomIndex = 0;
	//       rooms[curRoomIndex].RandomObjectGen();
	//	curMonsterCount = new Data<int>();
	//       curMonsterCount.onChange += UiController_Proto.Instance.playerHudView.UpdateLeftEnemyCount;


	//       if (rooms[curRoomIndex].curMonsterCount <= 0)
	//	{
	//		{ //일반방 클리어 조건
	//			rooms[curRoomIndex].ExitDoor(true);
	//			rooms[Mathf.Clamp(curRoomIndex + 1, 0, rooms.Count - 1)].EnteranceDoor(true);
	//		}
	//	}

	//	//foreach (var room in rooms)
	//	//{
	//	//	room.RandomObjectGen();
	//	//	//첫방부터 시작, 플레이어부터 조건 체크시작 하기 때문에 플레이어 부터 만들꺼임.
	//	////걱정 ㄴㄴ
	//	//}
	//}

	private void SetupRoom()
	{
		curRoomIndex = 0;

		for (int i = 1; i < rooms.Count; ++i)
		{
			rooms[i].gameObject.SetActive(false);
		}
		rooms.Add(bossRoom);
		
		enemyCount = new Data<int>[rooms.Count];
		//cleanObjs = new List<GameObject>[rooms.Count];
		for (int i = 0; i < rooms.Count; ++i)
		{
			enemyCount[i].Value = rooms[i].enemyPos.Count;
			//cleanObjs[i] = new List<GameObject>();
		}

		curRoom = rooms[curRoomIndex];
		IngameController.Instance.UpdateMinimapRenderCam(curRoom.centerPos, curRoom.size.y / 2f);

		UiController_Proto.Instance.playerHudView.UpdateLeftEnemyCount(enemyCount[curRoomIndex].Value);

	}

	public void NextRoom()
	{
		CleanObjects();

		rooms[curRoomIndex].gameObject.SetActive(false);

		curRoomIndex = curRoomIndex + 1 >= rooms.Count ? 0 : curRoomIndex + 1 ;

		curRoom = rooms[curRoomIndex];
		curRoom.gameObject.SetActive(true);

		Vector3 playerPos = curRoomIndex != rooms.Count - 1 ? curRoom.centerPos : new(3.5f, 7.5f, 0f);

		IngameController.Instance.Player.transform.position = playerPos;
		UiController_Proto.Instance.playerHudView.UpdateLeftEnemyCount(enemyCount[curRoomIndex].Value);
		IngameController.Instance.UpdateMinimapRenderCam(curRoom.centerPos, curRoom.size.y / 2f);
	}

	public void AddBullet(GameObject obj)
	{
		bullets.Add(obj);
	}

	public void AddDeadBody(Enemy_DeadBody script)
	{
		deadbody.Add(script);
	}

	public void CleanObjects()
	{
		for (int i = 0; i < bullets.Count; ++i)
		{
			Destroy(bullets[i]);
		}

		foreach (var item in deadbody)
		{
			item.Return();
		}

		bullets.Clear();
		deadbody.Clear();
	}


	public void DestoryRooms()
	{
		for (int i = 0; i < rooms.Count; ++i)
		{
			DestroyImmediate(rooms[i].gameObject);
        }
	}

	public void OnEnemyDeath()
	{
		--enemyCount[curRoomIndex].Value;
		killCount.Value++;
		UiController_Proto.Instance.playerHudView.UpdateLeftEnemyCount(enemyCount[curRoomIndex].Value);

		if (enemyCount[curRoomIndex].Value <= 0)
		{
			OnClearStage();
		}
	}

	public IEnumerator CreatePortalCor()
	{
		yield return new WaitForSecondsRealtime(1f);

		portal.SetActive(true);

		Vector3 playerPos = IngameController.Instance.Player.transform.position;
		Vector2Int playerIndex = curRoom.GetIndex(playerPos);

		List<Vector2Int> canIndexes = new	List<Vector2Int>();

		Vector2Int begin = new(playerIndex.x - portalRange, playerIndex.y - portalRange);
		Vector2Int end = new(playerIndex.x + portalRange, playerIndex.y + portalRange);
		
		for(int y = begin.y; y <= end.y; ++y)
		{ 
			for(int x =  begin.x; x <= end.x; ++x) 
			{
				if (x< 0 | y < 0 | x >= curRoom.tileStates.GetLength(0)| y >= curRoom.tileStates.GetLength(1))
				{
					continue;
				}

				if (playerIndex == new Vector2Int(x, y))
				{
					continue;
				}


				if (curRoom.tileStates[x,y] == tileGridState.Ground) 
				{
					canIndexes.Add(new(x, y));
				}
			}
		}

		int rand = UnityEngine.Random.Range(0, canIndexes.Count);

		portal.transform.position = curRoom.GetPos(canIndexes[rand]);
	}


	public void OnClearStage()
	{
		//플레이어 주위에 포탈 생성
		StartCoroutine(CreatePortalCor());
	}

	


	private void Awake()
	{
		rooms = new List<Room_Drunken>();
		
		DontDestroyOnLoad(gameObject);


		bullets = new List<GameObject>();
		deadbody = new List<Enemy_DeadBody>();

		killCount = new Data<int>();

    }

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	public void Release()
	{
		DestoryRooms();
		
	}

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		base.OnSceneChanged(scene, mode);

		if (scene.buildIndex == (int)SceneName.Ingame)
		{
			SetupRoom();
		}
	}
}
