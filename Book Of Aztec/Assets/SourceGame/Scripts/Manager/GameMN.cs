using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float currentRewards;
    public float currentBet = 0f;
    public int currentLine = 0;
    public AutoPlayData autoPlayData;
    public BonusSpinData bonusSpinData;
    public bool isHaveGambleGame = false;
    public float maxTimeToGetReward = 7.2f, maxTimeToCollect = 5f;
    public List<ValueSetting> betSettingList = new List<ValueSetting>();
    public List<ValueSetting> lineSettingList = new List<ValueSetting>();
    [HideInInspector] public bool isExitStartWheel = false;
    private void Start()
    {
        Application.targetFrameRate = 60;
        Init();
        SoundMN.Instance.PlayOneShot(SFXType.START_GAME);
    }

    private void Init()
    {
        currentLine = (int)GameSetting.min_line >= gameData.winningLines.Count ? gameData.winningLines.Count : (int)GameSetting.min_line;
        currentBet = GameSetting.min_bet;
        UIMN.Instance.UISetting();
        ResultMN.Instance.CreateSymbolOccurList();
        LineMN.Instance.LineSetting();
    }

    public void StartSpin()
    {
        if (gameActivity != GameActivity.NORMAL)
            return;

        if (!EnoughBalance())
        {
            UIMN.Instance.notEnoughBalance.SetActive(true);
            return;
        }
        else
        {
            UIMN.Instance.notEnoughBalance.SetActive(false);
        }

        if(!isBonusSpin())
            UIMN.Instance.RefreshWinning();

        if (!isAutoPlay())
            UIMN.Instance.ShowGUINormalSpin();
        else
            UIMN.Instance.ShowGUIAutoSpin();

        SlotMN.Instance.RunSlots();
    }

    public void SpinStop()
    {
        StartCoroutine(IESpinStop());
    }

    private IEnumerator IESpinStop()
    {
       
        yield return new WaitForSeconds(0.3f);
        yield return ShowAllWin();

        yield return CheckBonusSpin();

        //CHOOSE PLAY GAMBLE
        yield return IEShowChoosePlayGambleGame();

        //PLAY GAMBLE
        yield return IEShowGambleGame();

        //END
        yield return IEAllEnd();
    }

    private IEnumerator ShowAllWin()
    {
        GameStatus(GameActivity.NORMAL);
        ShowWinMN.Instance.ShowAllWin(true);

        BetLineSliderMN.Instance.BetLineSetting(true);
        SettingPanel.Instance.SettingPanelSetting(false);
        float reward = ResultMN.Instance.GetLineReward();

        if (reward <= 0f)
        {
            NotificationPanel.Instance.Show(NotificationType.GAME_OVER);
        }
        else
        {
            SoundMN.Instance.PlayOneShot("WIN_" + ResultMN.Instance.GetSymbolHighestReward().ToString());
            
            NotificationPanel.Instance.Show(NotificationType.WINNER);

            yield return new WaitForSeconds(2.75f);
            bool runNumberEnd = false;
            UIMN.Instance.RewardSetting(reward, maxTimeToGetReward, () => runNumberEnd = true);
            NotificationPanel.Instance.Show(NotificationType.TOTAL_WIN);
            SoundMN.Instance.PlayLoop(SFXType.COUNT_UP);
            while (!runNumberEnd)
                yield return new WaitForSeconds(0.1f);

            SoundMN.Instance.StopLoop();
            SoundMN.Instance.PlayOneShot(SFXType.WIN_DONE);
        }
    }

    IEnumerator SpawnBonusSymbol()
    {
        int column = gameData.column;
        int row = gameData.row;
        for (int i = 0; i < column; i++)
        {
            for (int j = row - 1; j >= 0; j--)
            {
                if(!ResultMN.Instance.CheckDuplicateSymbolWithBonus(i, j))
                {
                    Symbol symbol = SlotMN.Instance.GetSymbol(i,j);
                    SymbolData symbolData = gameData.symbols[ResultMN.Instance.symbolResultBonus.GetSymbol(i,j)];
                    SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_REPLACE);
                    symbol.Setting(symbolData);
                    symbol.SetBlurIcon(false);
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }

        for (int i = 0; i < ResultMN.Instance.winDatas.Count; i++)
        {
            WinData data = ResultMN.Instance.winDatas[i];
            if (data.isBonus)
            {
                SoundMN.Instance.PlayOneShot(SFXType.WIN_LINE_FREE_SPIN);
                ShowWinMN.Instance.ShowOnceWinBonus(data);
                yield return new WaitForSeconds(0.3f);
            }
        }

        ShowWinMN.Instance.HideAllWin();
        ShowLineMN.Instance.HideAll();

        float reward = ResultMN.Instance.GetBonusReward();

        bonusSpinData.expandingWin = reward;
        if (bonusSpinData.expandingWin != 0)
            NotificationPanel.Instance.Show(NotificationType.EXPANDING_WIN);

        if (reward > 0f)
        {
            yield return new WaitForSeconds(2.75f);
            bool runNumberEnd = false;
            UIMN.Instance.RewardSetting(reward, maxTimeToGetReward, () => runNumberEnd = true);
            SoundMN.Instance.PlayLoop(SFXType.COUNT_UP);
            while (!runNumberEnd)
                yield return new WaitForSeconds(0.1f);
            SoundMN.Instance.StopLoop();
            SoundMN.Instance.PlayOneShot(SFXType.WIN_DONE);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CheckBonusSpin()
    {
        bool isHaveBonus = false;
        foreach (WinData w in ResultMN.Instance.winDatas)
        {
            SymbolData nextSymData = gameData.symbols[w.symbol];
            if (nextSymData.type == SymbolType.SCATTER && w.symbolCount >= 3)
            {
                isHaveBonus = true;
                break;
            }
        }

        if (isHaveBonus)
        {
            if (isBonusSpin())
            {
                yield return new WaitForSeconds(0.5f);
                //SHOW PLUS PANEL
                BonusSpinCountPanel.Instance.Show(true);
                SoundMN.Instance.PlayOneShot(SFXType.ADD_FREE_SPIN);

                yield return new WaitForSeconds(5f);
                BonusSpinCountPanel.Instance.Show(false);
            }
            else
            {
                yield return new WaitForSeconds(1f);

                //SHOW START WHEEL PANEL
                SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_INIT);
                UIMN.Instance.StartWheelBonusShow(true);

                StartCoroutine(NotificationPanel.Instance.WheelNotification());
               
                yield return new WaitForSeconds(3.5f);
                SoundMN.Instance.PlayLoop(SFXType.FREE_SPIN_WAITING);
                while(!isExitStartWheel)
                    yield return new WaitForSeconds(0.1f);

                NotificationPanel.Instance.isWaiting = false;
                StopCoroutine(NotificationPanel.Instance.WheelNotification());

                SoundMN.Instance.StopLoop();
                UIMN.Instance.StartWheelBonusShow(false);

                //SHOW BONUS GAME WHEEL
                bool isEndWheelGame = false;

                BonusGameMN.Instance.Show(true, () => { isEndWheelGame = true; });
                SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_TRANSITION);
                while (!isEndWheelGame)
                    yield return new WaitForSeconds(0.1f);

                BonusGameMN.Instance.Show(false);
            }

            bonusSpinData.ValueSetting(true, gameData.bonusSpinCount);
            UIMN.Instance.ChangeSpr(isBonusSpin());
        } 
    }


    private IEnumerator IEShowChoosePlayGambleGame()
    {
        bool chooseEnd = false;
        isHaveGambleGame = false;

        if (isHaveGambleGame == false && currentRewards > 0f && !isAutoPlay() && !isBonusSpin())
        {
            yield return new WaitForSeconds(1f);
            UIMN.Instance.ShowGUISelectGamble();
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = true; chooseEnd = true; });
            UIMN.Instance.ChooseGambleAddEvent(() => { isHaveGambleGame = false; chooseEnd = true; }, isCollect: true);
            SoundMN.Instance.PlayLoop(SFXType.SHOW_CHOOSE_GAMBLE);

            while (!chooseEnd)
                yield return new WaitForSeconds(0.1f);
        }
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

    private IEnumerator IEAllEnd()
    {
        SoundMN.Instance.StopLoop();

        ShowWinMN.Instance.ShowAllWin(false);
        ShowWinMN.Instance.HideAllWin();
        ShowLineMN.Instance.HideAll();

        LineMN.Instance.LineSetting();
        
        if (isBonusSpin() && bonusSpinData.currentSpinCount != 0)
            yield return SpawnBonusSymbol();

        if (isBonusSpin())
        {
            yield return new WaitForSeconds(0.5f);
            SoundMN.Instance.PlayLoop(SFXType.FREE_SPIN_MELODY);
        }

        if (isBonusSpin() && bonusSpinData.isStop())
        {
            SoundMN.Instance.StopLoop();
            SoundMN.Instance.PlayOneShot(SFXType.FREE_SPIN_MELODY_DONE);
            CongratulationBonusWinPanel.Instance.Show(true);
            yield return new WaitForSeconds(2f);
            CongratulationBonusWinPanel.Instance.Show(false); 
        }

        if (!isBonusSpin())
        {
            yield return new WaitForSeconds(0.2f);
            yield return IEGiveReward();
        }
       
        EndSpinHistory();
         
        yield return new WaitForSeconds(0.5f);
        GameStatus(GameActivity.NORMAL);

        UIMN.Instance.ChangeSpr(isBonusSpin());
        if (!autoPlayData.isStop())
        {
            yield return new WaitForSeconds(0.5f);
            UIMN.Instance.PlaySlot();
        }
        else
        {
            if (!isBonusSpin())
            {
                UIMN.Instance.TotalWinSetting();
            }    

            UIMN.Instance.ShowGUINormal();

            if (isBonusSpin())
            {
                NotificationPanel.Instance.Show(NotificationType.BONUSSPIN_PLAYED);
            }
        }
    }

    public void GiveReward()
    {
        if (currentRewards > 0)
        {
            UIMN.Instance.BalanceSetting(currentRewards);
            UIMN.Instance.RewardSetting(-currentRewards);
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
            SoundMN.Instance.PlayLoop(SFXType.COLLECT);
            while (!runNumberEnd)
                yield return new WaitForSeconds(0.1f);
            SoundMN.Instance.StopLoop();
            SoundMN.Instance.PlayOneShot(SFXType.COLLECT_DONE);
        }
    }
    public float GetTimeToRun(float value, float maxTime)
    {
        float timeToRun = Mathf.Abs(value) / 10f;
        if (timeToRun > maxTime)
            timeToRun = maxTime;
        return timeToRun;
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

    public bool isBonusSpin()
    {
        return bonusSpinData.isBonusSpin;
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
    public static float max_win = 100000;
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

[System.Serializable]
public class WinData
{
    public int line = -1, symbol = -1, symbolCount = 0;
    public float lineReward = 0, bonusReward = 0;
    public int freeSpinReward = 0;
    public bool isBonus = false;
}

[System.Serializable]
public class BonusSpinData
{
    public bool isBonusSpin;
    public int symbolBonus = 0;
    public int bonusSpinCount = 0;
    public int currentSpinCount = 0;
    public int playedCount = 0;
    public float expandingWin = 0;

    public void ValueSetting(bool isBonusSpin , int count)
    {
        this.isBonusSpin = isBonusSpin;
        this.bonusSpinCount += count;
        playedCount = 0;
    }

    public bool isStop()
    {
        bool _isStop = false;

        currentSpinCount++;

        isBonusSpin = currentSpinCount < bonusSpinCount ? true : false;

        if (!isBonusSpin)
            _isStop = true;

        if (_isStop)
            Refresh();

        return _isStop;
    }

    public void Refresh()
    {
        isBonusSpin = false;
        bonusSpinCount = 0;
        playedCount = currentSpinCount;
        currentSpinCount = 0;
    }
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
            if (value >= minValue && value < maxValue && multi > 0)
                return true;

            if (value >= minValue && value <= maxValue && multi < 0)
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