using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameController : Singleton<IngameController>
{
    public Player player;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>(); // �ٸ����µ� ������ ���� �̱��� ������ ���ٰ����� ��θ� ����� �װ� �����ϴ� ������ ���� �����
        player.InitializePlayer();
    }
}
