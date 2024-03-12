using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Structs;

public class UIOption_View : View
{
    public Option owner;

    [Header("Vidio")]
    [SerializeField] GameObject videoGo;
    [SerializeField] TMP_Dropdown languageDropDown;
    [SerializeField] TMP_Dropdown resolutionDropDown;
    public TMP_Dropdown ResolutionDropDown { set { resolutionDropDown = value; } }
    [SerializeField] Toggle fullscreenToggle;
    public Toggle FullscreenToggle { get { return fullscreenToggle; } }

    [SerializeField] Toggle shadowToggle;

    [SerializeField] Slider cameraShakeSlider;

    [Header("Sound")]
    [SerializeField] GameObject soundGo;
    [SerializeField] Slider masterSoundSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Toggle muteToggle;

    [Header("KeySetting")]
    [SerializeField] GameObject keyBindGo;
    [SerializeField] Slider mouseSensSlider;
    [SerializeField] Toggle mouseFlipToggle;
    [SerializeField] Slider lStickSensSlider;
    [SerializeField] Toggle lStickFlipToggle;
    [SerializeField] Slider rStickSensSlider;
    [SerializeField] Toggle rStickFlipToggle;
    [SerializeField] Slider stickBibSlider;

    [SerializeField] TextMeshProUGUI[] keyBindTexts;
    public TextMeshProUGUI[] KeyBindTexts { get { return keyBindTexts; } }

    [Header("GamePlay")]
    [SerializeField] GameObject gamePlayGo;
    [SerializeField] TextMeshProUGUI gamePlayTimeText;

    #region 
    public void InitResolutionView(List<Resolution> resolutions)
    {
        resolutionDropDown.options.Clear();
        int optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " + item.refreshRateRatio + "hz";
            resolutionDropDown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropDown.value = optionNum;
            optionNum++;
        }
        resolutionDropDown.RefreshShownValue();
    }

    public void InitToggle(Toggle toggle, bool boolean)
    {
        toggle.isOn = boolean;
    }

    public void InitSlider(Slider slider, float amount)
    {
        slider.value = amount;
    }

    public void UpdateKeyBindText(int index,  string value) 
    {
        keyBindTexts[index].text = value;
    }
    #endregion

    public void InitSettingView(List<Resolution> resolutions, Options options)
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRateRatio.value == 60)
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }
        InitResolutionView(resolutions);
        InitToggle(fullscreenToggle, options.isFullScreen);
        InitToggle(shadowToggle, options.isShadowOn);

        InitSlider(masterSoundSlider, options.masterVolume);
        InitSlider(seSlider, options.effectVolume);
        InitSlider(bgmSlider, options.bgmVolume);
        InitToggle(muteToggle, options.isMute);

        InitSlider(mouseSensSlider, options.mouseSensitivity);
        InitToggle(mouseFlipToggle, options.isMouseFlip);
        InitSlider(lStickSensSlider, options.lstickSensitivity);
        InitToggle(lStickFlipToggle, options.lstickReverse);
        Debug.Log(options.lstickReverse + "юс");
        InitSlider(rStickSensSlider, options.rstickSensitivity);
        InitToggle(rStickFlipToggle, options.rstickReverse);
    }

    private void Update()
    {
        gamePlayTimeText.text = 00000.ToString();
    }
}
