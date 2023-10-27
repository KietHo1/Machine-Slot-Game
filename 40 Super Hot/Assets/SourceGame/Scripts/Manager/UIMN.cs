using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMN : Singleton<UIMN>
{
    public GUIButton playBtn, stopBtn, autoPlayBtn, stopAutoplayBtn, gambleBtn, collectBtn;
    [SerializeField] private GUIButton minusBetBtn, plusBetBtn, minusLineBtn, plusLineBtn;
    [SerializeField] private Button homeBtn;
    [SerializeField] private Text balanceTxt, betTxt, totalBetTxt, winTxt, lineTxt;
    [SerializeField] private PaytablePopup paytablePopup;
    [SerializeField] private Toggle soundTg;
    public GUIButton paytableBtn;

    private void Start()
    {
        EventSetting();
    }
    private void EventSetting()
    {
        homeBtn.onClick.AddListener(GoToLobby);
        soundTg.onValueChanged.AddListener(delegate { SoundMN.Instance.Mute(soundTg.isOn); });

        playBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.SPINBTN_CLICK); PlaySlot(); });
        stopBtn.SetEvent(delegate { SlotMN.Instance.StopSpining(); stopBtn.Disable(true); });
        autoPlayBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_AUTO_START); AutoPlayClick(); });
        stopAutoplayBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_AUTO_STOP); StopAutoPlayClick(); });

        paytableBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.INFOBTN_CLICK); ShowPopup(PopupType.INFO); });

        minusBetBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_CHANGEBET); ChangeBetValue(-1); });
        plusBetBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_CHANGEBET); ChangeBetValue(1); });

        minusLineBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_CHANGEBET); ChangeLines(-1); });
        plusLineBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.UI_CHANGEBET); ChangeLines(1); });
    }

    private void GoToLobby()
    {
        JavaSControl.GoToLobby();
    }

    public void UISetting()
    {
        ShowGUINormal();
        BalanceSetting(0f);
        RewardSetting(0f);
        BetSetting();
        TotalBetSetting();
        LineSetting();
    }

    public void RefreshWinning()
    {
        GameMN.Instance.currentRewards = 0f;
        RewardSetting(0f);
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

    public void BetSetting()
    {
        betTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
    }

    public void LineSetting()
    {
        lineTxt.text = GameMN.Instance.currentLine.ToString();
    }

    public void TotalBetSetting()
    {
        totalBetTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
    }

    public void PlaySlot()
    {
        GameMN.Instance.StartSpin();
    }

    private void AutoPlayClick()
    {
        GameMN.Instance.StartSpin();
    }

    private void StopAutoPlayClick()
    {
        stopAutoplayBtn.Disable(true);
        GameMN.Instance.SetAutoPlay(false);
    }

    private void HideAllPopup()
    {
        paytablePopup.ShowPopup(false);
    }

    public void ShowPopup(PopupType popupType)
    {
        HideAllPopup();
        switch (popupType)
        {
            case PopupType.INFO:
                paytablePopup.ShowPopup();
                break;
        }
    }

    private void ChangeBetValue(int multi)
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

        BetSetting();
        TotalBetSetting();
        ValueButtonSetting();
    }
    private void ChangeLines(int multi)
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
        LineSetting();
        TotalBetSetting();
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

    public void ShowGUISelectGamble()
    {
        DisableAll(true);

        gambleBtn.Show(true);
        collectBtn.Show(true);
    }

    public void ShowGUIGamble()
    {
        DisableAll(true);
        gambleBtn.Disable(true);
    }

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
        stopAutoplayBtn.Show(true);
    }

    public void ChooseGambleAddEvent(Action evt, bool isCollect = false)
    {
        if (!isCollect)
            gambleBtn.SetEvent(evt);
        else
            collectBtn.SetEvent(evt);
    }


    private void DisableAll(bool isDisable)
    {
        paytableBtn.Disable(isDisable);
        playBtn.Disable(isDisable);
        autoPlayBtn.Disable(isDisable);
        gambleBtn.Disable(isDisable);
        minusBetBtn.Disable(isDisable);
        plusBetBtn.Disable(isDisable);
        minusLineBtn.Disable(isDisable);
        plusLineBtn.Disable(isDisable);

        collectBtn.Show(false);
        stopAutoplayBtn.Show(false);
        stopBtn.Show(false);
    }

}

public enum PopupType
{
    SETTING, INFO, HISTORY, CHANGE_BET, AUTO_PLAY
}

public enum AutoPlayType
{
    AUTO, STOP
}
