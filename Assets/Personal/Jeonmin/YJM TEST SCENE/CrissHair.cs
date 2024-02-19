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

    public float fadeDuration = 2f;  // ���̵� ���� �ð� ����

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
        // ���� ���콺 ��ġ ��������
        Vector3 mousePosition = Input.mousePosition;

        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);

        // ũ�ν���� ��ġ ����
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
            // �������� 1 �̻��̸� ������ ���������� ����
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);
            SetGaugeAlpha(alpha);

            if (fadeTimer >= fadeDuration)
            {
                // ���������� �Ϸ�Ǹ� �ʱ�ȭ
                isFadingOut = true;
                fadeTimer = 0f;
            }
        }
        else if (value < 1f && isFadingOut)
        {
            // �������� 1 ���Ϸ� �������� ������ ��Ÿ���� ����
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            SetGaugeAlpha(alpha);

            if (fadeTimer >= fadeDuration)
            {
                // ��Ÿ���Ⱑ �Ϸ�Ǹ� �ʱ�ȭ
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
