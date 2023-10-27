using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : Singleton<NotificationPanel>
{
    [SerializeField] private string pressStart, selectBet, goodLuck, line, gameOver;
    [SerializeField] private Text text1, text2;
    [SerializeField] private GameObject[] symbols = new GameObject[5];

    private void Start()
    {
        Show(NotificationType.PRESS_START);
    }

    public void Show(NotificationType notificationType)
    {
        StopAllCoroutines();
        StartCoroutine(IEShow(notificationType));
    }

    IEnumerator IEShow(NotificationType notificationType)
    {
        switch (notificationType)
        {
            case NotificationType.PRESS_START:

                ChangeText(pressStart);
                yield return new WaitForSeconds(1f);
                Show(NotificationType.SELECT_BET);

                break;
            case NotificationType.SELECT_BET:

                ChangeText(selectBet);
                yield return new WaitForSeconds(1f);
                Show(NotificationType.PRESS_START);

                break;
            case NotificationType.GOOD_LUCK:
                ChangeText(goodLuck);
                break;
            case NotificationType.AUTO_PLAY:
                string str = goodLuck;// + " AUTOPLAYS LEFT " + (GameMN.Instance.autoPlayData.GetNumberPlay() - 1).ToString() + " TO " + GameMN.Instance.autoPlayData.numberPlay.ToString();
                ChangeText(str);
                break;
            case NotificationType.GAME_OVER:
                ChangeText(gameOver);
                break;
            case NotificationType.WIN:
                ShowWinObject();
                break;
        }
    }

    private void ChangeText(string str)
    {
        HideAll();

        text1.gameObject.SetActive(true);
        text1.alignment = TextAnchor.MiddleCenter;
        text1.text = str;
    }

    private void ShowWinObject()
    {
        HideAll();

        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);
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
        text1.alignment = TextAnchor.MiddleLeft;
        text1.text = line + " " + (data.line+1) + ": ";

        for (int i = 0; i < data.symbolCount; i++)
        {
            symbols[i].gameObject.SetActive(true);
            symbols[i].transform.GetComponent<Image>().sprite = GameMN.Instance.gameData.symbols[data.symbol].symbol;
        }

        text2.text = " = " + Ultility.GetMoneyFormated(data.lineReward);
    }

    private void HideAll()
    {
        HideAllSymbol();
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
    }
}

public enum NotificationType
{
    PRESS_START, SELECT_BET, GOOD_LUCK, GAME_OVER, WIN, AUTO_PLAY
}
