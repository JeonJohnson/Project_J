using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam2DZoom : MonoBehaviour
{
	public Camera cam;
	public float zoomSpd = 5f;
	private void Zoom()
	{
		float scrollVal = Input.GetAxis("Mouse ScrollWheel") * zoomSpd;

		cam.orthographicSize += scrollVal;
	}

	private void Awake()
	{
		//cam ??= GetComponent<Camera>();
		if(!cam) cam = GetComponent<Camera>();
		//cam ??= Camera.main;
		if (!cam) cam = Camera.main;
	}
	// Update is called once per frame
	void Update()
    {
		Zoom();
    }
}
