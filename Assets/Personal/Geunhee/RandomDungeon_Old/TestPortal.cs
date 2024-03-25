using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestPortal : MonoBehaviour
{
	public List<Sprite> sprites;
	public List<Color> colors;

	public UnityEngine.Rendering.Universal.Light2D light;
	public SpriteRenderer sr;

	private void RandomSprite()
	{
		int randNum = Random.Range(0, sprites.Count);
		if (sr != null)
		{
			sr.sprite = sprites[randNum];
			
		}

		if (light != null)
		{
			light.color = colors[randNum];
		}
	}

	private void Awake()
	{
		RandomSprite();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			//여기서 StageManager한테 다음 방넘어가달라고하기
			Debug.Log("다음방");
			//StageManager.Instance.NextRoom();

			RandomSprite();

			this.gameObject.SetActive(false);
		}
	}



}
