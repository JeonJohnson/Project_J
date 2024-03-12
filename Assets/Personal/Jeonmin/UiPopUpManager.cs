using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPopUpManager : MonoBehaviour
{
    public View InvenView;
    public View ShopView;
    public View OptionView;
    public View PauseView;

    public List<View> allViewList;
    public LinkedList<View> activeSystemViewList;

    private void Awake()
    {
        activeSystemViewList = new LinkedList<View>();
        Init();
        InitCloseAll();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeSystemViewList.Count > 0)
            {
                ClosePopup(activeSystemViewList.First.Value, true);
            }
            else
            {
                ToggleKeyDownAction(KeyCode.Escape, PauseView);
            }
        }

        ToggleKeyDownAction(KeyCode.I, InvenView);
        ToggleKeyDownAction(KeyCode.U, OptionView);
    }

    private void Init()
    {
        // 1. 리스트 초기화
        allViewList = new List<View>()
        {
            InvenView, ShopView, OptionView, PauseView
        };
    }

    /// <summary> 시작 시 모든 팝업 닫기 </summary>
    private void InitCloseAll()
    {
        foreach (var popup in allViewList)
        {
            ClosePopup(popup, popup.isSystem);
        }
    }

    private void ToggleKeyDownAction(in KeyCode key, View popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }

    private void ToggleOpenClosePopup(View popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup, popup.isSystem);
        else ClosePopup(popup, popup.isSystem);
    }

    private void OpenPopup(View popup, bool isSystem)
    {
        if (isSystem) activeSystemViewList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    private void ClosePopup(View popup, bool isSystem)
    {
        if(isSystem) activeSystemViewList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    private void RefreshAllPopupDepth()
    {
        foreach (var popup in activeSystemViewList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
