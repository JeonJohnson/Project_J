using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Structs;
using System;
using System.IO;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime;
using DG.Tweening.Plugins.Core.PathCore;

public enum KeyAction { Up, Down, Left, Right, Fire, Suck, Interact, End }
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class Option : MonoBehaviour
{
    [SerializeField] UIOption_View view;

    public int resoultionNum;
    private List<Resolution> resolutions = new List<Resolution>(); 
    public FullScreenMode screenMode;

    KeyCode[] defaltKeys = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.E };

    public Options options = new Options();
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

        if (Input.GetKeyDown(KeyCode.U))
        {
            SetDefaultSettings();
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
        Dictionary<string, string> settings = new Dictionary<string, string>();

        if (File.Exists(settingsFilePath))
        {
            string[] lines = File.ReadAllLines(settingsFilePath);

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
        }
        else
        {
            Console.WriteLine("Settings file not found.");
        }

        string bval;
        if (settings.TryGetValue("lstickReverse", out bval))
        {
            Debug.Log("lstickReverse 값은" + bval);
        }

        Type optionsType = typeof(Options);
        foreach (KeyValuePair<string, string> entry in settings)
        {
            string fieldName = entry.Key; // 설정 파일의 변수명
            string value = entry.Value; // 설정 값

            // Options 구조체의 해당 필드를 찾음
            FieldInfo field = optionsType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                // 필드의 타입에 따라 값을 파싱하여 할당

                object parsedValue = ParseValue(field.FieldType, value);

                Debug.Log(parsedValue);

                if (parsedValue != null)
                {
                    field.SetValue(options, parsedValue);
                    TypedReference reference = __makeref(options);
                    field.SetValueDirect(reference, parsedValue);
                }
                else
                {
                    Console.WriteLine($"Failed to parse value for field {fieldName}");
                }
            }
            else
            {
                Console.WriteLine($"Field {fieldName} not found in Options struct.");
            }
        }
    }

    private object ParseValue(Type fieldType, string value)
    {
        object parsedValue = null;

        if (fieldType == typeof(bool))
        {
            if (bool.TryParse(value, out bool boolValue))
            {
                parsedValue = boolValue;
            }
        }
        else if (fieldType == typeof(string))
        {
            parsedValue = value;
        }
        else if (fieldType == typeof(Resolution))
        {
            string[] resolutionParts = value.Split('x');
            if (resolutionParts.Length == 2)
            {
                int width, height;
                if (int.TryParse(resolutionParts[0].Trim(), out width) && int.TryParse(resolutionParts[1].Trim().Split('@')[0].Trim(), out height))
                {
                    parsedValue = new Resolution { width = width, height = height };
                }
                else
                {
                    Console.WriteLine($"Failed to parse resolution value: {value}");
                }
            }
            else
            {
                Console.WriteLine($"Invalid resolution format: {value}");
            }
        }
        else if (fieldType == typeof(float))
        {
            if (float.TryParse(value, out float floatValue))
            {
                parsedValue = floatValue;
            }
        }
        else if (fieldType == typeof(int))
        {
            if (int.TryParse(value, out int intValue))
            {
                parsedValue = intValue;
            }
        }

        return parsedValue;
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
        options.isMute = false;
        options.mouseSensitivity = 1.0f;
        options.isMouseFlip = false;
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
