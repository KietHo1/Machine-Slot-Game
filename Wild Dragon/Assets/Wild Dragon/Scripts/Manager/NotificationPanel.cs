using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : Singleton<NotificationPanel>
{
    [SerializeField] private string pressStart, goodLuck, totalWin, showLineBet;
    [SerializeField] private Text text3;
    [SerializeField] private Image notifiPanelImg;
    [SerializeField] private Sprite blackPanel, redPanel;

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
                break;
            case NotificationType.GOOD_LUCK:
                ChangeText(goodLuck);
                break;
            case NotificationType.TOTAL_WIN:
                totalWin = "WON: " + Ultility.GetMoneyFormated(ResultMN.Instance.GetLineReward());
                ChangeText(totalWin);
                break;
            case NotificationType.SHOWBETLINE:

                showLineBet = "LINES: " + GameMN.Instance.GetLine() + " - BET/LINE: " + Ultility.GetMoneyFormated(GameMN.Instance.GetBet());
                ChangeText(showLineBet);
                yield return new WaitForSeconds(4f);
                Show(NotificationType.PRESS_START);
                break;
        }
    }

    public Text GetNotificationText()
    {
        return text3;
    }

    private void ChangeText(string str)
    {
        text3.gameObject.SetActive(true);
        text3.text = str;
    }

    public void ShowGUIFreeSpin()
    {
        notifiPanelImg.sprite = redPanel;
    }

    public void ShowGUIEndFreeSpin()
    {
        notifiPanelImg.sprite = blackPanel;
    }
}

public enum NotificationType
{
    PRESS_START, GOOD_LUCK, TOTAL_WIN, GAME_OVER, WINNER, SHOWBETLINE
}