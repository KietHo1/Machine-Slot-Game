using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayPopup : Popups
{
    public Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            HidePopup();
            UIMN.Instance.AutoPlayClick();
        });
    }
}
