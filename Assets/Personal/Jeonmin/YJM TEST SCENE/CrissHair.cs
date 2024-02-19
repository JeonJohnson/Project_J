using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CrissHair : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform crosshair;
    public Image suckGaugeImage; 
    private Player player;
    public MMF_Player suckGaugeFullFeedback;

    public Color emptyCol;
    public Color fullCol;

    public float fadeDuration = 2f;  // 페이드 지속 시간 설정

    private float fadeTimer = 0f;
    private bool isFadingOut = false;

    private bool isFulled = false;

    private void Awake()
    {
        crosshair = GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        player = IngameController.Instance.Player;
        if (player.curWeapon.suctionStat.curSuctionRatio != null)
            player.curWeapon.suctionStat.curSuctionRatio.onChange += UpdateSuckGauge;
    }

    void Update()
    {
        // 현재 마우스 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;

        // 마우스 위치를 월드 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);

        // 크로스헤어 위치 설정
        crosshair.localPosition = localPoint;

        if(!isFulled)
        {
            if(player.curWeapon.suctionStat.curSuctionRatio.Value >= 1f)
            {
                suckGaugeFullFeedback?.PlayFeedbacks();
                isFulled = true;
            }
        }
        else
        {
            if (player.curWeapon.suctionStat.curSuctionRatio.Value < 0.95f)
            {
                isFulled = false;
            }
        }

        UpdateGaugeVisibility(player.curWeapon.suctionStat.curSuctionRatio.Value);
    }

    private void UpdateSuckGauge(float value)
    {
        suckGaugeImage.fillAmount = value;
    }

    private float gaugeAlhpa = 1f;
    private void UpdateGaugeVisibility(float value)
    {
        Color gaugeColor = Color.Lerp(emptyCol, fullCol, value);
        gaugeColor.a = gaugeAlhpa;
        suckGaugeImage.color = gaugeColor;

        if (value >= 1f && !isFadingOut)
        {
            // 게이지가 1 이상이면 서서히 투명해지기 시작
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            SetGaugeAlpha(alpha);

            if (fadeTimer >= fadeDuration)
            {
                // 투명해지가 완료되면 초기화
                isFadingOut = true;
                fadeTimer = 0f;
            }
        }
        else if (value < 1f && isFadingOut)
        {
            // 게이지가 1 이하로 내려가면 서서히 나타나기 시작
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            SetGaugeAlpha(alpha);

            if (fadeTimer >= fadeDuration)
            {
                // 나타나기가 완료되면 초기화
                isFadingOut = false;
                fadeTimer = 0f;
            }
        }
    }

    private void SetGaugeAlpha(float alpha)
    {
        Color gaugeColor = suckGaugeImage.color;
        gaugeColor.a = alpha;
        gaugeAlhpa = gaugeColor.a;
        suckGaugeImage.color = gaugeColor;
    }

    private void OnDisable()
    {
        if (player.curWeapon.suctionStat.curSuctionRatio != null)
            player.curWeapon.suctionStat.curSuctionRatio.onChange -= UpdateSuckGauge;
    }

}
