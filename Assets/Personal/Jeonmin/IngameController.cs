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
    [SerializeField] GameObject playerPrefab;

    private void FindPlayer()
    {
        if(player == null)
        {
            GameObject playerGo = GameObject.Find("Player");
            if (playerGo != null) player = playerGo.GetComponent<Player>();
            else player = Instantiate(playerPrefab).GetComponent<Player>(); // player = PoolingManager.Instance.
            player.InitializePlayer();
        }
    }
}
