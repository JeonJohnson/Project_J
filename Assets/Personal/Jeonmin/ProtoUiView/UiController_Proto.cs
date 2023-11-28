using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiController_Proto : Singleton<UiController_Proto>
{
    public UiView playerUiView;

    public Player player;

    private void Awake()
    {
        GameObject playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null) player = playerGo.GetComponent<Player>();
        else Debug.Log("Can't Find Player On Scene");
    }

    private void Update()
    {
        
    }

    public void PlayHitFeedback()
    {
        if (playerUiView != null)
        {
            playerUiView.PlayHitFeedback();
        }
    }
}
