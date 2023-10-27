using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetSettingPopup : MonoBehaviour
{
    public Toggle[] betToggles;
    public Text[] betToggleLabels;
    public GUIButton paytableBtn, autoplaySettingBtn, helpBtn;
    public Button  exitBtn;//goToLobbyBtn,
    //public Toggle soundTg;
    [SerializeField] private GameObject panel;
    private bool isShow = false;
    public bool GetIsShow() { return isShow; }

    void Start() 
    {
        SettingEvent();
    }

    private void SettingEvent()
    {
        exitBtn.onClick.AddListener(delegate{ShowPopup(false);  });
        //goToLobbyBtn.onClick.AddListener(GotoLobby);

        paytableBtn.SetEvent(delegate { UIMN.Instance.ShowPopup(PopupType.INFO); });
        autoplaySettingBtn.SetEvent(delegate { UIMN.Instance.ShowPopup(PopupType.SETTING); });
        SetBetBtnsEvent();
        ChangeBetLabel();
    }

    private void SetBetBtnsEvent()
    {
        //for (int i = 0; i < betToggles.Length ; i++) 
        //{
        //    int n = i;
             
        //    betToggles[i].onValueChanged.AddListener(delegate { 
        //        ChangeBet(n);
        //        if (this.betToggles[n].isOn && isShow)
        //        {
        //            int index = UIMN.Instance.CheckBetToggleIndex(this.betToggles);
        //            UIMN.Instance.ClearIsOn(UIMN.Instance.betToggles);
        //            UIMN.Instance.betToggles[index].isOn = true;
        //        }
        //    });       
        //}
    }

    private void ChangeBetLabel()
    {
        for (int i = 0; i < betToggleLabels.Length; i++)
        {
            //betToggleLabels[i].text = (GameMN.Instance.gameData.bets[i] * GameMN.Instance.GetLine()).ToString();
        }
    }

    public void GotoLobby()
    {

    }

    public void ShowPopup(bool isShow = true)
    {
        panel.SetActive(isShow);
        this.isShow = isShow;
    }

    private void ChangeLines(int value, bool isMax = false)
    {
        //if(isMax)
        //{
        //    GameMN.Instance.currentLinesIndex = GameMN.Instance.gameData.lineCountList.Count - 1;
        //    //lineTxt.text = GameMN.Instance.GetLine().ToString();
        //    return;
        //}

        //GameMN.Instance.currentLinesIndex += value;

        //if(GameMN.Instance.currentLinesIndex >= GameMN.Instance.gameData.lineCountList.Count)
        //    GameMN.Instance.currentLinesIndex = 0;
        
        //if(GameMN.Instance.currentLinesIndex < 0)
        //    GameMN.Instance.currentLinesIndex = GameMN.Instance.gameData.lineCountList.Count - 1;
        
        ////lineTxt.text = GameMN.Instance.GetLine().ToString();
        //UIMN.Instance.HighlightLineValue(GameMN.Instance.currentLinesIndex);
        UIMN.Instance.BetSetting();
    }

    private void ChangeBet(int value, bool isMax = false)
    {
        //GameMN.Instance.currentBetIndex = value;
    }
}
