using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

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
    public enum GameStatus
    {
        Playing,
        Win,
        Lose
    }

    public GameStatus gameStatus = GameStatus.Playing;

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


    public Enemy SpawnEnemy(int type, Vector3 pos)
    {
        string enemyObjName = type == 0 ? "Enemy_Rifle" : "Enemy_Shotgun";

        if (type == 2)
        {
            enemyObjName = "Boss_Demo";
        }


        var obj = PoolingManager.Instance.LentalObj(enemyObjName, 1);
        var script = obj.GetComponent<Enemy>();

        if (script)
        {
            script.agent.enabled = false;
            obj.transform.position = pos;
            script.agent.enabled = true;
        }


        return script;
	}


    public void SetMinimapRenderCam()
    {
        minimapRenderCam ??= GameObject.FindWithTag("MinimapRenderCam").GetComponent<Camera>();
    }

    public void UpdateMinimapRenderCam(Vector2 pos, float size)
    {
        minimapRenderCam.transform.position = new Vector3(pos.x, pos.y, -10f);
        minimapRenderCam.orthographicSize = size;
    }


    private void CheckMap()
    {
        GameObject stageMgr = GameObject.Find("StageManager");
        
        if (stageMgr == null)
        {
            GameObject prefab = Resources.Load("Managers/StageManager") as GameObject;
            stageMgr = Instantiate(prefab);

            //나중에 만드는디
            //DungeonGenerator에서 직접적으로 
            //GameManager에 이벤트 연결하는게 아니라
            
            //제네레이터는 방 만들어고 가지고있는 기능만 수행하고
            //Stage매니저는 이를 GameManager의 이벤트와 연결하고
            //할 수 있도록 하기
        }
    }

    private void EnemySpawn()
    {
        //추후에 아예 로딩할때 IngameController랑 StageMAnager랑 다 만들어서
        //플레이어 만들고 뭐하고 하고 하면 될 듯.

        for (int i = 0; i < StageManager.Instance.rooms.Count - 1; ++i)
        {
            var curRoom = StageManager.Instance.rooms[i];
			var enemyPosList = StageManager.Instance.rooms[i].enemyPos;

            foreach (var enemyPos in enemyPosList)
            {
                int enemyType = Random.Range(0, 2);
                Enemy enemy = SpawnEnemy(enemyType, enemyPos);
                enemy.transform.SetParent(StageManager.Instance.rooms[i].transform);
			}
		}

		Enemy Boss = SpawnEnemy(2, StageManager.Instance.bossRoom.centerPos);
		Boss.transform.SetParent(StageManager.Instance.bossRoom.transform);
    }

	public void GameOver(bool win)
	{
		if (IngameController.Instance.gameStatus == IngameController.GameStatus.Playing)
		{
            gameStatus = win ? GameStatus.Win : GameStatus.Lose;

            player.GetComponent<BoxCollider2D>().enabled = false;

			UiController_Proto.Instance.ShowResultWindow(true, win);
		}
	}

    public void GotoTitleScene()
    {
        StageManager.Instance.Release();
        Destroy(StageManager.Instance.gameObject);

        GameManager.Instance.LoadScene((int)SceneName.Title);
    }


	private void Awake()
	{
        
	}

	private void Start()
	{

		FindPlayer();
        player.transform.position = StageManager.Instance.curRoom.centerPos;

        SetMinimapRenderCam();
		//MainGame Scene에서 바로 시작하는 테스트를 위해서 
		//맵 StageManager이나 맵 없으면 여기서 만들어주자구

		EnemySpawn();

	}
	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isWindowActivated = !isWindowActivated;
            player.LockPlayer(isWindowActivated);
            UiController_Proto.Instance.ShowDetailStatusWindow(isWindowActivated);
        }
    }

	public override void OnSceneChanged(Scene scene, LoadSceneMode mode)
	{
		base.OnSceneChanged(scene, mode);
	}
}


