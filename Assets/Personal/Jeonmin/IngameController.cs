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
            else { Debug.Log("������ �÷��̾ ã���� �����ϴ�"); } // player = PoolingManager.Instance.
        }
    }

    public GameObject SpawnPlayer(Vector3 position)
    {
        GameObject playerGo = null ;
        GameObject obj = Resources.Load<GameObject>("Characters/Player/Player");
        if (obj != null) { playerGo = Instantiate(obj, position, Quaternion.identity); }
        else { Debug.Log("�÷��̾� �������� ��ã��"); }
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
