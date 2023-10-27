using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Networking.UnityWebRequest;

public class GambleGameMN : Singleton<GambleGameMN>
{
    public Action endGamble;
    [SerializeField] private GambleGamePopup gamblePopup;
    public List<GambleResult> cardResults = new List<GambleResult>();
    public GambleResult dealerResult;
    private int numberOfPlayCard = 4;
    bool isWin = false;
    public bool played = false;

    public void SetEndGambleEvent(Action endGamble = null)
    {
        this.endGamble = endGamble;
    }

    public void ShowGamePanel(bool isShow = true)
    {
        if (isShow)
        {
            played = false;
            isWin = false;
        }
            

        gamblePopup.ShowPopup(isShow);
    }

    public void CreateDealerCard()
    {
        dealerResult = new GambleResult();
        dealerResult.DealerGenerate();
        dealerResult.id = 0;

        isWin = false;
    }

    public void PickCard(int cardID)
    {
        GambleResult playerResult = cardResults.Find(x => x.id == cardID);

        if (playerResult.isBeats(dealerResult))
            isWin = true;

        if (GameMN.Instance.currentRewards * 2 > GameSetting.max_win)
        {
            isWin = false;
            while (playerResult.isBeats(dealerResult))
            {
                playerResult.NormalGenerate();
            }
        }

        gamblePopup.GameSetting(isNewGame: false, isWin);
    }

    public IEnumerator GeneratePlayerCard()
    {
        cardResults.Clear();

        for (int i = 0; i < numberOfPlayCard; i++)
        {
            GambleResult result = new GambleResult();
            result.NormalGenerate();
            result.id = i + 1;
            cardResults.Add(result);
        }

        for (int i = 0; i < numberOfPlayCard; i++)
        {
            while (cardResults[i].isDuplicateResult(dealerResult) || cardResults[i].isDuplicateResult(dealerResult))
            {
                cardResults[i].NormalGenerate();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Collect()
    {
        ShowGamePanel(false);
        endGamble?.Invoke();
        UIMN.Instance.collectBtn.Show(false);
    }
}
[Serializable]
public class GambleResult
{
    public int number;
    public int suit;
    public int id;

    public void DealerGenerate()
    {
        suit = UnityEngine.Random.Range(0, 4);
        number = UnityEngine.Random.Range(5, 12);
    }
    public void NormalGenerate()
    {
        suit = UnityEngine.Random.Range(0, 4);
        number = UnityEngine.Random.Range(2, 16);
    }

    public bool isDuplicateResult(GambleResult result)
    {
        if (id == result.id)
            return false;

        if (suit == result.suit && number == result.number)
            return true;

        return false;
    }

    public bool isDuplicateResultInList(List<GambleResult> resultList)
    {
        if (resultList.FindIndex(x => x.isDuplicateResult(this) == true) != -1)
            return true;

        return false;
    }

    public bool isBeats(GambleResult result)
    {
        if (number > result.number)
            return true;
        return false;
    }
}
