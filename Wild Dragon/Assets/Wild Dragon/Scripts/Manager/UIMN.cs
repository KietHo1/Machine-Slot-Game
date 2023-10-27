using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Playables;

public class UIMN : Singleton<UIMN>
{
    [SerializeField] private GUIButton playBtn, stopBtn, autoPlayBtn, stopAutoBtn, infoBtn, gambleBtn, collectBtn, speedBtn;
    [SerializeField] private GUIButton minusBetBtn, plusBetBtn, minusLineBtn, plusLineBtn;
    [SerializeField] private Button reloadBtn, fullscreenBtn;
    public Text balanceTxt, totalBetTxt, betTxt, lineTxt, winTxt;
    [SerializeField] private InfoPopup infoPopup;
    private int speedIndex = 0;
    public Sprite[] spriteFastBtn;
    public Image fastBtnImage;

    private bool isFullScreen;

    public void Start() {
        EventSetting();
        ShowGUINormal();
        SoundMN.Instance.PlayOneShot(SFXType.onstart);
    }
    private void EventSetting()
    {
        playBtn.SetEvent(PlaySlot);
        stopBtn.SetEvent(delegate { SlotMN.Instance.ReelStoping(); stopBtn.Disable(true); });

        reloadBtn.onClick.AddListener(Reload);
        fullscreenBtn.onClick.AddListener(FullScreen);
        
        autoPlayBtn.SetEvent(delegate { AutoPlayClick(); SoundMN.Instance.PlayOneShot(SFXType.auto_on); });
        stopAutoBtn.SetEvent(delegate { StopAutoPlayClick(); SoundMN.Instance.PlayOneShot(SFXType.auto_off);  stopAutoBtn.Disable(true); });

        infoBtn.SetEvent(ShowInfoPage);

        speedBtn.SetEvent(SetSpeedValue);

        minusBetBtn.SetEvent(delegate
        {
            ChangeBetValue(-1);
            
        });
        plusBetBtn.SetEvent(delegate {
            ChangeBetValue(1);
        });
        minusLineBtn.SetEvent(delegate
        {
            ChangeLines(-1);
        });
        plusLineBtn.SetEvent(delegate
        {
            ChangeLines(1);
        });
    }

    private void SetSpeedValue()
    {
        speedIndex++;
        switch (speedIndex)
        {
            case 0:
                fastBtnImage.sprite = spriteFastBtn[0];
                SpinData spinData1 = new SpinData();
                spinData1.ValueSetting(10, 4, 8);
                SetSpeed(spinData1);
                break;
            case 1:
                fastBtnImage.sprite = spriteFastBtn[1];
                SpinData spinData2 = new SpinData();
                spinData2.ValueSetting(1, 1, 16);
                SetSpeed(spinData2);
                break;
            default:
                fastBtnImage.sprite = spriteFastBtn[0];
                SpinData spinData4 = new SpinData();
                spinData4.ValueSetting(10, 4, 8);
                SetSpeed(spinData4);
                speedIndex = 0;
                break;
        }
    }

    private void SetSpeed(SpinData spinData)
    {
        foreach (SlotReel reel in SlotMN.Instance.reels)
        {
            reel.SetSpeedReel(spinData);
        }
    }

    public void ShowInfoPage()
    {
        ShowPopup(PopupType.INFO);
    }

    public void UISetting() {
        BalanceSetting(0f);
        RewardSetting(0f);
        TotalBetSetting();
        BetSetting();
        LineSetting();
        ChangeLines(-10);
    }

    public void RefreshWinning()
    {
        GameMN.Instance.currentRewards = 0f;
    }

    public void ShowBalance()
    {
        balanceTxt.text = Ultility.GetMoneyFormated(UserData.balance_point);
    }

    public void BalanceSetting(float value, float timeToRun = 1f, Action numberRunEnd = null)
    {
        balanceTxt.GetComponent<NumberRun>().Run(UserData.balance_point, value, timeToRun, numberRunEnd);
        UserData.balance_point += value;
    }

    public void RewardSetting(float value, float timeToRun = 1f, Action numberRunEnd = null)
    {
        winTxt.GetComponent<NumberRun>().Run(GameMN.Instance.currentRewards, value, timeToRun, numberRunEnd);
        GameMN.Instance.currentRewards += value;
    }

    public void TotalBetSetting()
    {
        totalBetTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
    }
   
    public void PlaySlot()
    {
        GameMN.Instance.StartSpin();
        infoPopup.HidePopup();
    }

    private void AutoPlayClick()
    {
        GameMN.Instance.autoPlayData.ValueSetting(true);
        PlaySlot();
    }

    private void StopAutoPlayClick()
    {
        stopAutoBtn.Show(false);
        GameMN.Instance.SetAutoPlay(false);
    }

    private void HideAllPopup()
    {
        infoPopup.ShowPopup(false);
    }

    public void ShowPopup(PopupType popupType)
    {
        HideAllPopup();
        switch(popupType) 
        {
            case PopupType.INFO:
                infoPopup.ShowPopup();
                break;
        }
    }

