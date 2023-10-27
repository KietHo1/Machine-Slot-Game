using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : Singleton<NotificationPopup>
{
    [SerializeField] private Button exitBtn;
    public GameObject panel;
    [SerializeField] private Text titleTxt, contentTxt;

    public void Start()
    {
        if (exitBtn != null)
            exitBtn.onClick.AddListener(delegate { ShowPopup(false); });
    }

    private void ShowPopup(bool isShow = true)
    {
        panel.SetActive(isShow);
    }

    public void ShowContent(string content, string title = "NOTIFICATION" )
    {
        ShowPopup();
        titleTxt.text = title;
        contentTxt.text = content;
    }
}
