using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GambleGamePopup : Popups
{
    [SerializeField] private GUIButton collectBtn;
    [SerializeField] private GambleCard dealerCard;
    public GambleCard[] playerCards;
    [SerializeField] private Text winTxt, stakeTxt, balanceTxt;
    [SerializeField] private Text notificationTxt;
    [SerializeField] private string chooseStr, youWinStr, dealerWinStr;


    private void Start()
    {
        collectBtn.SetEvent(GambleGameMN.Instance.Collect);
        notificationTxt.text = chooseStr;
        SetCardID();
    }

    private void SetCardID()
    {
        dealerCard.id = 0;
        for (int i = 0 ; i < playerCards.Length; i++)
        {
            playerCards[i].id = i + 1;
        }
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        GameMN.Instance.isShowingPopup = isShow;
        collectBtn.Show(true);
        UIMN.Instance.gambleBtn.Disable(true);
        if (!isShow) return;
        GameSetting(isNewGame: true);
    }


    public void GameSetting(bool isNewGame, bool isWin = false)
    {
        StartCoroutine(IEGameSetting(isNewGame, isWin));
    }

    IEnumerator IEGameSetting(bool isNewGame, bool isWin = false)
    {
        yield return new WaitForSeconds(0.1f);
        if (isNewGame)
        {
            collectBtn.Disable(false);
            PlayerCardSetting(isBack: true);
            GambleGameMN.Instance.CreateDealerCard();
            StartCoroutine(GambleGameMN.Instance.GeneratePlayerCard());
            DealerCardSetting(isBack: false, GambleGameMN.Instance.dealerResult.number, GambleGameMN.Instance.dealerResult.suit);
        }
        else
        {
            collectBtn.Disable(true);
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < GambleGameMN.Instance.cardResults.Count; i++)
                playerCards[i].FaceSetting(isBack: false, GambleGameMN.Instance.cardResults[i].number, GambleGameMN.Instance.cardResults[i].suit);

            notificationTxt.text = isWin ? youWinStr : dealerWinStr;

            yield return new WaitForSeconds(4f);
            if (isWin)
            {
                GameMN.Instance.currentRewards = GameMN.Instance.currentRewards * 2;
                SoundMN.Instance.PlayOneShot(SFXType.GAMBLE_WIN);
                GameSetting(isNewGame: true);
            }
            else
            {
                GameMN.Instance.currentRewards = 0;
                SoundMN.Instance.PlayOneShot(SFXType.GAMBLE_LOSE);
                GambleGameMN.Instance.Collect();
            }
        }

        AmountSetting();
    }

    private void AmountSetting()
    {
        winTxt.text = Ultility.GetMoneyFormated(GameMN.Instance.currentRewards);
    }

    private void PlayerCardSetting(bool isBack, int number = -1, int suit = 0)
    {
        foreach (GambleCard playerCard in playerCards)
        {
            playerCard.FaceSetting(isBack, number, suit);
        }
    }
    private void DealerCardSetting(bool isBack, int number = -1, int suit = 0)
    {
        dealerCard.FaceSetting(isBack, number, suit);
    }
}
