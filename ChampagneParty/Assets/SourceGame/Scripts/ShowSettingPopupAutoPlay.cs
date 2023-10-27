using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSettingPopupAutoPlay : MonoBehaviour
{
    [SerializeField] private GameObject settingPopup;
    public void ShowSettingPopup()
    {
        settingPopup.SetActive(true);
    }

    public void HideSettingPopup() {
        settingPopup.SetActive(false);
    }
}
