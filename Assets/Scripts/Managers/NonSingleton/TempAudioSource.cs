using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAudioSource : MonoBehaviour
{
	AudioSource AS;

	public void Awake()
	{
		AS = GetComponent<AudioSource>();
	}

	public void Update()
	{
		if (!AS.isPlaying)
		{
			gameObject.SetActive(false);
		}
	}
}
