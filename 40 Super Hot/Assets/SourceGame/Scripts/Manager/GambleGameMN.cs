using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GambleGameMN : Singleton<GambleGameMN>
{
    public Action endGamble;
    public float amount = 0f, currentBet = 0f;
    [SerializeField] private GambleGamePopup gamblePopup;
    public List<int> gambleResults = new List<int>();
    bool isWin = false;

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

    public void PlayGambleGame(int suit)
    {
        GambleResult result = new GambleResult();
        result.Generate();
        isWin = false;

        gambleResults.Insert(0, result.suit);

        //CHOOOSE BLACK
        if (suit == 4)
        {
            if (currentBet * 2 > GameSetting.max_win)
            {
                result.ChangeColorToLose(1);
                gambleResults[0] = result.suit;
            }

            if (result.suit < 2)
            {
                isWin = true;
                currentBet *= 2;
            }

        }

        //CHOOSE RED
        if (suit == 5)
        {
            if (currentBet * 2 > GameSetting.max_win)
            {
                result.ChangeColorToLose(3);
                gambleResults[0] = result.suit;
            }

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

    public void Collect()
    {
        GameMN.Instance.currentRewards = amount;
        ShowGamePanel(false);
        endGamble?.Invoke();
    }
}

public class GambleResult
{
    // Suit = 0 Spade
    // Suit = 1 Club
    // Suit = 2 Diamond
    // Suit = 3 Heart
    public int suit;
    public int number;

    public void Generate()
    {
        suit = UnityEngine.Random.Range(0, 4);
        number = UnityEngine.Random.Range(2, 11);
    }

    public void ChangeColorToLose(int suit)
    {
        int[] blackSuit = new int[2] { 0, 1 };
        int[] redSuit = new int[2] { 2, 3 };

        bool isBlack = false;
        Debug.Log(suit);
        for (int i = 0; i < blackSuit.Length; i++)
        {
            if (blackSuit[i] == suit)
            {

                isBlack = true;
                break;
            }
        }

        int rand = UnityEngine.Random.Range(0, 2);
        this.suit = isBlack ? redSuit[rand] : blackSuit[rand];
    }
}

public enum CardSuit
{
    SPADE, CLUB, DIAMOND, HEART
}
