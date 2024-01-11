using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomTriggerCollider : MonoBehaviour
{
    private bool isTrigged = false;
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        //if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        //StageManager.Instance.rooms[StageManager.Instance.curRoomIndex].ExitDoor(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !isTrigged) 
        //{
        //    Debug.Log(collision.transform.gameObject.name);
        //    isTrigged = true;
        //    StageManager.Instance.OnPlayerEnterNewRoom();
        //}
    }
}
