using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//인게임 씬의 전반적인 세팅 관련

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

    public void SetMinimapRenderCam()
    {
        minimapRenderCam = GameObject.FindWithTag("MinimapRenderCam").GetComponent<Camera>();
    }

    public void UpdateMinimapRenderCam(Vector2 pos, float size)
    {
        minimapRenderCam.transform.position = new Vector3(pos.x, pos.y, -10f);
        minimapRenderCam.orthographicSize = size;
    }

	private void Awake()
	{
		
	}

	private void Start()
	{
        FindPlayer();
        player.transform.position = StageManager.Instance.curRoom.centerPos;
	   
        //MainGame Scene에서 바로 시작하는 테스트를 위해서 
        //맵 StageManager이나 맵 없으면 여기서 만들어주자구

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
}
