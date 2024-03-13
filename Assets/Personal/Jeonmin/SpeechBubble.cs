using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    private GameObject owner;
    public bool isButtonShow;
    private RectTransform rt;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Image interactableIcon;

    public void Init(GameObject _owner)
    {
        owner = _owner;
        this.transform.parent = UiController_Proto.Instance.playerHudView.transform;
        rt = GetComponent<RectTransform>(); 
        interactableIcon.gameObject.SetActive(false);
    }

    public void Speech(string value, float size = 24f, float speed = 0.1f, bool showButton = false)
    {
        textMeshProUGUI.fontSizeMax = size;
        isButtonShow = showButton;
        textMeshProUGUI.text = "";
        float typeSpeed = (float)value.Length * speed; 
        textMeshProUGUI.DOText(value, typeSpeed, true, ScrambleMode.None).OnComplete(ShowButton);
    }

    public void ShowButton()
    {
        if (!isButtonShow) return;
        interactableIcon.gameObject.SetActive(true);
    }

    private Vector3 addValue = new Vector3(0f, 1f, 0f);

    private void Update()
    {
        rt.position = Camera.main.WorldToScreenPoint(owner.transform.position + addValue);
    }
}
