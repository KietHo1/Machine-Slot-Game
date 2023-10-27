using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoplaySettingPopup : MonoBehaviour
{
    public GUIButton paytableBtn, betSettingBtn, helpBtn;
    public Button  exitBtn;//goToLobbyBtn,
    [SerializeField] private GameObject panel;
    [SerializeField] private InputMoneyPanel inputMoneyPanel;
    [SerializeField] private Button addLossLimitValueBtn, addWinLimitValueBtn, addSingleWinLimitBtn;
    [SerializeField] private Text lossLimitTxt, winLimitTxt, singleWinLimitTxt;


    void Awake() 
    {
        SettingEvent();
    }

    private void SettingEvent()
    {
        exitBtn.onClick.AddListener(delegate{ShowPopup(false);  });
        //goToLobbyBtn.onClick.AddListener(GotoLobby);

        paytableBtn.SetEvent(delegate { UIMN.Instance.ShowPopup(PopupType.INFO); });
    }

    
    public void GotoLobby()
    {

    }

    public void ShowPopup(bool isShow = true)
    {
        panel.SetActive(isShow);

    }
}
