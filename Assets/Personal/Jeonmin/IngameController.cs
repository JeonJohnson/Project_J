using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Debug = Potato.Debug;

//인게임 씬의
//전반적인 세팅
//승리, 패배 등 조건


public class IngameController : Singleton<IngameController>
{
    [SerializeField]
    private Camera minimapRenderCam;

    private Player player;
    public Player Player 
    { 
        get 
        {
            if (player != null) return player;
            else 
            {
                FindPlayer();
                return player;
            }
        } 
    }

    private bool isWindowActivated = false;
    private bool isRuneWindowActivated = false;
    private bool isShopWindowActivated = false;
    public enum GameStatus
    {
        Playing,
        Win,
        Lose
    }

    public GameStatus gameStatus = GameStatus.Playing;




    public Stage stage;



	#region About_Player
	private void FindPlayer()
    {
        if(player == null)
        {
            GameObject playerGo = GameObject.Find("Player");
            if (playerGo != null)
            { 
                player = playerGo.GetComponent<Player>();
                player.InitializePlayer();
            }
            else { Debug.Log("씬에서 플레이어를 찾을수 없습니다"); } // player = PoolingManager.Instance.
        }
    }

    public GameObject SpawnPlayer(Vector3 position)
    {
        GameObject playerGo = null ;
        GameObject obj = Resources.Load<GameObject>("Characters/Player/Player");
        if (obj != null) { playerGo = Instantiate(obj, position, Quaternion.identity); }
        else { Debug.Log("플레이어 폴더에서 못찾음"); }
        return playerGo;
    }
    #endregion

    private void FindStage()
    {
        if (stage == null)
        {

			var Obj = GameObject.FindWithTag("Stage");

            if (Obj == null)
            {
                Obj = GameObject.Find("Stage");
            }

			if (Obj != null)
            { 
                stage = Obj.GetComponent<Stage>();
            }
		}
	}

    //public void AddPoolingObject(GameObject obj)
    //{ 
        
    
    //}

    public void SpawnEnemies()
    {
        var rooms = stage.rooms;

        foreach (var room in rooms)
        {
            if (room.Spec.roomType == RoomType.Normal)
            {
                foreach (var pos in room.enemyPos)
                {
                    int rand = UnityEngine.Random.Range(0, 2);
                    Enemy enemy = SpawnEnemy(rand, pos, room);
                    room.AddPoolingObj(enemy.gameObject);
                }
            }
            else if (room.Spec.roomType == RoomType.Boss)
            {
				Enemy enemy = SpawnEnemy(2, room.Spec.centerPos, room);
				room.AddPoolingObj(enemy.gameObject);
			}
        }
    }

	public Enemy SpawnEnemy(int type, Vector3 pos, Room room)
    {
        string enemyObjName = type == 0 ? "Enemy_Rifle" : "Enemy_Shotgun";

        if (type == 2)
        {
            enemyObjName = "Boss_Demo";
        }

        var obj = PoolingManager.Instance.LentalObj(enemyObjName, 1);
        if (room == null)
        {
            room = stage.curRoom;
        }
        //obj.transform.SetParent(room.poolingTr);
        var script = obj.GetComponent<Enemy>();

        if (script)
        {
            script.agent.enabled = false;
            obj.transform.position = pos;
            script.agent.enabled = true;

            script.status.dontTriggerLeftInfo = false;
        }

        return script;
	}


	#region Minimap
	public void SetMinimapRenderCam()
    {
        minimapRenderCam ??= GameObject.FindWithTag("MinimapRenderCam").GetComponent<Camera>();
    }

    public void UpdateMinimapRenderCam(Vector2 pos, float size)
    {
        minimapRenderCam.transform.position = new Vector3(pos.x, pos.y, -10f);
        minimapRenderCam.orthographicSize = size;
    }
	#endregion


	//private void CheckMap()
	//{
	//    GameObject stageMgr = GameObject.Find("StageManager");

	//    if (stageMgr == null)
	//    {
	//        GameObject prefab = Resources.Load("Managers/StageManager") as GameObject;
	//        stageMgr = Instantiate(prefab);

	//        //나중에 만드는디
	//        //DungeonGenerator에서 직접적으로 
	//        //GameManager에 이벤트 연결하는게 아니라

	//        //제네레이터는 방 만들어고 가지고있는 기능만 수행하고
	//        //Stage매니저는 이를 GameManager의 이벤트와 연결하고
	//        //할 수 있도록 하기
	//    }
	//}



	public void GameOver(bool win)
	{
		if (IngameController.Instance.gameStatus == IngameController.GameStatus.Playing)
		{
            gameStatus = win ? GameStatus.Win : GameStatus.Lose;

            string soundName = win ? "Win" : "Lose";

            player.GetComponent<BoxCollider2D>().enabled = false;

			UiController_Proto.Instance.ShowResultWindow(true, win);
		}
	}

    public void GotoTitleScene()
    {
        GameManager.Instance.LoadScene((int)SceneName.Title);
    }

    public void GotoMainMenu()
    {
        GameManager.Instance.LoadScene((int)SceneName.MainMenu);
    }


	public void ResetAllWindow()
	{
		isWindowActivated = false;
		isRuneWindowActivated = false;
		isShopWindowActivated = false;

		UiController_Proto.Instance.ShowDetailStatusWindow(isWindowActivated);
		UiController_Proto.Instance.ShowRuneWindow(isRuneWindowActivated);
		UiController_Proto.Instance.ShowShopWindow(isShopWindowActivated);

		Cursor.visible = true;
	}
	private void Awake()
    {
        Initailize(false);
    }

    private void Start()
	{
	    FindPlayer();
        
        SetMinimapRenderCam();

        SpawnEnemies();

        //PoolingManager.Instance.OnLental 
	}
	private void Update()
    {

    }

 

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		base.OnSceneChanged(scene, mode);


        
        //FindStage();
	}

	private void OnDestroy()
	{
		//PoolingManager.Instance.OnLental -= 
	}
}


