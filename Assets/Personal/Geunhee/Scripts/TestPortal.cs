using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestPortal : MonoBehaviour
{


	private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
        { 
            //여기서 StageManager한테 다음 방넘어가달라고하기
        
        }
	}
}
