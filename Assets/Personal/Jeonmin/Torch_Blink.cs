using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Torch_Blink : MonoBehaviour
{
    public float globalBright = 1f;

    public float minIntensity = 1f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 2.0f;

    private Light2D light2D;
    private float targetIntensity;
    private float timeSinceLastUpdate = 0.0f;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;


            // 부드러운 보간을 위해 Lerp 사용
            light2D.intensity = globalBright * Mathf.Lerp(light2D.intensity, targetIntensity, flickerSpeed * timeSinceLastUpdate);

            // 일정 주기마다 새로운 목표값 설정
            if (Mathf.Abs(light2D.intensity - targetIntensity) < 0.01f)
            {
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            }

            // 누적된 시간 초기화
            timeSinceLastUpdate = 0.0f;
    }
}
