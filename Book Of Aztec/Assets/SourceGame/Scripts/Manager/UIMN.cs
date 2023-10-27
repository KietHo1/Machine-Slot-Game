using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMN : Singleton<UIMN>
{
    [SerializeField] private GUIButton playBtn, stopBtn, autoPlayBtn, stopAutoBtn, infoBtn, gambleBtn, collectBtn;
    [SerializeField] private GUIButton minusBetBtn, plusBetBtn, minusLineBtn, plusLineBtn;
    [SerializeField] private Text balanceTxt, betTxt, totalBetTxt, lineTxt, winTxt;
    [SerializeField] private Text totalWinMesh;
    [SerializeField] public GameObject gameNameImg, totalWinFrame;
    [SerializeField] private InfoPopup infoPopup;

    [Header("UI TO CHANGE")]
    [SerializeField] private Image totalWinFrameImg;
    [SerializeField] private SpriteRenderer bgImg;
    [SerializeField] private Sprite totalWinFrameNormalSpr, totalWinFrameBonusSpr, bgNormalSpr, bgBonusSpr;
    [SerializeField] private GameObject bonusSymbolPanel, notificationBar, betLinePanel, startWheelBonus;
    [SerializeField] private SymbolInfo bonusSymbolInfo;
    public GUIButton exitBG;
    public GameObject notEnoughBalance;

    private void Start()
    {
        EventSetting();
        ShowGUINormal();
    }

    private void EventSetting()
    {
        playBtn.SetEvent(PlaySlot);
        stopBtn.SetEvent(delegate { SlotMN.Instance.ReelStoping(); stopBtn.Show(false); });

        autoPlayBtn.SetEvent(delegate { AutoPlayClick(); SoundMN.Instance.PlayOneShot(SFXType.AUTO_START); });
        stopAutoBtn.SetEvent(delegate { StopAutoPlayClick(); SoundMN.Instance.PlayOneShot(SFXType.AUTO_STOP); stopAutoBtn.Show(false); });

        infoBtn.SetEvent(delegate { ShowInfoPage(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });

        minusBetBtn.SetEvent(delegate { ChangeBetValue(-1); SoundMN.Instance.PlayOneShot("STEP_1"); });
        plusBetBtn.SetEvent(delegate { ChangeBetValue(1); SoundMN.Instance.PlayOneShot("STEP_1"); });

        minusLineBtn.SetEvent(delegate { ChangeLines(-1); SoundMN.Instance.PlayOneShot("LINE_STEP_1"); });
        plusLineBtn.SetEvent(delegate { ChangeLines(1); SoundMN.Instance.PlayOneShot("LINE_STEP_1"); });

        exitBG.SetEvent(delegate {GameMN.Instance.isExitStartWheel = true; });
    }

    public void ShowBalance()
    {
        balanceTxt.text = Ultility.GetMoneyFormated(UserData.balance_point);
    }

    public void ShowInfoPage()
    {
        ShowPopup(PopupType.INFO);
    }
    public void UISetting()
    {
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
        RewardSetting(0);
    }

    public void BalanceSetting(float value, float timeToRun = 1f, Action numberRunEnd = null)
    {
        balanceTxt.GetComponent<NumberRun>().Run(UserData.balance_point, value, timeToRun, numberRunEnd);
        UserData.balance_point += value;
    }

    public void RewardSetting(float value, float timeToRun = 1f, Action numberRunEnd = null)
    {
        totalWinFrame.SetActive(value == 0 ? false : true);
        gameNameImg.SetActive(value == 0 ? true : false);

        totalWinMesh.GetComponent<NumberRun>().Run(GameMN.Instance.currentRewards, value, timeToRun, numberRunEnd);
        winTxt.GetComponent<NumberRun>().Run(GameMN.Instance.currentRewards, value, timeToRun, numberRunEnd);
        GameMN.Instance.currentRewards += value;
    }
    
    public void TotalBetSetting()
    {
        totalBetTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetTotalBet());
    }
    public void TotalWinSetting()
    {
        totalWinFrame.SetActive(false);
        gameNameImg.SetActive(true);
    }

    public void PlaySlot()
    {
        GameMN.Instance.StartSpin();
    }

    private void AutoPlayClick()
    {
        GameMN.Instance.autoPlayData.ValueSetting(true);
        GameMN.Instance.SetSpinType(SpinType.AUTO_SPIN);
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

    private void ShowPopup(PopupType popupType)
    {
        HideAllPopup();
        switch (popupType)
        {
            case PopupType.INFO:
                infoPopup.ShowPopup();
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
        infoPopup.PaytableSetting();

        NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
    }

    public void BetSetting()
    {
        betTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
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
        LineMN.Instance.LineSetting();

        NotificationPanel.Instance.Show(NotificationType.SHOWBETLINE);
    }
    public void LineSetting()
    {
        lineTxt.text = GameMN.Instance.GetLine().ToString();
        ValueButtonSetting();
    }

    public void ShowGUINormal()
    {
        DisableAll(false);

        gambleBtn.Disable(true);
        playBtn.Show(true);

        ValueButtonSetting();

        NotificationPanel.Instance.Show(NotificationType.PRESS_START);
        NotificationPanel.Instance.HideAll();
        BetLineSliderMN.Instance.BetLineSetting(false);
        SettingPanel.Instance.SettingPanelSetting(true);
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
        NotificationPanel.Instance.HideAll();
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

    public void ShowGUISelectGamble()
    {
        DisableAll(true);

        gambleBtn.Show(true);
        collectBtn.Show(true);
    }

    public void ChooseGambleAddEvent(Action evt, bool isCollect = false)
    {
        if (!isCollect)
            gambleBtn.SetEvent(evt);
        else
            collectBtn.SetEvent(delegate { evt(); collectBtn.Disable(true); gambleBtn.Disable(true); });
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

    public void ChangeSpr(bool isBonusSpin)
    {
        totalWinFrameImg.sprite = isBonusSpin ? totalWinFrameBonusSpr : totalWinFrameNormalSpr;
        bgImg.sprite = isBonusSpin ? bgBonusSpr : bgNormalSpr;
        
        bonusSymbolPanel.SetActive(isBonusSpin);
        bonusSymbolInfo.Setting(GameMN.Instance.gameData.symbols[BonusGameMN.Instance.GetChoosenSymbol()], isBonus: true);
        bonusSymbolInfo.ShowRewards();

        notificationBar.SetActive(!isBonusSpin);
        betLinePanel.SetActive(!isBonusSpin);

        infoBtn.Show(!isBonusSpin);
        gambleBtn.Show(!isBonusSpin);
        gambleBtn.Disable(true);
        collectBtn.Show(!isBonusSpin);
    }

    public void StartWheelBonusShow(bool isShow)
    {
        startWheelBonus.SetActive(isShow);
        exitBG.Show(isShow);
    } 
}

public enum PopupType
{
    SETTING, INFO, GAMBLE
}