using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : Singleton<IngameController>
{
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
