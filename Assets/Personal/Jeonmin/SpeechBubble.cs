using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SpeechBurble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    public void Speech(string value, float size = 24f, float speed = 0.3f)
    {
        textMeshProUGUI.fontSizeMax = size;
        textMeshProUGUI.DOText(value, speed, true, ScrambleMode.Uppercase);
    }
}
