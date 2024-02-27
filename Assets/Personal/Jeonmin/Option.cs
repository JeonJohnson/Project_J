using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum KeyAction { Up, Down, Left, Right, Fire, Suck, Interact, End }
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class Option : MonoBehaviour
{
    [SerializeField] UIOption_View view;

    public int resoultionNum;
    private List<Resolution> resolutions = new List<Resolution>(); 
    public FullScreenMode screenMode;

    KeyCode[] defaltKeys = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.E };

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
        //ŸÁ
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
}
