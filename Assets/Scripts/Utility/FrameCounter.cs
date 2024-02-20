using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{
    public bool ReleaseOff;

    [Space(10f)]
    [Header("OnGUI Text Setting")]
    public bool On = true;
    [SerializeField]
    private int fontSize = 40;
    [SerializeField]
    private Color color = Color.green;
    [SerializeField]
    private float offset_X = 5f, offset_Y = 5f;

    private int fpsCount;
	private int fpsTempCount;
	private float fpsUnscaledTime = 0f;
    private string fpsText;


    

	private void Update()
	{
        if (!On)
        {
            return;
        }

        fpsUnscaledTime += Time.unscaledDeltaTime;
        ++fpsTempCount;

        if (fpsUnscaledTime >= 1f)
        {
            fpsCount = fpsTempCount;
            fpsUnscaledTime = 0f;
            fpsTempCount = 0;
        }
    }

	private void OnGUI()
	{

        if (!On)
        {
            return;
        }
        

        fpsText = string.Format("FPS : {0:N1}", fpsCount);

        Rect position = new Rect(offset_X, offset_Y, Screen.width, Screen.height);

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = color;

        GUI.Label(position, fpsText, style);

    }
}
