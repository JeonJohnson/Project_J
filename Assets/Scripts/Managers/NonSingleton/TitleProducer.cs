using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

public class TitleProducer : MonoBehaviour
{
    //역할은 매니저이긴한데
    //sigleton 아닌 애들 producer

    public Button newGameBtn;
    public Button loadGameBtn;
    public Button settingBtn;
    public Button exitBtn;


	private void Awake()
	{
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

	}

	void Start()
    {
        newGameBtn.onClick.AddListener(() => GameManager.Instance.LoadNextScene());
        exitBtn.onClick.AddListener(() => GameManager.Instance.ExitApp());
    }

    void Update()
    {
        
    }
}
