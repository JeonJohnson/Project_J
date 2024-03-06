using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIOption_View : MonoBehaviour
{
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

    [Header("KeyBind")]
    [SerializeField] GameObject keyBindGo;
    [SerializeField] TextMeshProUGUI[] keyBindTexts;
    public TextMeshProUGUI[] KeyBindTexts { get { return keyBindTexts; } }

    [Header("GamePlay")]
    [SerializeField] GameObject gamePlayGo;

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

    public void InitFullScreenView(bool boolean)
    {
        if(boolean)
            fullscreenToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        else
            fullscreenToggle.toggleTransition = Toggle.ToggleTransition.None;
    }

    public void InitShadowView(bool boolean)
    {
        if (boolean)
            shadowToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        else
            shadowToggle.toggleTransition = Toggle.ToggleTransition.None;
    }

    #region Sound

    public void InitVolumeView(float masterVolume, float effectVolume, float bgmVolume, bool isMute)
    {
        masterSoundSlider.value = masterVolume;
        seSlider.value = effectVolume;
        bgmSlider.value = bgmVolume;
        if(isMute)
        {
            muteToggle.toggleTransition = Toggle.ToggleTransition.Fade;
        }
        else
        {
            muteToggle.toggleTransition = Toggle.ToggleTransition.None;
        }
    }

    public void UpdateKeyBindText(int index,  string value) 
    {
        keyBindTexts[index].text = value;
    }
    #endregion
}
