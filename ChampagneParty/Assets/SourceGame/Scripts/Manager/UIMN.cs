using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIMN : Singleton<UIMN>
{
    public GUIButton playBtn, autoPlayBtn, stopAutoBtn, infoBtn, gambleBtn, collectBtn, paytableBtn, moreinfoBtn, playFreeSpinBtn, stopBtn;
    [SerializeField] private GUIButton minusBetBtn, plusBetBtn, minusLineBtn, plusLineBtn, soundBtn;
    [SerializeField] private Text balanceTxt, totalBetTxt, betTxt, lineTxt, winTxt;
    [SerializeField] private InfoPopup infoPopup;
    public FreeSpinPanel freeSpinPanel;
    [SerializeField] private Button homeBtn;

    private void Start() {
        EventSetting();
    }
    private void EventSetting()
    {
        playBtn.SetEvent(delegate {
            PlaySlot();
            SoundMN.Instance.PlayOneShot(SFXType.SPIN);
        }); 

        autoPlayBtn.SetEvent(delegate { AutoPlayClick(); });
        stopAutoBtn.SetEvent(delegate { StopAutoPlayClick(); stopAutoBtn.Show(false); });

        stopBtn.SetEvent(delegate { SlotMN.Instance.ReelStoping();  stopBtn.Show(false); });

        infoBtn.SetEvent(ShowInfoPage);

        minusBetBtn.SetEvent(delegate { ChangeBetValue(-1); SoundMN.Instance.PlayOneShot(SFXType.CHANGE_BET); });
        plusBetBtn.SetEvent(delegate { ChangeBetValue(1); SoundMN.Instance.PlayOneShot(SFXType.CHANGE_BET); });

        minusLineBtn.SetEvent(delegate { ChangeLines(-1); SoundMN.Instance.PlayOneShot(SFXType.CHANGE_BET); });
        plusLineBtn.SetEvent(delegate { ChangeLines(1); SoundMN.Instance.PlayOneShot(SFXType.CHANGE_BET); });
        paytableBtn.SetEvent(delegate { moreinfoBtn.Show(true); ShowInfoPage(); });

        homeBtn.onClick.AddListener(GoToLobby);
        soundBtn.SetEvent(delegate { SoundMN.Instance.Mute(soundBtn.GetStatusIndex()); });
    }

    private void GoToLobby()
    {
        JavaSControl.GoToLobby();
    }

    public void UISetting() {
        BalanceSetting(0f);
        RewardSetting(0f);
        TotalBetSetting();
        BetSetting();
        LineSetting();
        LinesBetSetting();
        LineCountSetting();
        ChangeLines(-40);
        ShowGUINormal();
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

    private void TotalBetSetting()
    {
        totalBetTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
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
        LinesBetSetting();
        infoPopup.PaytableSetting();
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

    private void LineSetting()
    {
        lineTxt.text = GameMN.Instance.GetLine().ToString();
        ShowLineMN.Instance.PreviewLines(GameMN.Instance.currentLine);
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

    public void LinesBetSetting()
    {
        foreach (PreviewLine line in ShowLineMN.Instance.previewLines)
        {
            line.linePrefab.SetBet(GameMN.Instance.GetBet());
        }
    }

    public void LineCountSetting()
    {
        for (int i = 0; i < ShowLineMN.Instance.previewLines.Length; i++)
        {
            ShowLineMN.Instance.previewLines[i].linePrefab.SetLine(i + 1);
        }
    }

    private void ShowInfoPage()
    {
        ShowPopup(PopupType.INFO);
    }

    public void RefreshWinning()
    {
        GameMN.Instance.currentRewards = 0f;
        RewardSetting(0f);
    }

    public void BetSetting()
    {
        betTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
    }
   
    public void PlaySlot()
    {
        GameMN.Instance.StartSpin();
        infoPopup.HidePopup();
    }

    public void AutoPlayClick()
    {
        GameMN.Instance.autoPlayData.AutoplaySetting(true);
        GameMN.Instance.AutoPlay();
    }

    private void StopAutoPlayClick()
    {
        stopAutoBtn.Disable(false);
        GameMN.Instance.SetAutoPlay(false);
    }

    private void HideAllPopup()
    {
        infoPopup.ShowPopup(false);
    }

    private void ShowPopup(PopupType popupType)
    {
        HideAllPopup();
        switch(popupType) 
        {
            case PopupType.INFO:
                infoPopup.ShowPopup();
                break;
        }
    }

    public void ShowGUINormal()
    {
        DisableAll(false);
        
        collectBtn.Show(false);
        gambleBtn.Disable(true);
        playBtn.Disable(false);

        ValueButtonSetting();

        NotificationPanel.Instance.Show(NotificationType.PRESS_START);
    }

    public void ShowGUIStartFreeSpin(Action evt)
    {
        DisableAll(true);
        playFreeSpinBtn.Show(true);
        playFreeSpinBtn.SetEvent(delegate { evt(); ShowGUIFreeSpin(); });
    }

    public void ShowGUIFreeSpin()
    {
        DisableAll(true);
        freeSpinPanel.Hide();
        NotificationPanel.Instance.ShowGUIFreeSpin();
    }

    public void ShowGUIEndFreeSpin()
    {
        freeSpinPanel.Show(isStep1: false, GameMN.Instance.autoPlayData.totalFreespinReward);
        NotificationPanel.Instance.ShowGUIEndFreeSpin();
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

        stopBtn.Show(false);
        collectBtn.Show(false);
        stopAutoBtn.Show(false);
        playFreeSpinBtn.Show(false);
    }

    public void ShowGUISelectGamble()
    {
        DisableAll(true);

        gambleBtn.Disable(false);
        collectBtn.Show(true);
    }

    public void ShowGUIGamble()
    {
        DisableAll(true);
        gambleBtn.Disable(true);
    }

    public void ChooseGambleAddEvent(Action evt, bool isCollect = false)
    {
        if (!isCollect)
            gambleBtn.SetEvent(evt);
        else
            collectBtn.SetEvent(delegate { evt(); collectBtn.Disable(true); gambleBtn.Disable(true); });    
    }
}

public enum PopupType
{
    SETTING, INFO, HISTORY, CHANGE_BET, AUTO_PLAY, GAMBLE
}

public enum AutoPlayType
{
   AUTO, STOP
} 
