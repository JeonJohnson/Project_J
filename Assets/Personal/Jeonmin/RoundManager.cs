using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : Singleton<RoundManager>
{
    public int curRoomIndex;
    public int pastRoomIndex;
    private RoomData curRoomData;
    private RoomData pastRoomData;

    public List<Enemy> leftEnemyList;

    private void EnterRoom(RoomData roomData)
    {
        pastRoomIndex = curRoomIndex;
        curRoomIndex = roomData.roomIndex;
        pastRoomData = curRoomData;
        curRoomData = roomData;

        IngameController.Instance.Player.aimController.cinemachineConfiner.m_BoundingVolume = roomData.cameraCollider;
        if(!roomData.isCleared) roomData.Init();
    }

    private void CheckRoomStatus()
    {
        if(curRoomData == null) return;

        if(!curRoomData.isCleared)
        {
            if (leftEnemyList.Count <= 0) curRoomData.isCleared = true;
        }
    }

    private void Update()
    {
        
    }
}


public class RoomData
{
    public struct EnemyData
    {
        public string name;
        public Vector3 position;
    }

    public enum RoomTheme
    {
        Prison,
        Cave,
        End
    }

    public EnemyData[] EnemyDatas{ set { enemyDatas = value; } }
    private EnemyData[] enemyDatas;

    public int roomIndex;
    public Collider cameraCollider;
    public RoomTheme roomTheme;
    public bool isCleared;

    public System.Action OnEnterAction; 

    public void Init() // 룸 첨 들어갔을때만 실행
    {
        for (int i = 0; i < enemyDatas.Length; i++)
        {
            //GameObject enemy = PoolingManager.Insatnce 풀링으로 적 생성
            // enemy.transform.position = roomData.enemyDatas[i].position;
            //enemyList.Add(enemy.GetComponent<Enemy>());
        }
        OnEnterAction?.Invoke();
    }
}

