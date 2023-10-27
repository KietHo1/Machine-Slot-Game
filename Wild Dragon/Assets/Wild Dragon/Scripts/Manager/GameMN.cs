using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;

public class GameMN : Singleton<GameMN>
{
    public GameData gameData;
    public UserData userData;
    public List<GameHistory> historyList = new List<GameHistory>();
    public GameHistory currentHistory;
    public BetType betType = BetType.NORMAL;
    public GameActivity gameActivity = GameActivity.NORMAL;
    public SpinType spinType = SpinType.NORMAL_SPIN;
    public List<string> currency = new List<string>();
    public float currentRewards = 0f, lastRewards = 0f;
    public float currentBet = 0f;
    public int currentLine = 0;
    public AutoPlayData autoPlayData;
    public bool isHaveGambleGame = false;
    public bool isShowingPopup = false;
    public float maxTimeToCollect = 5f;
    public List<ValueSetting> betSettingList = new List<ValueSetting>();
    public List<ValueSetting> lineSettingList = new List<ValueSetting>();

    public void Start()
    {
        Application.targetFrameRate = 60;
        Init();
    }

    public void Init()
    {
        currentBet = GameSetting.min_bet;
        currentLine = (int)GameSetting.min_line >= gameData.winningLines.Count ? gameData.winningLines.Count : (int)GameSetting.min_line;
        SlotMN.Instance.CreateReels();
        UIMN.Instance.UISetting();
        ResultMN.Instance.CreateSymbolOccurList();
    }

    public void StartSpin()
    {
        if (gameActivity != GameActivity.NORMAL)
            return;

        if (!EnoughBalance())
            return;

        UIMN.Instance.RefreshWinning();

        if (!isAutoPlay())
            UIMN.Instance.ShowGUINormalSpin();
        else
            UIMN.Instance.ShowGUIAutoSpin();

        SlotMN.Instance.RunSlots();
    }

    public void AutoPlay()
    {
        if (gameActivity != GameActivity.NORMAL)
            return;

        if (!EnoughBalance())
        {
            UIMN.Instance.ShowGUINormal();
            return;
        }

        SetAutoPlay(true);
        UIMN.Instance.ShowGUIAutoSpin();
        if (isAutoPlay() && gameActivity == GameActivity.NORMAL)
        {
            SetSpinType(SpinType.AUTO_SPIN);
            StartSpin();
        }
    }

    public void SpinStop()
    {
        StartCoroutine(IESpinStop());
    }

    private IEnumerator IESpinStop()
    {
        yield return new WaitForSeconds(0.3f);
        ShowWin();

        //CHOOSE PLAY GAMBLE
        yield return IEShowChoosePlayGambleGame();

        //PLAY GAMBLE
        yield return IEShowGambleGame();

        //END
        yield return IEAllEnd();
    }

    private void ShowWin()
    {
        ShowWinMN.Instance.ShowAllWin(true);

        float reward = ResultMN.Instance.GetLineReward();
        if (reward > 0f)
            SoundMN.Instance.PlayWinSound();

        GameStatus(GameActivity.NORMAL);
        UIMN.Instance.RewardSetting(ResultMN.Instance.GetLineReward());
    }

