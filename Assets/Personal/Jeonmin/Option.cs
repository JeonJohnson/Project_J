using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;
using System;
using System.IO;
using System.Reflection;

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
    public Options editedOptions;
    private string settingsFilePath = "Assets/Resources/Data/settings.ini";

    private void OnEnable()
    {
        //InitResolutionSettings();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SaveSettingsToFile();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            LoadSettingsFromFile();
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            editedOptions = options;
            InitResolutionSettings();
            view.InitSettingView(resolutions, editedOptions);
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
            //설정 값을 ini 파일로 저장
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
            //ini 파일에서 설정 값을 불러오기
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
        FieldInfo[] fields = typeof(Options).GetFields();

        using (StreamWriter sw = new StreamWriter(settingsFilePath, false))
        {
            sw.WriteLine("[Settings]");

            foreach (FieldInfo field in fields)
            {
                sw.WriteLine($"{field.Name}={field.GetValue(options)}");
            }
        }
    }
    private void ReadIniFile()
    {
        if (!File.Exists(settingsFilePath))
        {
            Debug.LogError("Settings file does not exist.");
            return;
        }

        Dictionary<string, string> settings = new Dictionary<string, string>();

        using (StreamReader sr = new StreamReader(settingsFilePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("[") || line.StartsWith(";"))
                    continue;

                string[] keyValue = line.Split('=');
                if (keyValue.Length == 2)
                {
                    settings[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
        }
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
