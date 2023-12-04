using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : Singleton<IngameController>
{
    public Player player;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>(); // 다른에셋들 참고해 보니 싱글톤 등으로 접근가능한 경로를 만든뒤 그걸 참조하는 형식을 많이 사용함
        player.InitializePlayer();
    }
}
