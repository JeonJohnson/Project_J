using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG;
using DG.Tweening;

public class IntroProducer : MonoBehaviour
{
	//역할은 매니저이긴한데
	//sigleton 아닌 애들 producerㅋㅋ
	//별 의미는 없고 걍 잼잖씀~
    

    public Image fade;
    public Image teamLogo;

    [Space(10f)]

    public float fadeInTime;
    public float logoTime;
    public float fadeOutTime;


    Sequence introSeq ;

    public void SettingSequence()
    {
        
    }

	void Awake()
	{
        fade.color = Color.black;
        introSeq = DOTween.Sequence();
	}


	void Start()
    {
		introSeq.Append(fade.DOFade(0f, fadeInTime)).AppendInterval(logoTime)
            .Append(fade.DOFade(1f, fadeOutTime)).AppendCallback(() => GameManager.Instance.LoadScene(2)).Play();
	}
}
