using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public struct RoomStatus
{ 


}

//1. BSP (이진 공간 분할법) 알고리즘을 이용하여 특정 조건까지 나누기.
    //BSP -> 특정 공간을 재귀 함수로 나눌때 트리 구조를 만들어 재귀를 돌림.
//2. 나눠진 공간내에서 여백을 주고 실질적인 방 사이즈를 정해줌.
//3. 만들어진 방들에서 시작 지점 정해주기
//4. 들로네 삼각 분할 알고리즘을 통하여 인접한 방들 연결하기
//5. 최소 스패닝 트리를 구상하여 통로 정리
//6. 직각,수직의 선으로 정리하기

public class RoomGenerator : MonoBehaviour
{
    public int minRoomCount, maxRoomCount;
    
    //Dictionary<>
    
    //[Space(10f)]
    //public Vector2Int roomTileCount_X, roomTileCount_Y;

    //1. 일단 원하는 개수까지 나눠보기
    //condition 01. 가로로 나눌지 세로로 나눌지
    //condition 02. 길이의 비율값 3~7 비율까지

    //public void DivideSpace()
    //{ 
        
    //}









}
