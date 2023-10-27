using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : Singleton<NotificationPanel>
{
    [SerializeField] private string pressStart, selectBet, minMaxBet, goodLuck, line, gameOver, showLineBet, winner, totalWin, bonusspinplayed, expandingWin, waitingEndWheel, none;
    [SerializeField] private Text lineTxt, winTxt, notificationTxt, notificationBonustxt;
    [SerializeField] private GameObject[] symbols = new GameObject[5];

    private void Start()
    {
        Show(NotificationType.PRESS_START);
        HideAll();
    }

    public void Show(NotificationType notificationType)
    {
        StopAllCoroutines();
        StartCoroutine(IEShow(notificationType));
    }

    private IEnumerator IEShow(NotificationType notificationType)
    {
        switch (notificationType)
        {
            case NotificationType.PRESS_START:
                ChangeText(pressStart);
                yield return new WaitForSeconds(5f);
                Show(NotificationType.SELECT_BET);
                break;

            case NotificationType.SELECT_BET:
                ChangeText(selectBet);
                yield return new WaitForSeconds(5f);
                Show(NotificationType.MIN_MAX);
                break;

            case NotificationType.MIN_MAX:
                ChangeText(minMaxBet);
                yield return new WaitForSeconds(5f);
                Show(NotificationType.PRESS_START);
                break;

            case NotificationType.GOOD_LUCK:
                ChangeText(goodLuck);
                break;

            case NotificationType.BONUSSPIN_PLAYED:
                bonusspinplayed = "BONUSSPINS PLAYED: " + (GameMN.Instance.bonusSpinData.currentSpinCount - 1).ToString() + " OF " + GameMN.Instance.bonusSpinData.bonusSpinCount.ToString();
                ChangeTextBonus(bonusspinplayed);
                break;

            case NotificationType.BONUSSPIN_CURRENT:
                bonusspinplayed = "BONUSSPINS PLAYED: " + GameMN.Instance.bonusSpinData.currentSpinCount.ToString() + " OF " + GameMN.Instance.bonusSpinData.bonusSpinCount.ToString();
                ChangeTextBonus(bonusspinplayed);
                break;

            case NotificationType.EXPANDING_WIN:
                expandingWin ="Expanding Symbol Win: " + GameMN.Instance.bonusSpinData.expandingWin.ToString();
                ChangeTextBonus(expandingWin);
                break;

            case NotificationType.GAME_OVER:
                ChangeText(gameOver);
                break;

            case NotificationType.WIN:
                ShowWinObject();
                break;

            case NotificationType.SHOWBETLINE:

                showLineBet = "LINES: " + GameMN.Instance.GetLine() + " - BET/LINE: " + Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
                ChangeText(showLineBet);
                yield return new WaitForSeconds(4f);
                Show(NotificationType.PRESS_START);

                break;

            case NotificationType.WINNER:
                ChangeText(winner);
                break;

            case NotificationType.TOTAL_WIN:
                totalWin = "TOTAL WIN: " + Ultility.GetMoneyFormated(ResultMN.Instance.GetLineReward());
                ChangeText(totalWin);
                break;

            case NotificationType.TOTAL_LINE:
                ShowTotalLine();
                break;
        }
    }
  
    private void ChangeText(string str)
    {
        notificationTxt.gameObject.SetActive(true);
        notificationTxt.text = str;
    }

    private void ChangeTextBonus(string str)
    {
        HideAll();
        notificationBonustxt.gameObject.SetActive(true);
        notificationBonustxt.text = str;

    }
    private void ShowWinObject()
    {
        HideAll();

        lineTxt.gameObject.SetActive(true);
        winTxt.gameObject.SetActive(true);
    }

    private void HideAllSymbol()
    {
        foreach (GameObject symbol in symbols)
        {
            symbol.SetActive(false);
        }
    }

    public void ShowWin(WinData data)
    {
        lineTxt.text = line + " " + (data.line + 1) + ": ";
        if(data.line == -1)
        {
            lineTxt.text = "SCATTERED: " ;
        }

        for (int i = 0; i < data.symbolCount; i++)
        {
            symbols[i].gameObject.SetActive(true);
            symbols[i].transform.GetChild(0).GetComponent<Image>().sprite = GameMN.Instance.gameData.symbols[data.symbol].symbol;
        }

        winTxt.text = " WIN: " + Ultility.GetMoneyFormated(data.lineReward);
    }

    public void ShowTotalLine()
    {
        int totalLine = GetTotalLine();
        lineTxt.gameObject.SetActive(true);
        lineTxt.text = "WIN " + totalLine + " LINES";
    }
    
    private int GetTotalLine()
    {
        HideAll();
        return ResultMN.Instance.winDatas.Count;
    }

   [HideInInspector] public bool isWaiting = true;
    public IEnumerator WheelNotification()
    {
        while (isWaiting)
        {
            yield return ContinuousNotification(1, waitingEndWheel);
            yield return ContinuousNotification(1, none);
        }
    }

    private IEnumerator ContinuousNotification(int time, string txt)
    {
        isWaiting = true;
        ChangeText(txt);
        yield return new WaitForSeconds(time);
        
    }

    public void HideAll()
    {
        HideAllSymbol();
        lineTxt.gameObject.SetActive(false);
        winTxt.gameObject.SetActive(false);
        notificationBonustxt.gameObject.SetActive(false);
    }
}

public enum NotificationType
{
    PRESS_START, SELECT_BET, MIN_MAX, GOOD_LUCK, GAME_OVER, WIN, SHOWBETLINE, WINNER, TOTAL_WIN, TOTAL_LINE,
    EXPANDING_WIN, ADD_BONUS, BONUSSPIN_PLAYED, BONUSSPIN_CURRENT
}