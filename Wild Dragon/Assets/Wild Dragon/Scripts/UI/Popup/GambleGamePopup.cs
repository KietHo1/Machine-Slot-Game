using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GambleGamePopup : Popups
{
    [SerializeField] private GUIButton halfBtn, collectBtn, spadeBtn, clubBtn, diamondBtn, heartBtn, blackBtn, redBtn;
    [SerializeField] private List<GambleCard> historyCards = new List<GambleCard>();
    [SerializeField] private GambleCard mainCard;
    [SerializeField] private Text x4WinTxt, x2WinTxt, amountTxt, winTxt, balanceTxt;
    [SerializeField] private Text notificationTxt;
    public string youWinStr, dealerWinStr;
    private float timeLoop = 0.3f;

    public void Start()
    {
        halfBtn.SetEvent(delegate { GambleGameMN.Instance.HalfRewardSet(); /*StartCoroutine(BetBtnSetting());*/});
        collectBtn.SetEvent(delegate { GambleGameMN.Instance.Collect(); /*StopAllCoroutines();*/ });

        spadeBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(0); });
        clubBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(1); });
        diamondBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(2); });
        heartBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(3); });

        blackBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(4); });
        redBtn.SetEvent(delegate { DisableAll(); GambleGameMN.Instance.PlayGambleGame(5); });
    }

    public override void ShowPopup(bool isShow = true)
    {
        base.ShowPopup(isShow);
        LastWinSetting();
        GameMN.Instance.isShowingPopup = isShow;
        if (!isShow) return;

        GameSetting(isNewGame: true);
        halfBtn.gameObject.SetActive(true);
    }

    public override void HidePopup()
    {
        base.HidePopup();
    }

    //IEnumerator BetBtnSetting()
    //{
    //    bool isHalf = !halfBtn.gameObject.activeSelf;

    //    //halfBtn.Disable(true);

    //    yield return new WaitForSeconds(0.7f);
    //    //halfBtn.Show(isHalf);
    //}

    public void GameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {
        StartCoroutine(IEGameSetting(isNewGame, isWin, result));
    }

    IEnumerator IEGameSetting(bool isNewGame, bool isWin = false, GambleResult result = null)
    {
        if (isNewGame)
        {
            DisableAll(false);
            MainCardSetting(isBack: true);
            notificationTxt.text = "";

            //StartCoroutine(IEChooseSuitAnims());
            //StartCoroutine(IEChooseColorAnims());
            //StartCoroutine(IEWinAnims());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            MainCardSetting(isBack: false, result.number, result.suit);
            string str = isWin ? youWinStr : dealerWinStr;
            ShowNotification(str);
            yield return new WaitForSeconds(2f);

            if (isWin)
            {
                //SoundMN.Instance.PlayOneShot(SFXType.GAMBLE_WIN);
                GameSetting(isNewGame: true);
            }
            else
            {
                halfBtn.Disable(true);
                collectBtn.Disable(true);
                HidePopup();
                UIMN.Instance.ShowGUINormal();
            }
        }

        GambleGameMN.Instance.AmountSetting();

        CardHistorySetting();
        BetSetting();
        WinSetting();
    }

    private void CardHistorySetting()
    {
        for (int i = 0; i < historyCards.Count; i++)
        {
            GambleCard gambleCard = historyCards[i];
            if (i < GambleGameMN.Instance.gambleResults.Count)
                gambleCard.FaceSetting(isBack: false, 0, GambleGameMN.Instance.gambleResults[i]);
            else
                gambleCard.FaceSetting(isBack: true);
        }
    }

    public void LastWinSetting()
    {
        winTxt.text = UIMN.Instance.winTxt.text;
    }

    public void WinSetting()
    {
        x4WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 4);
        x2WinTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.currentBet * 2);
    }

    public void BetSetting()
    {
        amountTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.amount);
        winTxt.text = Ultility.GetMoneyFormated(GambleGameMN.Instance.amount);
        balanceTxt.text = Ultility.GetMoneyFormated(UserData.balance_point);
    }

    private void MainCardSetting(bool isBack, int number = -1, int suit = 0)
    {
        mainCard.FaceSetting(isBack, number, suit);
    }

    private void DisableAll(bool isDisable = true)
    {
        StopAllCoroutines();

        //halfBtn.Disable(isDisable);
        collectBtn.Disable(isDisable);
        spadeBtn.Disable(isDisable);
        clubBtn.Disable(isDisable);
        diamondBtn.Disable(isDisable);
        heartBtn.Disable(isDisable);
        blackBtn.Disable(isDisable);
        redBtn.Disable(isDisable);

        //spadeBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
        //clubBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
        //diamondBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
        //heartBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
        //blackBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
        //redBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();

        x4WinTxt.color = Color.white;
        x2WinTxt.color = Color.white;
    }

    //IEnumerator IEChooseSuitAnims()
    //{
    //    spadeBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    spadeBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
    //    heartBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    heartBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
    //    diamondBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    diamondBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
    //    clubBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    clubBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();

    //    StartCoroutine(IEChooseSuitAnims());
    //}

    //IEnumerator IEChooseColorAnims()
    //{
    //    redBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    redBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();
    //    blackBtn.GetComponent<SprAnim>().SetLastFrameClickButton();
    //    yield return new WaitForSeconds(timeLoop);
    //    blackBtn.GetComponent<SprAnim>().SetFirstFrameClickButton();

    //    StartCoroutine(IEChooseColorAnims());
    //}

    //IEnumerator IEWinAnims()
    //{
    //    x4WinTxt.color = Color.white;
    //    x2WinTxt.color = Color.white;
    //    notificationTxt.color = Color.white;
    //    yield return new WaitForSeconds(timeLoop);
    //    x4WinTxt.color = new Color(1f, 1f, 1f, 0f);
    //    x2WinTxt.color = new Color(1f, 1f, 1f, 0f);
    //    notificationTxt.color = new Color(1f, 1f, 1f, 0f);
    //    yield return new WaitForSeconds(timeLoop);

    //    StartCoroutine(IEWinAnims());
    //}

    private void ShowNotification(string str)
    {
        if (str == youWinStr) notificationTxt.color = new Color(1, 0.8433768f, 0);
        if (str == dealerWinStr) notificationTxt.color = new Color(0.3018868f, 0.3018868f, 0.03018868f);
        notificationTxt.text = str;
    }
}
