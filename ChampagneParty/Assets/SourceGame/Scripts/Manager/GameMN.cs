using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMN : Singleton<GameMN>
{
    public GameData gameData;
    public UserData userData;
    public List<GameHistory> historyList = new List<GameHistory>();
    public GameHistory currentHistory;
    public BetType betType = BetType.NORMAL;
    public GameType gameType = GameType.NORMAL;
    public GameActivity gameActivity = GameActivity.NORMAL;
    public SpinType spinType = SpinType.NORMAL_SPIN;
    public List<string> currency = new List<string>();
    public float currentRewards;
    public float currentBet = 0f;
    public int currentLine = 0;
    public AutoPlayData autoPlayData;
    public BonusData bonusData;
    public bool isHaveGambleGame = false;
    public bool isShowingPopup = false;
    public float maxTimeToGetReward = 1f, maxTimeToCollect = 1f;
    public List<ValueSetting> betSettingList = new List<ValueSetting>();
    public List<ValueSetting> lineSettingList = new List<ValueSetting>();

    private void Start() {
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

    public float GetTimeToRun(float value, float maxTime)
    {
        float timeToRun = Mathf.Abs(value) / 10f;
        if (timeToRun > maxTime)
            timeToRun = maxTime;
        return timeToRun;
    }


    public void BalanceSetting(float value)
    {
        UIMN.Instance.BalanceSetting(value);
    }

    public void StartSpin()
    {
        if (gameActivity != GameActivity.NORMAL)
            return;

        if (!EnoughBalance())
        {
            NotificationPopup.Instance.ShowContent("NOT ENOUGH BALANCE!");
            return;
        }

        UIMN.Instance.RefreshWinning();

        if (!isAutoPlay() && !isFreeSpin())
            UIMN.Instance.ShowGUINormalSpin();
        else if (isAutoPlay() && !isFreeSpin())
            UIMN.Instance.ShowGUIAutoSpin();
        else if (isFreeSpin())
            UIMN.Instance.ShowGUIFreeSpin();

        SoundMN.Instance.PlayOneShot(SFXType.AUTO_PLAY);
        SlotMN.Instance.RunSlots();
    }

    public void AutoPlay()
    {
        if (gameActivity != GameActivity.NORMAL)
            return;

        if (!EnoughBalance())
        {
            NotificationPopup.Instance.ShowContent("NOT ENOUGH BALANCE!");
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
        SoundMN.Instance.StopLoop();
        yield return new WaitForSeconds(0.3f);
        yield return ShowWin();

        yield return CheckPartyBonus();

        if (!isFreeSpin())
            yield return CheckFreeSpin();
        else
            yield return FreeSpinStop();

        yield return IEShowChoosePlayGambleGame();

        //PLAY GAMBLE
        yield return IEShowGambleGame();

        //END
        yield return IEAllEnd();
    }

    IEnumerator CheckPartyBonus()
    {
        bool haveBonus = false;
        for(int i=0; i< ResultMN.Instance.winDatas.Count; i++)
        {
            WinData wData = ResultMN.Instance.winDatas[i];
            SymbolData SymData = gameData.symbols[wData.symbol];
            if (SymData.type == SymbolType.BONUS)
            {
                haveBonus = true;
                break;
            }
        }

        if (haveBonus)
        {
            bool bonusEnd = false;
            yield return new WaitForSeconds(2f);
            ShowWinMN.Instance.ShowAllWin(false);
            NotificationPanel.Instance.ShowGUIFreeSpin();
            BonusGameMN.Instance.ShowGamePanel();
            BonusGameMN.Instance.SetEndBonusEvent(() => { bonusEnd = true; });

            while (!bonusEnd)
                yield return new WaitForSeconds(0.1f);

            NotificationPanel.Instance.ShowGUIEndFreeSpin();
        }
    }

    IEnumerator CheckFreeSpin()
    {
        bool isHaveFreeSpin = false;
        int maxFreeSpin = 0;
        
        //Check Scatter
        foreach (WinData w in ResultMN.Instance.winDatas)
        {
            SymbolData SymData = gameData.symbols[w.symbol];
            isHaveFreeSpin = SymData.type == SymbolType.SCATTER && w.symbolCount >= 3;
            if (isHaveFreeSpin)
            {
                maxFreeSpin = 15;
                break;
            }
        }

        //Check Wild
        foreach (WinData w in ResultMN.Instance.winDatas)
        {
            SymbolData SymData = gameData.symbols[w.symbol];
            isHaveFreeSpin = SymData.type == SymbolType.WILD && w.symbolCount == 5;
            if (isHaveFreeSpin)
            {
                maxFreeSpin = 30;
                break;
            }
        }

        if (!isHaveFreeSpin)
            yield break;

        yield return new WaitForSeconds(1f);
        autoPlayData.FreespinSetting(true, maxFreeSpin);
        autoPlayData.SetReward(currentRewards);
        UIMN.Instance.freeSpinPanel.Show(isStep1: true);
        SoundMN.Instance.PlayOneShot(SFXType.FREE_INTRO);

        yield return new WaitForSeconds(5f);

        bool startFreeSpin = false;
        UIMN.Instance.ShowGUIStartFreeSpin(() => startFreeSpin = true);
        while (!startFreeSpin)
            yield return new WaitForSeconds(0.1f);

    }

    IEnumerator FreeSpinStop()
    {
        autoPlayData.SetReward(currentRewards * 2);
        if (!isFreeSpin())
        {
            currentRewards = 0;
            UIMN.Instance.RewardSetting(autoPlayData.totalFreespinReward, 0.5f);
            SoundMN.Instance.StopLoop();
            UIMN.Instance.ShowGUIEndFreeSpin();
            yield return new WaitForSeconds(4f);
            UIMN.Instance.freeSpinPanel.Hide();
        }
    }

    private IEnumerator ShowWin()
    {
        GameStatus(GameActivity.NORMAL);
        ShowWinMN.Instance.ShowAllWin(true);
        bool runNumberEnd = false;
        UIMN.Instance.RewardSetting(ResultMN.Instance.GetLineReward(), maxTimeToGetReward, () => runNumberEnd = true);
        //SoundMN.Instance.PlayLoop(SFXType.COLLECT_LOOP);
        while (!runNumberEnd)
            yield return new WaitForSeconds(0.1f);
    }

    public virtual IEnumerator IEShowGambleGame()
    {
        bool gambleEnd = false;
        if (isHaveGambleGame && currentRewards > 0f)
        {
            SoundMN.Instance.StopLoop();
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.ShowGUIGamble();
            GambleGameMN.Instance.ShowGamePanel();
            GambleGameMN.Instance.SetEndGambleEvent(() => { gambleEnd = true; });

            while (!gambleEnd)
                yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator IEShowChoosePlayGambleGame()
    {
        bool chooseEnd = false;
        isHaveGambleGame = false;
        if (!isHaveGambleGame && currentRewards > 0f && !isAutoPlay() && !isFreeSpin())
        {
            yield return new WaitForSeconds(0.25f);
            UIMN.Instance.ShowGUISelectGamble();
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = true; chooseEnd = true; });
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = false; chooseEnd = true; }, isCollect: true);
            while (!chooseEnd)
                yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator IEAllEnd()
    {
        SoundMN.Instance.StopLoop();
        ShowWinMN.Instance.ShowAllWin(false);
        ShowWinMN.Instance.HideAllWin();

        yield return IEGiveReward();

        EndSpinHistory();

        GameStatus(GameActivity.NORMAL);
        yield return new WaitForSeconds(0.5f);

        if (!autoPlayData.isStop())
        {
            yield return new WaitForSeconds(1f);
            UIMN.Instance.PlaySlot();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            UIMN.Instance.ShowGUINormal();
        }
    }

    IEnumerator IEGiveReward()
    {
        if (currentRewards > 0 && !isFreeSpin())
        {
            if(isAutoPlay())
                yield return new WaitForSeconds(1f);

            bool runNumberEnd = false;
            UIMN.Instance.BalanceSetting(currentRewards, maxTimeToCollect, () => runNumberEnd = true);
            UIMN.Instance.RewardSetting(-currentRewards, maxTimeToCollect, () => runNumberEnd = true);
            SoundMN.Instance.PlayLoop(SFXType.COLLECT);
            while (!runNumberEnd)
                yield return new WaitForSeconds(0.1f);
            //if (!isFreeSpin())
            SoundMN.Instance.StopLoop();

            //SoundMN.Instance.PlayOneShot(SFXType.TEASER1);

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
      return GetBet() * GetLine();
   }
   
   public float GetBet()
   {
      return currentBet;
   }

   public int GetLine()
   {
      return currentLine;
   }

   public bool EnoughBalance()
   {
        if (isFreeSpin())
        {
            StartSpinHistory();
            return true;
        }

        if (UserData.balance_point >= GetTotalBet())
        {
         StartSpinHistory();
         BalanceSetting(-GetTotalBet());
         return true;
        }
      return false;
   }

   public bool isAutoPlay()
   {
      return autoPlayData.isAutoPlay;
   }

    public bool isFreeSpin()
    {
        return autoPlayData.isFreeSpin;
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
      this.timeData = new TimeData(time.Day, time.Month, time.Year, time.Hour, time.Minute);;
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

public enum GameType
{
   NORMAL, GAMBLE, BONUS
}

//public enum GambleResult
//{
//    WIN, LOSE
//}

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

[System.Serializable]
public class WinData
{
    public int line = -1, symbol = -1, symbolCount = 0;
    public float lineReward = 0, bonusReward = 0;
    public int freeSpinReward = 0;
}

[System.Serializable]
public class BonusData
{
    public bool isBonus = false;
    public void BonusSetting(bool isBonus)
    {
        this.isBonus = isBonus;
    }
}

[System.Serializable]
public class AutoPlayData
{
    public bool isAutoPlay = false;
    public bool isFreeSpin = false;
    public float totalFreespinReward = 0f;
    public int freeSpinCount = 0;
    public int maxFreeSpin = 15;

    public void AutoplaySetting(bool isAutoPlay)
    {
        this.isAutoPlay = isAutoPlay;
    }

    public void FreespinSetting(bool isFreeSpin, int maxFreeSpin)
    {
        this.isFreeSpin = isFreeSpin;
        totalFreespinReward = 0;
        freeSpinCount = 0;
        this.maxFreeSpin = maxFreeSpin;
    }

    public void SetReward(float value)
    {
        totalFreespinReward += value;
        freeSpinCount++;
        if (freeSpinCount >= maxFreeSpin)
            isFreeSpin = false;
    }

    public bool isStop()
    {
        bool _isStop = false;

        if (!isAutoPlay && !isFreeSpin)
            _isStop = true;

        if (_isStop)
            Refresh();

        return _isStop;
    }

    public void Refresh()
    {
        isAutoPlay = false;
    }

    public string GetNotification()
    {
        return "FREE GAME " + freeSpinCount.ToString() + " TO " + maxFreeSpin.ToString();
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

