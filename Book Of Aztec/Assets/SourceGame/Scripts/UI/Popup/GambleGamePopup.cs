using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GambleGamePopup : Popups
{
    [SerializeField] private GUIButton halfBtn, collectBtn, spadeBtn, clubBtn, diamondBtn, heartBtn, blackBtn, redBtn;
    [SerializeField] private List<GambleCard> historyCards = new List<GambleCard>();
    [SerializeField] private GambleCard mainCard;
    [SerializeField] private Text x4WinTxt, x2WinTxt, amountTxt, balanceTxt, winTxt, notificationTxt;
    [SerializeField] private string notification1, notification2;
    private int winCount = 0;
    private Coroutine notificationCoroutine;

    private void Start()
    {
        halfBtn.SetEvent(delegate { GambleGameMN.Instance.HalfRewardSet(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });

        collectBtn.SetEvent(delegate { GambleGameMN.Instance.Collect(); SoundMN.Instance.PlayOneShot(SFXType.CLICK); });

        spadeBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(0); });
        clubBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(1); });
        diamondBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(2); });
        heartBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(3); });

        blackBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(4); });
        redBtn.SetEvent(delegate { SoundMN.Instance.PlayOneShot(SFXType.CLICK); DisableAll(); GambleGameMN.Instance.PlayGambleGame(5); });
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        if (!isShow)
        {
            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }
            return;
        }
        GameSetting(isNewGame: true);
        notificationCoroutine = StartCoroutine(ShowNotificationRoutine());
    }

    public void GameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {
        StartCoroutine(IEGameSetting(isNewGame, isWin, result));
    }

    private IEnumerator IEGameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {
        if (isNewGame)
        {
            DisableAll(false);
            ResetButtonImages();
            MainCardSetting(isBack: true);
            GambleGameMN.Instance.AmountSetting();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            MainCardSetting(isBack: false, result.number, result.suit, isWin);

            GambleGameMN.Instance.AmountSetting();
            yield return new WaitForSeconds(0.5f);
            if (isWin)
            {
                winCount++;
                if (winCount > 3)
                    winCount = 3;
                SoundMN.Instance.PlayOneShot("GAMBLE_WIN_" + winCount.ToString());
            }
            yield return new WaitForSeconds(2f);
            if (isWin)
                GameSetting(isNewGame: true);
            else
                GambleGameMN.Instance.Collect();
        }

        CardHistorySetting();
        BetSetting();
        WinSetting();
    }

    private void CardHistorySetting()
    {
        for (int i = 0; i < historyCards.Count; i++)
        {
            GambleCard gambleCard = historyCards[i];
            if (i < GambleGameMN.Instance.gambleResults.Count)
                gambleCard.FaceSetting(isBack: false, 0, GambleGameMN.Instance.gambleResults[i]);
            else
                gambleCard.FaceSetting(isBack: true);
        }
    }

    public void WinSetting()
    {
        x4WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 4);
        x2WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 2);
    }

    public void BetSetting()
    {
        amountTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.amount);
        winTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.amount);
        balanceTxt.text = Ultility.GetMoneyFormated(UserData.balance_point);
    }

    private void MainCardSetting(bool isBack, int number = -1, int suit = 0, bool isWin = false)
    {
        mainCard.FaceSetting(isBack, number, suit);
        mainCard.ShowWinText(isWin);
    }

    private void DisableAll(bool isDisable = true)
    {
        halfBtn.Disable(isDisable);
        collectBtn.Disable(isDisable);
        spadeBtn.Disable(isDisable);
        clubBtn.Disable(isDisable);
        diamondBtn.Disable(isDisable);
        heartBtn.Disable(isDisable);
        blackBtn.Disable(isDisable);
        redBtn.Disable(isDisable);
    }


    private IEnumerator ShowNotificationRoutine()
    {
        while (true)
        {
            notificationTxt.text = notification1;
            yield return new WaitForSeconds(3f);

            notificationTxt.text = notification2;
            yield return new WaitForSeconds(3f);
        }
    }

    private void ResetButtonImages()
    {
        spadeBtn.SetImageIndex(0);
        clubBtn.SetImageIndex(0);
        diamondBtn.SetImageIndex(0);
        heartBtn.SetImageIndex(0);
        blackBtn.SetImageIndex(0);
        redBtn.SetImageIndex(0);
    }

    public void CheckClickButton(int indexSelected)
    {
        //CHOOOSE SPADE
        if (indexSelected == 0)
        {
            spadeBtn.SetImageIndex(1);
        }
        //CHOOOSE CLUB
        if (indexSelected == 1)
        {
            clubBtn.SetImageIndex(1);
        }
        //CHOOOSE DIAMOND
        if (indexSelected == 2)
        {
            diamondBtn.SetImageIndex(1);
        }
        //CHOOOSE HEART
        if (indexSelected == 3)
        {
            heartBtn.SetImageIndex(1);
        }
        //CHOOOSE BLACK
        if (indexSelected == 4)
        {
            blackBtn.SetImageIndex(1);
        }

        //CHOOSE RED
        if (indexSelected == 5)
        {
            redBtn.SetImageIndex(1);
        }
    }
}