using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

using TMPro;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    GameObject progressCricle;

    [SerializeField]
    TextMeshProUGUI loadingText;

    [SerializeField]
    Image loadingImage;

    int loadSceneNumber;

    [HideInInspector] public bool isSceneLoading = false;


    float loadingTextTimer = 0f;


    public UnityAction[] OnLoadingEvents;
    

    public void LoadScene(int sceneNumber)
    {
        int curScene = SceneManager.GetActiveScene().buildIndex;

        isSceneLoading = true;
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneNumber = sceneNumber;

        StartCoroutine(LoadSceneProcess(curScene,sceneNumber));
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) // arg0 = 불러와진 씬 
    {
        if (arg0.buildIndex == loadSceneNumber) // 즉 arg0 이름과 loadSceneName 이름이 같다면 씬이 불러와진거임
        {
            print(arg0.buildIndex);
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
            isSceneLoading = false;
            //GameManager.Instance.SceneCheck(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ChangeLoadingText()
    {
        loadingTextTimer += Time.deltaTime;

        if(loadingTextTimer < 0.3f)
        {
            loadingText.text = "Loading.";
        }
        else if(loadingTextTimer < 0.6f)
        {
            loadingText.text = "Loading..";
        }
        else if(loadingTextTimer < 0.9f)
        {
            loadingText.text = "Loading...";
        }
        else
        {
            loadingTextTimer = 0f;
        }
    }
    IEnumerator LoadSceneProcess(int curScene, int loadScene)
    {
        yield return StartCoroutine(Fade(true)); // 코루틴안에서 코루틴 실행시키면서 yield return으로 실행시키면 호출된 코루틴이 끝날때까지 기다리게 만들수 있음. 즉 Fade timer시간인 1초만큼 기다림

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneNumber); // 비동기로 씬을 불러옴
        op.allowSceneActivation = false; // 로딩 끝나자마자 자동으로 씬전환이 되지않게

        float timer = 0f;
        while(!op.isDone)
        {
            progressCricle.transform.Rotate(new Vector3(0,0f,-0.5f) * 360 * Time.deltaTime);
            ChangeLoadingText();

            //loadingImage.fillAmount = op.progress;
            loadingImage.fillAmount = op.progress * 0.8f;

            yield return null;

            if (op.progress >= 0.9f) 
            {
                //여기서 0.9는 90%가 불러와졌다는 뜻이 아니라
                //유니티 내부에서 씬 로드가 끝나면 0.9라는 수치를 띄워준다는거임.
                //근데 보통 그냥 저 op.progress의 값을 로딩 %로 사용하기 때문에 그렇게 보이는거.

                LoadSpecific(curScene,loadScene);

                ChangeLoadingText();

                //timer += Time.unscaledDeltaTime;
                loadingImage.fillAmount = 1;
                
                //if (timer > 1f)
                //{
                    op.allowSceneActivation = true;
                    yield break;
                //}
            }
        }
    }

    IEnumerator Fade(bool isFadeIn) // 로딩끝났을때 페이드인/아웃 효과
    {
        float timer = 0;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 4f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if(!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    private void LoadSpecific(int curScene, int loadScene)
    {
        if (curScene == 2 && loadScene == 3)
        {
            OnLoadingEvents[2].Invoke();
        }

    }

    private void Awake()
    {
        canvasGroup.alpha = 0f;

        OnLoadingEvents = new UnityAction[(int)SceneName.End];
    }

}
