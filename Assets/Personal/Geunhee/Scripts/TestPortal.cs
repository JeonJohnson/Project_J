using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestPortal : MonoBehaviour
{

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			//여기서 StageManager한테 다음 방넘어가달라고하기
			Debug.Log("다음방");
			StageManager.Instance.NextRoom();
		}
	}



}
