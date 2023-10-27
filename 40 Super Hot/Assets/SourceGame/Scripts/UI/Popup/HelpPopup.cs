using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopup : MonoBehaviour
{
    public GUIButton paytableBtn, betSettingBtn, autoplaySettingBtn;
    public Button  exitBtn;//goToLobbyBtn,
    [SerializeField] private GameObject panel;


    void Awake() 
    {
        SettingEvent();
    }

    private void SettingEvent()
    {
        exitBtn.onClick.AddListener(delegate { ShowPopup(false); });
        //goToLobbyBtn.onClick.AddListener(GotoLobby);
    }

    
    public void GotoLobby()
    {

    }

    public void ShowPopup(bool isShow = true)
    {
        panel.SetActive(isShow);
        //lineTxt.text = GameMN.Instance.gameData.lineCountList[GameMN.Instance.currentLinesIndex].ToString();
        //betTxt.text = GameMN.Instance.gameData.bets[GameMN.Instance.currentBetIndex].ToString();
        //autoPlayTxt.text = GameMN.Instance.gameData.NumberAutoPlay[autoIndex].ToString();
    }

    
    
}
