using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GambleGamePopup : Popups
{
    [SerializeField] private GUIButton collectBtn, blackBtn, redBtn;
    [SerializeField] private List<GambleCard> historyCards = new List<GambleCard>();
    [SerializeField] private GambleCard mainCard;
    [SerializeField] private Text x2WinTxt, amountTxt, attemptsLeftTxt;
    [SerializeField] private Text notificationTxt;
    [SerializeField] private string chooseStr, youWinStr, dealerWinStr;
    private int numberOfPlays = 6;

    private void Start()
    {
        collectBtn.SetEvent(GambleGameMN.Instance.Collect);
        blackBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(4); });
        redBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(5); });
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        GameMN.Instance.isShowingPopup = isShow;
        if (!isShow) return;

        numberOfPlays = 6;
        GameSetting(isNewGame: true);

    }


    public void GameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {
        if (numberOfPlays == 0)
        {
            numberOfPlays = 6;
            return;
        }
        StartCoroutine(IEGameSetting(isNewGame, isWin, result));
    }

    IEnumerator IEGameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {

        if (isNewGame)
        {
            numberOfPlays--;
            DisableAll(false);
            MainCardSetting(isBack: true);
            ShowNotification(chooseStr);
            ShowAttemptsLeft(numberOfPlays.ToString());

            SoundMN.Instance.PlayLoop(SFXType.GAMBLE_LOOP);
            StartCoroutine(IEMainCardAnims());
        }
        else
        {
            SoundMN.Instance.StopLoop();
            yield return new WaitForSeconds(1f);
            MainCardSetting(isBack: false, result.suit);
            string str = isWin ? youWinStr : dealerWinStr;
            ShowNotification(str);

            if (isWin)
                SoundMN.Instance.PlayOneShot(SFXType.GAMBLE_WIN);
            else
                SoundMN.Instance.PlayOneShot(SFXType.GAMBLE_LOSE);

            yield return new WaitForSeconds(2f);

            if (isWin)
                GameSetting(isNewGame: true);
            else
            {
                GambleGameMN.Instance.ShowGamePanel(false);
                UIMN.Instance.ShowGUINormal();
            }
        }

        GambleGameMN.Instance.AmountSetting();

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
                gambleCard.FaceSetting(isBack: false, GambleGameMN.Instance.gambleResults[i]);
            else
                gambleCard.FaceSetting(isBack: true);
        }
    }

    public void WinSetting()
    {
        //x4WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 4);
        x2WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 2);
    }

    public void BetSetting()
    {
        amountTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.amount);
        //halfAmountTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.halfCurrenBet);
        //balanceTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.userData.balance);
    }

    private void MainCardSetting(bool isBack, int suit = 0)
    {
        mainCard.FaceSetting(isBack, suit);
    }

    private void DisableAll(bool isDisable = true)
    {
        StopAllCoroutines();


        blackBtn.Disable(isDisable);
        redBtn.Disable(isDisable);


    }


    IEnumerator IEMainCardAnims()
    {
        mainCard.SetCardBack("cardback1");
        yield return new WaitForSeconds(0.1f);
        mainCard.SetCardBack("cardback2");
        yield return new WaitForSeconds(0.1f);
        mainCard.SetCardBack("cardback1");

        StartCoroutine(IEMainCardAnims());
    }

    private void ShowNotification(string str)
    {
        if (str == youWinStr) notificationTxt.color = new Color(1, 0.8433768f, 0);
        if (str == dealerWinStr) notificationTxt.color = new Color(0.3018868f, 0.3018868f, 0.03018868f);
        notificationTxt.text = str;
    }

    private void ShowAttemptsLeft(string str)
    {
        attemptsLeftTxt.text = str;
    }

}
