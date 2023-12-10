using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
	public int index;
	public Rect rect;
	public SpriteRenderer sr;

	public void SetRect()
	{
		rect.size = transform.localScale;
		rect.center = transform.position;
	}
	private void Awake()
	{
		if (!sr)
		{ 
			sr = GetComponent<SpriteRenderer>();
		}
	}
}
