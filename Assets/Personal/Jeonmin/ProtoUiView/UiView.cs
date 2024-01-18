using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using DG.Tweening;

public class UiView : MonoBehaviour
{
    [Header("HpStatus")]
    [SerializeField] Transform hpHolderTr;
    [SerializeField] Transform armorHolderTr;
    [SerializeField] Image hpGaugeImg;
	//private List<Image> hpImages;
    private List<Image> armorImages;
    [SerializeField] TextMeshProUGUI lifeCountText;
    [SerializeField] MMF_Player hitFeedback;

    private string hpPrefabPath = "Sprites/UI/Prefabs/Heart_Image";
    private string hpSpritePath = "Sprites/SpriteSheet/OldVer_Objects_Sheet";
    private string hpEmptySpriteName = "HUD_Heart_Empty";
    private string hpFillSpriteName = "HUD_Heart_Fill";
    private Sprite[] hpSpriteFiles;

    [Header("StageStatus")]
    [SerializeField] TextMeshProUGUI leftEnemyCountText;

    [Header("WeaponStatus")]
    [SerializeField] Image consumeImage;
    [SerializeField] TextMeshProUGUI bulletCountText;
    [SerializeField] Image weaponImage;

    [Header("ItemStatus")]
    [SerializeField] Image activeItemImage;
    [SerializeField] Transform passiveItemHolder;
    private Image[] passiveItemImages;

    [SerializeField] Image activeItemGauge;

    [Header("BossHpBar")]
    public GameObject bossHpBarHolder;
    [SerializeField] Image[] bossHpBarImage;
    [SerializeField] MMF_Player bossHpBarFeedback;

    [Header("Result")]
    public GameObject resultHolder;
    public CanvasGroup resultCanvasGroup;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button resultButton;

    [Header("CrossHair")]
    public RectTransform crossHair;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //hpImages = new List<Image>(hpHolderTr.GetComponentsInChildren<Image>());
        armorImages = new List<Image>(armorHolderTr.GetComponentsInChildren<Image>());
        passiveItemImages = passiveItemHolder.GetComponentsInChildren<Image>();

        hpSpriteFiles = new Sprite[2];
        hpSpriteFiles[0] = Funcs.FindSprite(hpSpritePath, hpEmptySpriteName);
        hpSpriteFiles[1] = Funcs.FindSprite(hpSpritePath, hpFillSpriteName);
    }

    #region HpStatus
    public void UpdateLifeText(int _curLife)
    {
        lifeCountText.text = $"x{_curLife}";
    }

    public void UpdateHpImage(int _curHp, int _maxHp = 5)
    {
         hpGaugeImg.fillAmount = Mathf.Clamp((float)_curHp / (float)_maxHp,0f,1f); 
        


        //_curHp = Mathf.Clamp( _curHp, 0, hpImages.Count );

        //if( _maxHp != hpImages.Count)
        //{
        //    while (hpImages.Count < _maxHp)
        //    {
        //        hpImages.Add(Instantiate(Resources.Load("hpPrefabPath"), hpHolderTr).GetComponent<Image>());
        //    }
        //    while (hpImages.Count > _maxHp)
        //    {
        //        Image targetImage = hpImages[hpImages.Count - 1];
        //        hpImages.Remove(targetImage);
        //        Destroy(targetImage);
        //    }
        //}

        //for (int i = 0; i < hpImages.Count; i++)
        //{
        //    if (i < _curHp)
        //    {
        //        hpImages[i].sprite = hpSpriteFiles[1];
        //    }
        //    else
        //    {
        //        hpImages[i].sprite = hpSpriteFiles[0];
        //    }
        //}
    }

    public void UpdateShieldImage(int _curArmor)
    {
        for (int i = 0; i < armorImages.Count; i++)
        {
            if (i < _curArmor)
            {
                armorImages[i].gameObject.SetActive(true);
            }
            else
            {
                armorImages[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region StageStatus
    public void UpdateLeftEnemyCount(int _value)
    {
        leftEnemyCountText.text = $":{_value}";

    }
    #endregion

    #region WeaponStatus
    public void UpdateWeapon(Sprite sprite)
    {
        weaponImage.sprite = sprite;
    }

    public void UpdateWeaponConsume(float value)
    {
        consumeImage.fillAmount = value;
    }

    public void UpdateBulletCount(int cnt)
    {
        bulletCountText.text = $":{cnt}";
    }
    #endregion

    #region ItemStatus
    public void UpdateActiveItem(Sprite sprite)
    {
        activeItemImage.sprite = sprite;
        activeItemGauge.sprite = sprite;
    }

    public void UpdatePassiveItem(Sprite sprite, Item[] passiveItemSlot)
    {
        int i = 0;
        for (int j = 0; j < passiveItemImages.Length; j++)
        {
            passiveItemImages[j].gameObject.SetActive(false) ;
        }

        for (int k = 0; k < passiveItemSlot.Length; k++)
        {
            if (passiveItemSlot[k] !=null)
            {
                passiveItemImages[i].gameObject.SetActive(true);
                passiveItemImages[i].sprite = passiveItemSlot[k].item_sprite;
                i++;
            }
        }
    }

    public void UpdateActiveItemGauge(float value)
    {
        activeItemGauge.fillAmount = value;
    }
    #endregion

    public void UpdateBossHpBar(float value)
    {
        bossHpBarImage[0].fillAmount= value;
        bossHpBarImage[1].fillAmount = value;
        bossHpBarFeedback.PlayFeedbacks();
    }

    public void UpdateResult(bool isWin)
    {
        resultText.text = "";
        if (isWin)
        {
            resultText.DOText("WIN...?", 0.5f, true, ScrambleMode.Uppercase).SetDelay(1f);
        }
        else
        {
            resultText.DOText("YOU DIED", 0.5f, true, ScrambleMode.Uppercase).SetDelay(1f);
        }
        resultButton.onClick.RemoveAllListeners();

		//resultButton.onClick.AddListener(() => GameManager.Instance.LoadScene(2));
		resultButton.onClick.AddListener(OnclickResultButton);

		resultButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetDelay(3f);
    }

    private void OnclickResultButton()
    {
		SoundManager.Instance.PlaySound("UI_ClickButton", Camera.main.gameObject);
		IngameController.Instance.GotoTitleScene();

	}

    public void UpdateCrossHairAnchorPos(Vector3 screenPos, bool isActivate = true)
    {
        if (!isActivate) return;
        //crossHair.anchoredPosition = screenPos; 족버그;;
    }

    #region Feedbacks
    public void PlayHitFeedback()
    {
        hitFeedback.PlayFeedbacks();
    }
    #endregion
}
