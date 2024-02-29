using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;
using System;
using System.IO;

public enum KeyAction { Up, Down, Left, Right, Fire, Suck, Interact, End }
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class Option : MonoBehaviour
{
    [SerializeField] UIOption_View view;

    public int resoultionNum;
    private List<Resolution> resolutions = new List<Resolution>(); 
    public FullScreenMode screenMode;

    KeyCode[] defaltKeys = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.E };

    public Options options;
    public string settingsFilePath;

    private void OnEnable()
    {
        //InitResolutionSettings();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            InitVolumeSettings();
        }
    }

    #region Video
    public void InitResolutionSettings()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRateRatio.value == 60)
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }
        view.InitResolutionView(resolutions);
    }

    public void DropboxOptionChange(int i)
    {
        resoultionNum = i;
    }

    public void OnFullScreenButtonClick(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnApplyButtonClick()
    {
        Screen.SetResolution(resolutions[resoultionNum].width, resolutions[resoultionNum].height, screenMode);
        view.FullscreenToggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    #endregion

    #region Sound
    public void InitVolumeSettings()
    {
        SoundManager soundManager = SoundManager.Instance;
        view.InitVolumeView(0.5f, soundManager.EffectOffset, soundManager.BgmOffset);
    }

    public void OnMasterSliderChange(float f)
    {
        //읎
    }

    public void OnEffectSliderChange(float f)
    {
        SoundManager.Instance.EffectOffset = f;
    }

    public void OnBgmSliderChange(float f)
    {
        SoundManager.Instance.BgmOffset = f;
    }
    #endregion

    #region KeyBind
    public void InitKeyBindSetting()
    {
        for(int i = 0; i < (int)KeyAction.End; i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaltKeys[i]);
        }

        for (int i = 0; i < view.KeyBindTexts.Length;i++)
        {
            view.UpdateKeyBindText(i, defaltKeys[i].ToString());
        }
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;

        if(keyEvent.isKey)
        {
            KeySetting.keys[0] = keyEvent.keyCode;
        }
    }

    int key = -1;
    public void ChangeKey(int num)
    {
        key = num;
    }
    #endregion

    #region OptioniniSave
    public void SaveSettingsToFile()
    {
        try
        {
            // 설정 값을 ini 파일로 저장
            WriteIniFile();
            Debug.Log("Settings saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }
    }

    public void LoadSettingsFromFile()
    {
        try
        {
            // ini 파일에서 설정 값을 불러오기
            ReadIniFile();
            Debug.Log("Settings loaded successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to load settings: " + e.Message);
            SetDefaultSettings();
        }
    }

    private void WriteIniFile()
    {
        List<string> lines = new List<string>
        {
            "[Settings]",
            "IsFullScreen=" + options.isFullScreen.ToString(),
            "Language=" + options.language,
            "ResolutionWidth=" + options.resolution.width.ToString(),
            "ResolutionHeight=" + options.resolution.height.ToString(),
            "IsShadowOn=" + options.isShadowOn.ToString(),
            "MasterVolume=" + options.masterVolume.ToString(),
            "BgmVolume=" + options.bgmVolume.ToString(),
            "EffectVolume=" + options.effectVolume.ToString(),
            "LStickSensitivity=" + options.lstickSensitivity.ToString(),
            "RStickSensitivity=" + options.rstickSensitivity.ToString(),
            "LStickReverse=" + options.lstickReverse.ToString(),
            "RStickReverse=" + options.rstickReverse.ToString(),
            "HapticSensitivity=" + options.hapticSentivity.ToString(),
            "BossCutscene=" + options.bossCutscene.ToString(),
            "Subtitle=" + options.subtitle.ToString(),
            "SubTypeSpeed=" + options.subTypeSpeed.ToString(),
            "HUDSize=" + options.hudSize.ToString(),
            "HUDActivate=" + options.hudActivate.ToString(),
            "DamageVisual=" + options.damageVisual.ToString(),
            "CamShakeStrength=" + options.camShakeStrengh.ToString(),
            "CrossHairType=" + options.crossHairType.ToString(),
            "SuckAngleType=" + options.suckAngleType.ToString()
        };

        File.WriteAllLines(settingsFilePath, lines.ToArray());
    }

    private void ReadIniFile()
    {
        string[] lines = File.ReadAllLines(settingsFilePath);
        Dictionary<string, string> settings = new Dictionary<string, string>();

        foreach (string line in lines)
        {
            if (line.StartsWith(";") || line.StartsWith("["))
                continue;

            string[] keyValue = line.Split('=');
            if (keyValue.Length == 2)
            {
                settings[keyValue[0].Trim()] = keyValue[1].Trim();
            }
        }

        if (settings.ContainsKey("IsFullScreen"))
            bool.TryParse(settings["IsFullScreen"], out options.isFullScreen);
        if (settings.ContainsKey("Language"))
            options.language = settings["Language"];
        if (settings.ContainsKey("ResolutionWidth") && settings.ContainsKey("ResolutionHeight"))
            options.resolution = new Resolution() { width = int.Parse(settings["ResolutionWidth"]), height = int.Parse(settings["ResolutionHeight"]) };
        if (settings.ContainsKey("IsShadowOn"))
            bool.TryParse(settings["IsShadowOn"], out options.isShadowOn);
        if (settings.ContainsKey("MasterVolume"))
            float.TryParse(settings["MasterVolume"], out options.masterVolume);
        if (settings.ContainsKey("BgmVolume"))
            float.TryParse(settings["BgmVolume"], out options.bgmVolume);
        if (settings.ContainsKey("EffectVolume"))
            float.TryParse(settings["EffectVolume"], out options.effectVolume);
        if (settings.ContainsKey("LStickSensitivity"))
            float.TryParse(settings["LStickSensitivity"], out options.lstickSensitivity);
        if (settings.ContainsKey("RStickSensitivity"))
            float.TryParse(settings["RStickSensitivity"], out options.rstickSensitivity);
        if (settings.ContainsKey("LStickReverse"))
            bool.TryParse(settings["LStickReverse"], out options.lstickReverse);
        if (settings.ContainsKey("RStickReverse"))
            bool.TryParse(settings["RStickReverse"], out options.rstickReverse);
        if (settings.ContainsKey("HapticSensitivity"))
            bool.TryParse(settings["HapticSensitivity"], out options.hapticSentivity);
        if (settings.ContainsKey("BossCutscene"))
            bool.TryParse(settings["BossCutscene"], out options.bossCutscene);
        if (settings.ContainsKey("Subtitle"))
            bool.TryParse(settings["Subtitle"], out options.subtitle);
        if (settings.ContainsKey("SubTypeSpeed"))
            bool.TryParse(settings["SubTypeSpeed"], out options.subTypeSpeed);
        if (settings.ContainsKey("HUDSize"))
            float.TryParse(settings["HUDSize"], out options.hudSize);
        if (settings.ContainsKey("HUDActivate"))
            bool.TryParse(settings["HUDActivate"], out options.hudActivate);
        if (settings.ContainsKey("DamageVisual"))
            bool.TryParse(settings["DamageVisual"], out options.damageVisual);
        if (settings.ContainsKey("CamShakeStrength"))
            float.TryParse(settings["CamShakeStrength"], out options.camShakeStrengh);
        if (settings.ContainsKey("CrossHairType"))
            int.TryParse(settings["CrossHairType"], out options.crossHairType);
        if (settings.ContainsKey("SuckAngleType"))
            int.TryParse(settings["SuckAngleType"], out options.suckAngleType);
    }

    private void SetDefaultSettings()
    {
        options.isFullScreen = true;
        options.language = "English";
        options.resolution = new Resolution() { width = 1920, height = 1080 };
        options.isShadowOn = true;
        options.masterVolume = 0.5f;
        options.bgmVolume = 0.5f;
        options.effectVolume = 0.5f;
        options.lstickSensitivity = 1.0f;
        options.rstickSensitivity = 1.0f;
        options.lstickReverse = false;
        options.rstickReverse = false;
        options.hapticSentivity = true;
        options.bossCutscene = true;
        options.subtitle = true;
        options.subTypeSpeed = true;
        options.hudSize = 1.0f;
        options.hudActivate = true;
        options.damageVisual = true;
        options.camShakeStrengh = 0.5f;
        options.crossHairType = 0;
        options.suckAngleType = 0;
    }
    #endregion
}