    private IEnumerator IEShowChoosePlayGambleGame()
    {
        bool chooseEnd = false;
        isHaveGambleGame = false;
        if (!autoPlayData.isAutoPlay && currentRewards > 0f)
        {
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.ShowGUISelectGamble();
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = true; chooseEnd = true; });
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = false; chooseEnd = true; }, isCollect: true);
            while (!chooseEnd)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public virtual IEnumerator IEShowGambleGame()
    {
        bool gambleEnd = false;
        if (isHaveGambleGame)
        {
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.ShowGUIGamble();
            GambleGameMN.Instance.ShowGamePanel();
            GambleGameMN.Instance.SetEndGambleEvent(() => { gambleEnd = true; });

            while (!gambleEnd)
                yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator IEAllEnd()
    {
        lastRewards = currentRewards;
        ShowWinMN.Instance.ShowAllWin(false);
        yield return IEGiveReward();
        EndSpinHistory();

        if (!autoPlayData.isStop())
        {
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.PlaySlot();
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.ShowGUINormal();
        }
    }

    IEnumerator IEGiveReward()
    {
        if (currentRewards > 0)
        {
            bool runNumberEnd = false;
            float bonus = (currentRewards * UserData.auto_bonus / 100f);
            GameFunc.updateHistory(GetTotalBet(), GetLine(), currentRewards + bonus, bonus);
            currentRewards += bonus;
            UIMN.Instance.BalanceSetting(currentRewards, maxTimeToCollect, () => runNumberEnd = true);
            UIMN.Instance.RewardSetting(-currentRewards, maxTimeToCollect, () => runNumberEnd = true);
            while (!runNumberEnd)
                yield return new WaitForSeconds(0.1f);
        }
    }

    public void StartSpinHistory()
    {
        GameHistory newHistory = new GameHistory();
        newHistory.StartSpinData(betType, GetTotalBet(), UserData.balance_point);
        historyList.Add(newHistory);
        currentHistory = newHistory;
    }

    public void EndSpinHistory()
    {
        currentHistory.EndSpinData(currentRewards, UserData.balance_point, ResultMN.Instance.symbolResult, ResultMN.Instance.winDatas);
    }

    public float GetTotalBet()
    {
        return  GetBet() * GetLine() * GetMultipler();
    }

    public float GetBet()
    {
        return currentBet * GetMultipler();
    }

    public int GetLine()
    {
        return currentLine;
    }

    public float GetMultipler()
    {
        return betType == BetType.FORTUNE_BET ? gameData.betMultipler : 1;
    }

    public bool EnoughBalance()
    {
        if (UserData.balance_point >= GetTotalBet())
        {
            StartSpinHistory();
            UIMN.Instance.BalanceSetting(-GetTotalBet());
            return true;
        }
        return false;
    }

    public bool isAutoPlay()
    {
        return autoPlayData.isAutoPlay;
    }

    public void GameStatus(GameActivity gameActivity)
    {
        this.gameActivity = gameActivity;
    }

    public void SetSpinType(SpinType spinType)
    {
        this.spinType = spinType;
    }

    public void SetAutoPlay(bool isAutoPlay)
    {
        autoPlayData.isAutoPlay = isAutoPlay;
    }
}

[System.Serializable]
public class UserData
{
    public static string shop_id = "1";
    public static string terminal_id = "1";
    public static float balance_point = 10000;
    public static float auto_bonus = 0;

    internal static void LoadData(JSONObject data)
    {
        data.GetField(ref auto_bonus, Schema.auto_bonus);
        LoadBalance(data);
    }

    internal static void LoadBalance(JSONObject data)
    {
        data.GetField(ref balance_point, Schema.balance_point);

        if (UIMN.Instance) UIMN.Instance.ShowBalance();
    }
}

public class GameSetting
{
    public static string game_id = "1";
    public static string name = "";
    public static float max_bet = 200;
    public static float min_bet = 10;
    public static float min_line = 1;
    public static float max_win = 1000;
    public static float percent = 80;

    internal static void LoadData(JSONObject data)
    {
        data.GetField(ref name, Schema.name);
        data.GetField(ref max_bet, Schema.max_bet);
        data.GetField(ref min_bet, Schema.min_bet);
        data.GetField(ref min_line, Schema.min_line);
        data.GetField(ref max_win, Schema.max_win);
        data.GetField(ref percent, Schema.percent);
    }
}

[System.Serializable]
public class GameHistory
{
    public TimeData timeData;
    public BetType betType;
    public float totalBet;
    public float totalWin;
    public float balanceBefore;
    public float balanceAfter;
    public List<WinData> winDatas = new List<WinData>();
    public SymbolResult symbolResult;

    public void StartSpinData(BetType betType, float totalBet, float balanceBefore)
    {
        DateTime time = DateTime.Now;
        this.timeData = new TimeData(time.Day, time.Month, time.Year, time.Hour, time.Minute); ;
        this.betType = betType;
        this.totalBet = totalBet;
        this.balanceBefore = balanceBefore;
    }

    public void EndSpinData(float totalWin, float balanceAfter, SymbolResult symbolResult, List<WinData> winDatas)
    {
        this.totalWin = totalWin;
        this.balanceAfter = balanceAfter;
        this.symbolResult = symbolResult;
        this.winDatas = winDatas;
    }
}

public enum GameActivity
{
    NORMAL, SPIN, GAMBLE_GAME, BONUS_GAME, AUTO_PLAY
}

public enum BetType
{
    NORMAL, FORTUNE_BET
}

public enum SpinType
{
    NORMAL_SPIN, AUTO_SPIN, FREE_SPIN
}

[System.Serializable]
public class TimeData
{
    public int day, month, year, hour, minute;

    public TimeData(int day, int month, int year, int hour, int minute)
    {
        this.day = day;
        this.month = month;
        this.year = year;
        this.hour = hour;
        this.minute = minute;
    }

    public string Str()
    {
        return day + "/" + month + "/" + year + " " + hour + ":" + minute;
    }
}

public class SymbolResult
{
    public int[,] symbolResult;

    public SymbolResult()
    {

    }

    public SymbolResult(int column, int row, List<int> symbolOccurList)
    {
        int symbolIndex = 0;
        symbolResult = new int[column, row];
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                symbolResult[i, j] = symbolOccurList[symbolIndex];
                symbolIndex++;
            }
        }
    }

    public void Debugs(int column, int row)
    {
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Debug.Log("Column: " + i + "_" + "row: " + j + "_" + "Symbol: " + symbolResult[i, j]);
            }
        }
    }

    public int GetSymbol(int column, int row)
    {
        return symbolResult[column, row];
    }

    public void SetSymbol(int column, int row, int symbol)
    {
        symbolResult[column, row] = symbol;
    }
}

[System.Serializable]
public class WinData
{
    public int line = -1, symbol = -1, symbolCount = 0;
    public float lineReward = 0, bonusReward = 0;
    public int freeSpinReward = 0;
}

[System.Serializable]
public class AutoPlayData
{
    public bool isAutoPlay = false;

    public void ValueSetting(bool isAutoPlay)
    {
        this.isAutoPlay = isAutoPlay;
    }

    public bool isStop()
    {
        bool _isStop = false;

        if (!isAutoPlay)
            _isStop = true;


        if (_isStop)
            Refresh();

        return _isStop;
    }

    public void Refresh()
    {
        isAutoPlay = false;
    }
}

[System.Serializable]
public class ValueSetting
{
    public float minValue, maxValue, delta;
    public bool isMatch(float value, float multi)
    {
        if (maxValue != -1)
        {
            if (value >= minValue && value < maxValue && multi == 1)
                return true;

            if (value >= minValue && value <= maxValue && multi == -1)
                return true;
        }
        else
        {
            if (value >= minValue)
                return true;
        }

        return false;
    }

    public float GetValue(float value, float multi)
    {
        value += delta * multi;
        return value;
    }
}