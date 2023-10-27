using System;
using System.Collections.Generic;
using UnityEngine;

public class GambleGameMN : Singleton<GambleGameMN>
{
    public Action endGamble;
    public float amount = 0f, currentBet = 0f;
    [SerializeField] private GambleGamePopup gamblePopup;
    public List<int> gambleResults = new List<int>();
    private bool isWin = false;

    public void SetEndGambleEvent(Action endGamble = null)
    {
        this.endGamble = endGamble;
    }

    public void ShowGamePanel(bool isShow = true)
    {
        isWin = false;

        amount = GameMN.Instance.currentRewards;
        currentBet = amount;

        gamblePopup.BetSetting();
        gamblePopup.ShowPopup(isShow);
    }

    private void HalfBet()
    {
        float halfAmount = amount / 2;

        if (halfAmount < 0.01f)
            return;

        currentBet = halfAmount;
        amount = halfAmount;

        GameMN.Instance.currentRewards = halfAmount;
        GameMN.Instance.GiveReward();

        gamblePopup.BetSetting();
    }

    public void PlayGambleGame(int index)
    {
        GambleResult result = new GambleResult();
        result.Generate();
        isWin = false;

        gambleResults.Insert(0, result.suit);

        //CHOOSSE Suit
        if (index < 4)
        {
            gamblePopup.CheckClickButton(index);
            // index = 1 Spade
            // index = 2 Club
            // index = 3 Diamond
            // index = 4 Heart
            if (index == result.suit)
            {
                isWin = true;
                currentBet *= 4;
            }
        }

        //CHOOOSE BLACK
        if (index == 4)
        {
            gamblePopup.CheckClickButton(index);
            if (result.suit < 2)
            {
                isWin = true;
                currentBet *= 2;
            }
        }

        //CHOOSE RED
        if (index == 5)
        {
            gamblePopup.CheckClickButton(index);
            if (result.suit >= 2)
            {
                isWin = true;
                currentBet *= 2;
            }
        }

        if (!isWin)
        {
            currentBet = 0f;
        }

        gamblePopup.GameSetting(isNewGame: false, isWin, result);
    }

    public void AmountSetting()
    {
        amount = currentBet;
        gamblePopup.BetSetting();
    }

    public void HalfRewardSet()
    {
        HalfBet();
        gamblePopup.WinSetting();
    }

    public void Collect()
    {
        GameMN.Instance.currentRewards = amount;
        ShowGamePanel(false);
        endGamble?.Invoke();
    }
}

public class GambleResult
{
    // Suit = 1 Spade
    // Suit = 2 Club
    // Suit = 3 Diamond
    // Suit = 4 Heart
    public int suit;

    public int number;

    public void Generate()
    {
        suit = UnityEngine.Random.Range(0, 4);
        number = UnityEngine.Random.Range(2, 11);
    }
}

public enum CardSuit
{
    SPADE, CLUB, DIAMOND, HEART
}