    public void ChangeBetValue(int multi)
    {
        ValueSetting valueSetting = GameMN.Instance.betSettingList.Find(x => x.isMatch(GameMN.Instance.currentBet, multi));
        if (valueSetting != null)
        {
            GameMN.Instance.currentBet = valueSetting.GetValue(GameMN.Instance.currentBet, multi);
            if (GameMN.Instance.currentBet <= GameSetting.min_bet)
                GameMN.Instance.currentBet = GameSetting.min_bet;

            if (GameMN.Instance.currentBet >= GameSetting.max_bet)
                GameMN.Instance.currentBet = GameSetting.max_bet;
        }

        SoundMN.Instance.PlayOneShot(SFXType.step1);
        BetSetting();
        TotalBetSetting();
        infoPopup.PaytableSetting();
    }

    public void BetSetting()
    {
        betTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
        ValueButtonSetting();
    }

    public void ChangeLines(int multi)
    {
        ValueSetting valueSetting = GameMN.Instance.lineSettingList.Find(x => x.isMatch(GameMN.Instance.currentLine, multi));
        if (valueSetting != null)
        {
            GameMN.Instance.currentLine = (int)valueSetting.GetValue(GameMN.Instance.currentLine, multi);
            if (GameMN.Instance.currentLine <= GameSetting.min_line)
                GameMN.Instance.currentLine = (int)GameSetting.min_line;

            if (GameMN.Instance.currentLine >= GameMN.Instance.gameData.winningLines.Count)
                GameMN.Instance.currentLine = GameMN.Instance.gameData.winningLines.Count;
        }

        SoundMN.Instance.PlayOneShot(SFXType.step1);
        ShowLineMN.Instance.ShowLinePanel(GameMN.Instance.currentLine);
        //ShowLineMN.Instance.ShowLinePreview(GameMN.Instance.gameData.lineCountList[GameMN.Instance.currentLinesIndex]);

        LineSetting();
        TotalBetSetting();
    }

    public void LineSetting()
    {
        lineTxt.text = GameMN.Instance.GetLine().ToString();
        ValueButtonSetting();
    }

    private void ValueButtonSetting()
    {
        minusBetBtn.Disable(false);
        plusBetBtn.Disable(false);

        minusLineBtn.Disable(false);
        plusLineBtn.Disable(false);

        if (GameMN.Instance.currentBet >= GameSetting.max_bet)
            plusBetBtn.Disable(true);

        if (GameMN.Instance.currentBet <= GameSetting.min_bet)
            minusBetBtn.Disable(true);

        if (GameMN.Instance.currentLine >= GameMN.Instance.gameData.winningLines.Count)
            plusLineBtn.Disable(true);

        if (GameMN.Instance.currentLine <= GameSetting.min_line)
            minusLineBtn.Disable(true);
    }

    public void ShowGUIGamble()
    {
        DisableAll(true);
        gambleBtn.Disable(true);
    }

    //public void ShowGUIStartFreeSpin(Action evt)
    //{
    //    DisableAll(true);
    //    playFreeSpinBtn.Show(true);
    //    playFreeSpinBtn.SetEvent(delegate { evt(); ShowGUIFreeSpin(); });
    //}

    //public void ShowGUIFreeSpin()
    //{
    //    DisableAll(true);
    //    freeSpinPanel.Hide();
    //    NotificationPanel.Instance.ShowGUIFreeSpin();
    //}

    //public void ShowGUIEndFreeSpin()
    //{
    //    freeSpinPanel.Show(isStep1: false, GameMN.Instance.autoPlayData.totalFreespinReward);
    //    NotificationPanel.Instance.ShowGUIEndFreeSpin();
    //}

    public void ShowGUINormal()
    {
        DisableAll(false);

        gambleBtn.Disable(true);
        playBtn.Show(true);

        ValueButtonSetting();

        NotificationPanel.Instance.Show(NotificationType.PRESS_START);
    }

    public void ShowGUINormalSpin()
    {
        DisableAll(true);
        stopBtn.Show(true);
    }

    public void ShowGUIAutoSpin()
    {
        DisableAll(true);
        stopAutoBtn.Show(true);
    }

    public void ShowGUISelectGamble()
    {
        DisableAll(true);

        gambleBtn.Show(true);
        collectBtn.Show(true);
    }

    public void ChooseGambleAddEvent(Action evt, bool isCollect = false)
    {
        if(!isCollect)
            gambleBtn.SetEvent(evt);
        else
            collectBtn.SetEvent(delegate{evt(); collectBtn.Disable(true); gambleBtn.Disable(true); 
            });
    }

    private void Reload()
    {

    }

    private void FullScreen()
    {

    }

    private void DisableAll(bool isDisable)
    {
        infoBtn.Disable(isDisable);
        playBtn.Disable(isDisable);
        autoPlayBtn.Disable(isDisable);
        gambleBtn.Disable(isDisable);
        minusBetBtn.Disable(isDisable);
        plusBetBtn.Disable(isDisable);
        minusLineBtn.Disable(isDisable);
        plusLineBtn.Disable(isDisable);

        collectBtn.Show(false);
        stopAutoBtn.Show(false);
        stopBtn.Show(false);
    }
}

public enum PopupType
{
    SETTING, INFO, GAMBLE
}
